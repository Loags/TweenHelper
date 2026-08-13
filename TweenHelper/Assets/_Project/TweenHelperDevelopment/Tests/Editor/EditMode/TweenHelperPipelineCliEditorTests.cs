using System;
using System.IO;
using System.Linq;
using LB.TweenHelper.Pipeline.Editor;
using NUnit.Framework;
using Unity.Pipeline.Commands;
using UnityEngine;

namespace LB.TweenHelper.Tests.Editor
{
    public sealed class TweenHelperPipelineCliEditorTests
    {
        private string _testRoot;

        [SetUp]
        public void SetUp()
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            _testRoot = Path.Combine(projectRoot, "Temp", "TweenHelperTelemetryTests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_testRoot);
            CliCommandTelemetry.ConfigureForTests(_testRoot, false);
        }

        [TearDown]
        public void TearDown()
        {
            CliCommandTelemetry.ResetTestConfiguration();
            if (!Directory.Exists(_testRoot)) return;

            foreach (string file in Directory.GetFiles(_testRoot)) File.Delete(file);
            Directory.Delete(_testRoot);
        }

        [Test]
        public void Commands_ExposeTheCatalogAndDeveloperTelemetrySummary()
        {
            CommandInfo[] commands = CommandRegistry.DiscoverCommands()
                .Where(command => command.Name.StartsWith("tween_helper_", StringComparison.Ordinal))
                .OrderBy(command => command.Name, StringComparer.Ordinal)
                .ToArray();

            Assert.AreEqual(2, commands.Length);
            Assert.AreEqual("tween_helper_catalog", commands[0].Name);
            CollectionAssert.AreEqual(new[] { "query", "family", "offset", "limit" }, commands[0].Parameters.Select(parameter => parameter.Name).ToArray());
            Assert.IsTrue(commands[0].Parameters.All(parameter => !parameter.Required));
            Assert.AreEqual("tween_helper_dev_telemetry_summary", commands[1].Name);
            Assert.IsEmpty(commands[1].Parameters);
            Assert.IsTrue(commands.All(command => command.MainThreadRequired));
            CollectionAssert.AreEqual(CliCommandTelemetry.CommandIds.OrderBy(commandId => commandId, StringComparer.Ordinal).ToArray(), commands.Select(command => command.Name).ToArray());
        }

        [Test]
        public void Catalog_PagesAllBuiltInPresetsWithoutChangingRegistry()
        {
            int registryCount = TweenPresetRegistry.Count;
            string[] registryNames = TweenPresetRegistry.PresetNames.OrderBy(name => name, StringComparer.Ordinal).ToArray();

            TweenHelperCatalogResult first = TweenHelperPipelineCommands.GetCatalog(limit: 100);
            TweenHelperCatalogResult second = TweenHelperPipelineCommands.GetCatalog(offset: 100, limit: 100);
            TweenHelperCatalogResult third = TweenHelperPipelineCommands.GetCatalog(offset: 200, limit: 100);
            TweenHelperPresetInfo[] presets = first.Presets.Concat(second.Presets).Concat(third.Presets).ToArray();

            Assert.AreEqual(300, first.PresetCount);
            Assert.AreEqual(300, first.MatchedCount);
            Assert.AreEqual(300, presets.Length);
            Assert.AreEqual(300, presets.Select(preset => preset.Name).Distinct(StringComparer.Ordinal).Count());
            Assert.IsTrue(presets.All(preset => !string.IsNullOrWhiteSpace(preset.Name) && preset.DefaultDuration > 0f));
            Assert.IsFalse(third.HasMore);
            Assert.AreEqual(registryCount, TweenPresetRegistry.Count);
            CollectionAssert.AreEqual(registryNames, TweenPresetRegistry.PresetNames.OrderBy(name => name, StringComparer.Ordinal).ToArray());
        }

