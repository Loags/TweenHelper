using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LB.TweenHelper.Demo
{
    /// <summary>
    /// Play mode diagnostic runner that sweeps preset displays and verifies reset reliability.
    /// Uses AnimationResetManager.ResetLast/ResetAll directly (not keyboard simulation).
    /// </summary>
    [AddComponentMenu("LB/TweenHelper/Demo/Animation Reset Audit Runner")]
    public class AnimationResetAuditRunner : MonoBehaviour
    {
        public enum AuditMode
        {
            ResetLastSweep,
            ResetAllBatchSweep,
            RepeatPlayThenReset,
            StressMixed
        }

        [Serializable]
        private class AuditEntry
        {
            public string PresetName;
            public string Family;
            public string Mode;
            public bool Passed;
            public string FailureReason;
            public string Notes;
        }

        [Header("Execution")]
        [SerializeField] private bool autoRunOnStart;
        [SerializeField] private AuditMode mode = AuditMode.ResetLastSweep;
        [SerializeField] private bool ensureAllVisibleViaSpawner = true;
        [SerializeField] private bool clearHistoryBetweenCases = true;
        [SerializeField] private bool verbosePerPresetLogs;

        [Header("Timing")]
        [SerializeField] private float settleBeforeResetSeconds = 0.2f;
        [SerializeField] private float postResetWatchWindowSeconds = 0.35f;
        [SerializeField] private float betweenPresetDelaySeconds = 0.02f;
        [SerializeField] private float repeatPlayGapSeconds = 0.05f;
        [SerializeField] private int resetAllBatchSize = 10;
        [SerializeField] private int repeatPlayCount = 3;

        [Header("Verification Tolerances")]
        [SerializeField] private float positionTolerance = 0.001f;
        [SerializeField] private float scaleTolerance = 0.001f;
        [SerializeField] private float rotationAngleTolerance = 0.1f;
        [SerializeField] private float alphaTolerance = 0.01f;

        [Header("Reporting")]
        [SerializeField] private bool writeReportToTemp = true;
        [SerializeField] private string reportFileName = "TweenHelper_ResetAuditReport.txt";

        private Coroutine _auditCoroutine;
        private readonly List<AuditEntry> _results = new List<AuditEntry>();

        private void Start()
        {
            if (autoRunOnStart)
            {
                RunSelectedAudit();
            }
        }

        [ContextMenu("Run Selected Audit")]
        public void RunSelectedAudit()
        {
            StartAudit(mode);
        }

        [ContextMenu("Run ResetLast Sweep")]
        public void RunResetLastSweep()
        {
            StartAudit(AuditMode.ResetLastSweep);
        }

        [ContextMenu("Run ResetAll Batch Sweep")]
        public void RunResetAllBatchSweep()
        {
            StartAudit(AuditMode.ResetAllBatchSweep);
        }

        [ContextMenu("Run RepeatPlayThenReset")]
        public void RunRepeatPlayThenReset()
        {
            StartAudit(AuditMode.RepeatPlayThenReset);
        }

        [ContextMenu("Run StressMixed")]
        public void RunStressMixed()
        {
            StartAudit(AuditMode.StressMixed);
        }

        [ContextMenu("Stop Audit")]
        public void StopAudit()
        {
            if (_auditCoroutine == null) return;
            StopCoroutine(_auditCoroutine);
            _auditCoroutine = null;
            Debug.Log("AnimationResetAuditRunner: Audit stopped.");
        }

        private void StartAudit(AuditMode requestedMode)
        {
            if (_auditCoroutine != null)
            {
                Debug.LogWarning("AnimationResetAuditRunner: Audit already running.");
                return;
            }

            _auditCoroutine = StartCoroutine(RunAuditCoroutine(requestedMode));
        }

        private IEnumerator RunAuditCoroutine(AuditMode requestedMode)
        {
            _results.Clear();

            yield return null;

            var manager = EnsureResetManager();
            if (manager == null)
            {
                Debug.LogError("AnimationResetAuditRunner: Could not find or create AnimationResetManager.");
                _auditCoroutine = null;
                yield break;
            }

            var spawner = PresetShowcaseSpawner.Instance;
            if (ensureAllVisibleViaSpawner && spawner != null)
            {
                spawner.RelayoutWithFilter(_ => true);
                yield return null;
            }

            var displays = FindObjectsByType<AnimationPresetDisplay>(FindObjectsSortMode.None)
                .Where(d => d != null && d.gameObject.activeInHierarchy)
                .OrderBy(d => d.PresetName, StringComparer.Ordinal)
                .ToList();

            if (displays.Count == 0)
            {
                Debug.LogWarning("AnimationResetAuditRunner: No active AnimationPresetDisplay instances found.");
                _auditCoroutine = null;
                yield break;
            }

            Debug.Log($"AnimationResetAuditRunner: Starting {requestedMode} across {displays.Count} displays.");

            if (clearHistoryBetweenCases)
            {
                manager.ClearHistory();
            }

            switch (requestedMode)
            {
                case AuditMode.ResetLastSweep:
                    yield return RunResetLastSweep(displays, manager);
                    break;
                case AuditMode.ResetAllBatchSweep:
                    yield return RunResetAllBatchSweep(displays, manager);
                    break;
                case AuditMode.RepeatPlayThenReset:
                    yield return RunRepeatPlayThenReset(displays, manager);
                    break;
                case AuditMode.StressMixed:
                    yield return RunStressMixed(displays, manager);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            string report = BuildReport(requestedMode, displays.Count);
            Debug.Log(report);

            if (writeReportToTemp)
            {
                TryWriteReportToTemp(report);
            }

            _auditCoroutine = null;
        }

        private IEnumerator RunResetLastSweep(List<AnimationPresetDisplay> displays, AnimationResetManager manager)
        {
            for (int i = 0; i < displays.Count; i++)
            {
                var display = displays[i];
                if (display == null) continue;

                if (clearHistoryBetweenCases)
                {
                    manager.ClearHistory();
                }

                display.ResetAnimation();
                yield return null;

                display.PlayPreset();
                yield return WaitSeconds(settleBeforeResetSeconds);

                manager.ResetLast();
                yield return null;

                yield return VerifyDisplayWithWatch(display, "ResetLastSweep");

                if (betweenPresetDelaySeconds > 0f)
                {
                    yield return WaitSeconds(betweenPresetDelaySeconds);
                }
            }
        }

        private IEnumerator RunResetAllBatchSweep(List<AnimationPresetDisplay> displays, AnimationResetManager manager)
        {
            int batchSize = Mathf.Max(1, resetAllBatchSize);

            for (int start = 0; start < displays.Count; start += batchSize)
            {
                if (clearHistoryBetweenCases)
                {
                    manager.ClearHistory();
                }

                int end = Mathf.Min(displays.Count, start + batchSize);
                var batch = new List<AnimationPresetDisplay>(end - start);

                for (int i = start; i < end; i++)
                {
                    var display = displays[i];
                    if (display == null) continue;

                    display.ResetAnimation();
                    yield return null;

                    display.PlayPreset();
                    batch.Add(display);

                    if (betweenPresetDelaySeconds > 0f)
                    {
                        yield return WaitSeconds(betweenPresetDelaySeconds);
                    }
                }

                yield return WaitSeconds(settleBeforeResetSeconds);
                manager.ResetAll();
                yield return null;

                foreach (var display in batch)
                {
                    yield return VerifyDisplayWithWatch(display, $"ResetAllBatchSweep(batch={start / batchSize + 1})");
                }
            }
        }

        private IEnumerator RunRepeatPlayThenReset(List<AnimationPresetDisplay> displays, AnimationResetManager manager)
        {
            int repeats = Mathf.Max(2, repeatPlayCount);

            foreach (var display in displays)
            {
                if (display == null) continue;

                if (clearHistoryBetweenCases)
                {
                    manager.ClearHistory();
                }

                display.ResetAnimation();
                yield return null;

                for (int i = 0; i < repeats; i++)
                {
                    display.PlayPreset();
                    if (repeatPlayGapSeconds > 0f)
                    {
                        yield return WaitSeconds(repeatPlayGapSeconds);
                    }
                    else
                    {
                        yield return null;
                    }
                }

                yield return WaitSeconds(settleBeforeResetSeconds);
                manager.ResetLast();
                yield return null;

                yield return VerifyDisplayWithWatch(display, $"RepeatPlayThenReset(repeats={repeats})");
            }
        }

        private IEnumerator RunStressMixed(List<AnimationPresetDisplay> displays, AnimationResetManager manager)
        {
            int batchSize = Mathf.Max(2, Mathf.Min(resetAllBatchSize, 12));

            for (int i = 0; i < displays.Count; i++)
            {
                var display = displays[i];
                if (display == null) continue;

                if (i % 5 == 0)
                {
                    // Mini batch reset-all cycle.
                    if (clearHistoryBetweenCases)
                    {
                        manager.ClearHistory();
                    }

                    var batch = displays.Skip(i).Take(batchSize).Where(d => d != null).ToList();
                    foreach (var item in batch)
                    {
                        item.ResetAnimation();
                        yield return null;
                        item.PlayPreset();
                    }

                    yield return WaitSeconds(settleBeforeResetSeconds);
                    manager.ResetAll();
                    yield return null;

                    foreach (var item in batch)
                    {
                        yield return VerifyDisplayWithWatch(item, "StressMixed-ResetAll");
                    }

                    i += batch.Count - 1;
                    continue;
                }

                // Single-item repeated play then reset-last cycle.
                if (clearHistoryBetweenCases)
                {
                    manager.ClearHistory();
                }

                display.ResetAnimation();
                yield return null;
                display.PlayPreset();
                yield return WaitSeconds(repeatPlayGapSeconds);
                display.PlayPreset();
                yield return WaitSeconds(settleBeforeResetSeconds);
                manager.ResetLast();
                yield return null;
                yield return VerifyDisplayWithWatch(display, "StressMixed-ResetLast");
            }
        }

        private IEnumerator VerifyDisplayWithWatch(AnimationPresetDisplay display, string modeLabel)
        {
            var immediate = display.VerifyResetState(positionTolerance, scaleTolerance, rotationAngleTolerance, alphaTolerance);

            if (postResetWatchWindowSeconds > 0f)
            {
                yield return WaitSeconds(postResetWatchWindowSeconds);
            }

            var afterWatch = display.VerifyResetState(positionTolerance, scaleTolerance, rotationAngleTolerance, alphaTolerance);
            bool passed = immediate.Passed && afterWatch.Passed;

            string failureReason = "";
            if (!immediate.Passed)
            {
                failureReason += $"Immediate: {immediate.Details}";
            }
            if (!afterWatch.Passed)
            {
                if (!string.IsNullOrEmpty(failureReason)) failureReason += " | ";
                failureReason += $"PostWatch: {afterWatch.Details}";
            }

            var entry = new AuditEntry
            {
                PresetName = display.PresetName,
                Family = PresetFamilyClassifier.GetFamilyName(display.PresetName),
                Mode = modeLabel,
                Passed = passed,
                FailureReason = failureReason,
                Notes = passed ? "" : "Reset verification failed"
            };
            _results.Add(entry);

            if (verbosePerPresetLogs || !passed)
            {
                Debug.Log($"AnimationResetAuditRunner: [{modeLabel}] {display.PresetName} => {(passed ? "PASS" : "FAIL")} {failureReason}");
            }
        }

        private static WaitForSeconds WaitSeconds(float seconds)
        {
            return seconds > 0f ? new WaitForSeconds(seconds) : null;
        }

        private AnimationResetManager EnsureResetManager()
        {
            if (AnimationResetManager.Instance != null)
            {
                return AnimationResetManager.Instance;
            }

            var go = new GameObject("AnimationResetManager");
            return go.AddComponent<AnimationResetManager>();
        }

        private string BuildReport(AuditMode requestedMode, int discoveredDisplayCount)
        {
            int total = _results.Count;
            int failures = _results.Count(r => !r.Passed);
            int passes = total - failures;

            var sb = new StringBuilder(4096);
            sb.AppendLine("=== TweenHelper Reset Audit Report ===");
            sb.AppendLine($"Timestamp (UTC): {DateTime.UtcNow:O}");
            sb.AppendLine($"Mode: {requestedMode}");
            sb.AppendLine($"Discovered Displays: {discoveredDisplayCount}");
            sb.AppendLine($"Cases Executed: {total}");
            sb.AppendLine($"Pass: {passes}");
            sb.AppendLine($"Fail: {failures}");

            if (failures > 0)
            {
                sb.AppendLine();
                sb.AppendLine("Failure counts by family:");
                foreach (var group in _results.Where(r => !r.Passed)
                             .GroupBy(r => r.Family)
                             .OrderByDescending(g => g.Count())
                             .ThenBy(g => g.Key, StringComparer.Ordinal))
                {
                    sb.AppendLine($"- {group.Key}: {group.Count()}");
                }

                sb.AppendLine();
                sb.AppendLine("Failures:");
                foreach (var entry in _results.Where(r => !r.Passed)
                             .OrderBy(r => r.Family, StringComparer.Ordinal)
                             .ThenBy(r => r.PresetName, StringComparer.Ordinal))
                {
                    sb.AppendLine($"- [{entry.Mode}] {entry.PresetName} ({entry.Family}) :: {entry.FailureReason}");
                }
            }

            return sb.ToString();
        }

        private void TryWriteReportToTemp(string report)
        {
            try
            {
                var projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
                var tempDir = Path.Combine(projectRoot, "Temp");
                Directory.CreateDirectory(tempDir);
                var path = Path.Combine(tempDir, reportFileName);
                File.WriteAllText(path, report);
                Debug.Log($"AnimationResetAuditRunner: Wrote report to {path}");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"AnimationResetAuditRunner: Failed to write report. {ex.Message}");
            }
        }
    }
}
