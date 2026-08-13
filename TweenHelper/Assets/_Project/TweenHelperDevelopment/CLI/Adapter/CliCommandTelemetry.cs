using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace LB.TweenHelper.Pipeline.Editor
{
    internal static class CliCommandTelemetry
    {
        internal const string CatalogCommandId = "tween_helper_catalog";
        internal const string SummaryCommandId = "tween_helper_dev_telemetry_summary";
        internal const int EventSchemaVersion = 1;
        internal const long MaximumFileBytes = 5L * 1024L * 1024L;

        private const string ActiveFileName = "cli-telemetry-v1.jsonl";
        private const string BackupFileName = "cli-telemetry-v1.old.jsonl";
        private const string DisabledFileName = "cli-telemetry.disabled";
        private const string LegacyEnabledFileName = "cli-telemetry.enabled";
        private const string UnknownCommandId = "unknown";

        private static readonly object FileLock = new object();
        private static readonly Encoding Utf8NoBom = new UTF8Encoding(false);
        private static readonly string[] KnownCommandIdList = { CatalogCommandId, SummaryCommandId };
        private static readonly HashSet<string> KnownCommandIds = new HashSet<string>(KnownCommandIdList, StringComparer.Ordinal);
        private static readonly HashSet<string> KnownStatuses = new HashSet<string>(new[] { "success", "exception" }, StringComparer.Ordinal);
        private static readonly string[] CoverageExclusions =
        {
            "requests rejected before handler entry",
            "Pipeline built-in and third-party commands",
            "non-Pipeline tools",
            "domain-service calls that bypass the command adapter"
        };

        private static string _storageDirectoryOverride;
        private static bool? _enabledOverride;

        internal static bool Enabled
        {
            get
            {
                if (_enabledOverride.HasValue) return _enabledOverride.Value;

                try
                {
                    return !File.Exists(DisabledFilePath);
                }
                catch
                {
                    return true;
                }
            }
            set
            {
                if (_enabledOverride.HasValue)
                {
                    _enabledOverride = value;
                    return;
                }

                if (value)
                {
                    if (File.Exists(DisabledFilePath)) File.Delete(DisabledFilePath);
                    if (File.Exists(LegacyEnabledFilePath)) File.Delete(LegacyEnabledFilePath);
                    return;
                }

                Directory.CreateDirectory(StorageDirectory);
                File.WriteAllText(DisabledFilePath, "disabled", Utf8NoBom);
                if (File.Exists(LegacyEnabledFilePath)) File.Delete(LegacyEnabledFilePath);
            }
        }

        internal static IReadOnlyList<string> CommandIds => KnownCommandIdList;
        internal static string ActiveFilePath => Path.Combine(StorageDirectory, ActiveFileName);
        internal static string BackupFilePath => Path.Combine(StorageDirectory, BackupFileName);

        private static string DisabledFilePath => Path.Combine(StorageDirectory, DisabledFileName);
        private static string LegacyEnabledFilePath => Path.Combine(StorageDirectory, LegacyEnabledFileName);

        private static string StorageDirectory
        {
            get
            {
                if (!string.IsNullOrEmpty(_storageDirectoryOverride)) return _storageDirectoryOverride;

                string projectRoot = Directory.GetParent(Application.dataPath)?.FullName;
                if (string.IsNullOrEmpty(projectRoot)) throw new InvalidOperationException("TweenHelper telemetry could not resolve the Unity project root.");
                return Path.Combine(projectRoot, "Library", "TweenHelper");
            }
        }

        internal static T Record<T>(string commandId, Func<T> command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (!Enabled) return command();

            var stopwatch = Stopwatch.StartNew();
            Exception commandException = null;
            try
            {
                return command();
            }
            catch (Exception exception)
            {
                commandException = exception;
                throw;
            }
            finally
            {
                stopwatch.Stop();
                try
                {
                    AppendEvent(CreateEvent(commandId, stopwatch.Elapsed.TotalMilliseconds, commandException));
                }
                catch
                {
                }
            }
        }

        internal static TweenHelperTelemetrySummaryResult BuildSummary()
        {
            CliTelemetryReadResult readResult;
            long currentFileBytes = 0;

            lock (FileLock)
            {
                try
                {
                    if (File.Exists(ActiveFilePath)) currentFileBytes = new FileInfo(ActiveFilePath).Length;
                    readResult = ReadEvents();
                }
                catch
                {
                    readResult = new CliTelemetryReadResult { ReadFailed = true };
                }
            }

            TweenHelperTelemetryCommandSummary[] commandSummaries = readResult.Events
                .GroupBy(item => item.CommandId, StringComparer.Ordinal)
                .OrderBy(group => group.Key, StringComparer.Ordinal)
                .Select(group => new TweenHelperTelemetryCommandSummary
                {
                    CommandId = group.Key,
                    Count = group.Count(),
                    SuccessCount = group.Count(item => item.Status == "success"),
                    ExceptionCount = group.Count(item => item.Status == "exception"),
                    AverageDurationMs = RoundMilliseconds(group.Average(item => item.DurationMs)),
                    MaximumDurationMs = RoundMilliseconds(group.Max(item => item.DurationMs))
                })
                .ToArray();

            TweenHelperTelemetryStatusSummary[] statusSummaries = readResult.Events
                .GroupBy(item => item.Status, StringComparer.Ordinal)
                .OrderBy(group => group.Key, StringComparer.Ordinal)
                .Select(group => new TweenHelperTelemetryStatusSummary { Status = group.Key, Count = group.Count() })
                .ToArray();

            return new TweenHelperTelemetrySummaryResult
            {
                RecordingEnabled = Enabled,
                EventSchemaVersion = EventSchemaVersion,
                Coverage = "registered tween_helper_* handlers after Pipeline argument binding",
                Excluded = CoverageExclusions.ToArray(),
                MaximumFileBytes = MaximumFileBytes,
                CurrentFileBytes = currentFileBytes,
                TotalCalls = readResult.Events.Count,
                MalformedLineCount = readResult.MalformedLineCount,
                ReadFailed = readResult.ReadFailed,
                Commands = commandSummaries,
                Statuses = statusSummaries
            };
        }

        internal static IReadOnlyList<CliTelemetryEvent> ReadEventsForTests()
        {
            lock (FileLock)
            {
                return ReadEvents().Events.ToArray();
            }
        }

        internal static void ConfigureForTests(string storageDirectory, bool? enabled)
        {
            _storageDirectoryOverride = Path.GetFullPath(storageDirectory);
            _enabledOverride = enabled;
        }

        internal static void ResetTestConfiguration()
        {
            _storageDirectoryOverride = null;
            _enabledOverride = null;
        }

        private static CliTelemetryEvent CreateEvent(string commandId, double durationMs, Exception exception)
        {
            return new CliTelemetryEvent
            {
                SchemaVersion = EventSchemaVersion,
                TimestampUtc = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture),
                CommandId = NormalizeCommandId(commandId),
                DurationMs = RoundMilliseconds(durationMs),
                Status = exception == null ? "success" : "exception",
                ExceptionType = exception == null ? null : SanitizeExceptionType(exception.GetType().Name)
            };
        }

        private static void AppendEvent(CliTelemetryEvent telemetryEvent)
        {
            string line = JsonConvert.SerializeObject(telemetryEvent, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            long appendedBytes = Utf8NoBom.GetByteCount(line + "\n");

            lock (FileLock)
            {
                Directory.CreateDirectory(StorageDirectory);
                RotateIfNeeded(appendedBytes);
                File.AppendAllText(ActiveFilePath, line + "\n", Utf8NoBom);
            }
        }

        private static void RotateIfNeeded(long appendedBytes)
        {
            if (!File.Exists(ActiveFilePath)) return;
            if (new FileInfo(ActiveFilePath).Length + appendedBytes <= MaximumFileBytes) return;

            EnsureTelemetryFile(ActiveFilePath);
            EnsureTelemetryFile(BackupFilePath);
            if (File.Exists(BackupFilePath)) File.Delete(BackupFilePath);
            File.Move(ActiveFilePath, BackupFilePath);
        }

        private static void EnsureTelemetryFile(string path)
        {
            string root = Path.GetFullPath(StorageDirectory).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
            string candidate = Path.GetFullPath(path);
            if (!candidate.StartsWith(root, StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("Telemetry file resolved outside the telemetry directory.");
        }

        private static CliTelemetryReadResult ReadEvents()
        {
            var result = new CliTelemetryReadResult();
            if (!File.Exists(ActiveFilePath)) return result;

            string content = ReadBoundedText(ActiveFilePath);
            string[] lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                try
                {
                    var telemetryEvent = JsonConvert.DeserializeObject<CliTelemetryEvent>(line);
                    if (!IsValid(telemetryEvent))
                    {
                        result.MalformedLineCount++;
                        continue;
                    }

                    result.Events.Add(telemetryEvent);
                }
                catch
                {
                    result.MalformedLineCount++;
                }
            }

            return result;
        }

        private static string ReadBoundedText(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                int length = (int)Math.Min(stream.Length, MaximumFileBytes);
                var buffer = new byte[length];
                int offset = 0;
                while (offset < length)
                {
                    int read = stream.Read(buffer, offset, length - offset);
                    if (read == 0) break;
                    offset += read;
                }

                return Utf8NoBom.GetString(buffer, 0, offset);
            }
        }

        private static bool IsValid(CliTelemetryEvent telemetryEvent)
        {
            if (telemetryEvent == null || telemetryEvent.SchemaVersion != EventSchemaVersion) return false;
            if (!KnownCommandIds.Contains(telemetryEvent.CommandId)) return false;
            if (!KnownStatuses.Contains(telemetryEvent.Status)) return false;
            if (telemetryEvent.DurationMs < 0d || double.IsNaN(telemetryEvent.DurationMs) || double.IsInfinity(telemetryEvent.DurationMs)) return false;
            return DateTime.TryParseExact(telemetryEvent.TimestampUtc, "O", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out _);
        }

        private static string NormalizeCommandId(string commandId) => KnownCommandIds.Contains(commandId) ? commandId : UnknownCommandId;

        private static string SanitizeExceptionType(string exceptionType)
        {
            if (string.IsNullOrEmpty(exceptionType)) return null;
            return new string(exceptionType.Where(character => char.IsLetterOrDigit(character) || character == '_').Take(128).ToArray());
        }

        private static double RoundMilliseconds(double value) => Math.Round(value, 3, MidpointRounding.AwayFromZero);

        internal sealed class CliTelemetryEvent
        {
            [JsonProperty("schemaVersion")]
            public int SchemaVersion { get; set; }

            [JsonProperty("timestampUtc")]
            public string TimestampUtc { get; set; }

            [JsonProperty("commandId")]
            public string CommandId { get; set; }

            [JsonProperty("durationMs")]
            public double DurationMs { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("exceptionType", NullValueHandling = NullValueHandling.Ignore)]
            public string ExceptionType { get; set; }
        }

        private sealed class CliTelemetryReadResult
        {
            public List<CliTelemetryEvent> Events { get; } = new List<CliTelemetryEvent>();
            public int MalformedLineCount { get; set; }
            public bool ReadFailed { get; set; }
        }
    }

    internal static class CliCommandTelemetryMenu
    {
        private const string MenuPath = "Tools/TweenHelper/Development/Record CLI Telemetry";

        [MenuItem(MenuPath)]
        private static void ToggleRecording()
        {
            CliCommandTelemetry.Enabled = !CliCommandTelemetry.Enabled;
            Menu.SetChecked(MenuPath, CliCommandTelemetry.Enabled);
        }

        [MenuItem(MenuPath, true)]
        private static bool ValidateToggleRecording()
        {
            Menu.SetChecked(MenuPath, CliCommandTelemetry.Enabled);
            return true;
        }
    }

    public sealed class TweenHelperTelemetrySummaryResult
    {
        [JsonProperty("recordingEnabled")]
        public bool RecordingEnabled { get; set; }

        [JsonProperty("eventSchemaVersion")]
        public int EventSchemaVersion { get; set; }

        [JsonProperty("coverage")]
        public string Coverage { get; set; }

        [JsonProperty("excluded")]
        public string[] Excluded { get; set; }

        [JsonProperty("maximumFileBytes")]
        public long MaximumFileBytes { get; set; }

        [JsonProperty("currentFileBytes")]
        public long CurrentFileBytes { get; set; }

        [JsonProperty("totalCalls")]
        public int TotalCalls { get; set; }

        [JsonProperty("malformedLineCount")]
        public int MalformedLineCount { get; set; }

        [JsonProperty("readFailed")]
        public bool ReadFailed { get; set; }

        [JsonProperty("commands")]
        public TweenHelperTelemetryCommandSummary[] Commands { get; set; }

        [JsonProperty("statuses")]
        public TweenHelperTelemetryStatusSummary[] Statuses { get; set; }
    }

    public sealed class TweenHelperTelemetryCommandSummary
    {
        [JsonProperty("commandId")]
        public string CommandId { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("successCount")]
        public int SuccessCount { get; set; }

        [JsonProperty("exceptionCount")]
        public int ExceptionCount { get; set; }

        [JsonProperty("averageDurationMs")]
        public double AverageDurationMs { get; set; }

        [JsonProperty("maximumDurationMs")]
        public double MaximumDurationMs { get; set; }
    }

    public sealed class TweenHelperTelemetryStatusSummary
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