        [Test]
        public void Catalog_FiltersAndValidatesSimplePagingArguments()
        {
            TweenHelperCatalogResult filtered = TweenHelperPipelineCommands.GetCatalog(query: "fade", limit: 100);

            Assert.Greater(filtered.MatchedCount, 0);
            Assert.IsTrue(filtered.Presets.All(preset => Contains(preset.Name, "fade") || Contains(preset.Family, "fade") || Contains(preset.Description, "fade")));
            Assert.Throws<ArgumentOutOfRangeException>(() => TweenHelperPipelineCommands.GetCatalog(offset: -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => TweenHelperPipelineCommands.GetCatalog(limit: 101));
        }

        [Test]
        public void Telemetry_Disabled_DoesNotCreateFiles()
        {
            TweenHelperPipelineCommands.GetCatalog(limit: 1);

            Assert.IsEmpty(Directory.GetFiles(_testRoot));
        }

        [Test]
        public void Telemetry_DefaultsToEnabledAndPersistsOptOut()
        {
            CliCommandTelemetry.ConfigureForTests(_testRoot, null);

            Assert.IsTrue(CliCommandTelemetry.Enabled);

            CliCommandTelemetry.Enabled = false;

            Assert.IsFalse(CliCommandTelemetry.Enabled);
            CollectionAssert.AreEqual(new[] { "cli-telemetry.disabled" }, Directory.GetFiles(_testRoot).Select(Path.GetFileName).ToArray());

            CliCommandTelemetry.Enabled = true;

            Assert.IsTrue(CliCommandTelemetry.Enabled);
            Assert.IsEmpty(Directory.GetFiles(_testRoot));
        }

        [Test]
        public void Telemetry_RecordsEveryRegisteredCommandOnce()
        {
            CliCommandTelemetry.Enabled = true;

            TweenHelperPipelineCommands.GetCatalog(limit: 1);
            TweenHelperTelemetrySummaryResult summary = TweenHelperPipelineCommands.GetTelemetrySummary();
            CliCommandTelemetry.CliTelemetryEvent[] events = CliCommandTelemetry.ReadEventsForTests().ToArray();

            Assert.IsTrue(summary.RecordingEnabled);
            Assert.AreEqual(1, summary.TotalCalls);
            Assert.AreEqual(2, events.Length);
            CollectionAssert.AreEqual(new[] { CliCommandTelemetry.CatalogCommandId, CliCommandTelemetry.SummaryCommandId }, events.Select(item => item.CommandId).ToArray());
            Assert.IsTrue(events.All(item => item.Status == "success" && item.DurationMs >= 0d));
        }

        [Test]
        public void Telemetry_StoresNoInputsOrExceptionMessagesAndRethrowsOriginalException()
        {
            CliCommandTelemetry.Enabled = true;
            const string secret = "C:/private/project/secret-source-value";
            var expectedException = new InvalidOperationException(secret);

            TweenHelperPipelineCommands.GetCatalog(query: secret, limit: 1);
            InvalidOperationException actualException = Assert.Throws<InvalidOperationException>(() =>
                CliCommandTelemetry.Record<object>(CliCommandTelemetry.CatalogCommandId, () => throw expectedException));
            string storedText = File.ReadAllText(CliCommandTelemetry.ActiveFilePath);
            CliCommandTelemetry.CliTelemetryEvent exceptionEvent = CliCommandTelemetry.ReadEventsForTests().Last();

            Assert.AreSame(expectedException, actualException);
            StringAssert.DoesNotContain(secret, storedText);
            Assert.AreEqual("exception", exceptionEvent.Status);
            Assert.AreEqual(nameof(InvalidOperationException), exceptionEvent.ExceptionType);
        }

        [Test]
        public void Telemetry_WriteFailure_DoesNotChangeCommandResult()
        {
            string blockedStoragePath = Path.Combine(_testRoot, "not-a-directory");
            File.WriteAllText(blockedStoragePath, "file");
            CliCommandTelemetry.ConfigureForTests(blockedStoragePath, true);
            var expectedException = new InvalidOperationException("command failure");

            TweenHelperCatalogResult result = TweenHelperPipelineCommands.GetCatalog(limit: 1);
            InvalidOperationException actualException = Assert.Throws<InvalidOperationException>(() =>
                CliCommandTelemetry.Record<object>(CliCommandTelemetry.CatalogCommandId, () => throw expectedException));

            Assert.AreEqual(1, result.ReturnedCount);
            Assert.AreSame(expectedException, actualException);
        }

        [Test]
        public void Telemetry_RotatesToOneBackupAtTheSizeLimit()
        {
            CliCommandTelemetry.Enabled = true;
            File.WriteAllBytes(CliCommandTelemetry.ActiveFilePath, new byte[(int)CliCommandTelemetry.MaximumFileBytes]);

            TweenHelperPipelineCommands.GetCatalog(limit: 1);

            Assert.IsTrue(File.Exists(CliCommandTelemetry.ActiveFilePath));
            Assert.IsTrue(File.Exists(CliCommandTelemetry.BackupFilePath));
            Assert.AreEqual(1, CliCommandTelemetry.ReadEventsForTests().Count);
            Assert.AreEqual(2, Directory.GetFiles(_testRoot, "cli-telemetry-v1*.jsonl").Length);
        }

        [Test]
        public void Telemetry_SummarySkipsMalformedLinesAndReportsThem()
        {
            CliCommandTelemetry.Enabled = true;
            TweenHelperPipelineCommands.GetCatalog(limit: 1);
            File.AppendAllText(CliCommandTelemetry.ActiveFilePath, "not-json" + Environment.NewLine);

            TweenHelperTelemetrySummaryResult summary = TweenHelperPipelineCommands.GetTelemetrySummary();

            Assert.AreEqual(1, summary.TotalCalls);
            Assert.AreEqual(1, summary.MalformedLineCount);
            Assert.IsFalse(summary.ReadFailed);
            TweenHelperTelemetryCommandSummary catalog = summary.Commands.Single(item => item.CommandId == CliCommandTelemetry.CatalogCommandId);
            Assert.AreEqual(1, catalog.Count);
            Assert.AreEqual(1, catalog.SuccessCount);
            Assert.AreEqual(0, catalog.ExceptionCount);
        }

        private static bool Contains(string value, string query) => value.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
