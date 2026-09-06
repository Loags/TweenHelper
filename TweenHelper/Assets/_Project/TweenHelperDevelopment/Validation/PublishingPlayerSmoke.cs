using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace LB.TweenHelper.Development
{
    public sealed class PublishingPlayerSmoke : MonoBehaviour
    {
        [Serializable]
        private sealed class Report
        {
            public string unity;
            public string device;
            public string operatingSystem;
            public string backend;
            public string result;
            public int registryCount;
            public bool customPresetPassed;
            public bool allocationCounterSupported;
            public double registryInitializationMs;
            public double cancellationCycleMs;
            public int cancellationCycles;
            public List<Measurement> measurements = new List<Measurement>();
        }

        [Serializable]
        private sealed class Measurement
        {
            public string workload;
            public int size;
            public double constructionMs;
            public long constructionBytes;
            public double medianStepMs;
            public double p95StepMs;
            public long stepBytes;
            public double cleanupMs;
            public int remainingTweens;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Begin()
        {
            if (!Environment.GetCommandLineArgs().Contains("--tweenhelper-smoke")) return;
            new GameObject("Publishing Player Smoke").AddComponent<PublishingPlayerSmoke>();
        }

        private async void Start()
        {
            var report = new Report
            {
                unity = Application.unityVersion,
                device = SystemInfo.processorType + " / " + SystemInfo.graphicsDeviceName,
                operatingSystem = SystemInfo.operatingSystem,
#if ENABLE_IL2CPP
                backend = "IL2CPP",
#else
                backend = "Mono",
#endif
            };
            int exitCode = 0;
            try
            {
                DOTween.Init();
                DOTween.SetTweensCapacity(2000, 1000);
                long probeStart = GC.GetAllocatedBytesForCurrentThread();
                var allocationProbe = new byte[4096];
                report.allocationCounterSupported = GC.GetAllocatedBytesForCurrentThread() - probeStart >= allocationProbe.Length;
                GC.KeepAlive(allocationProbe);
                var startupClock = Stopwatch.StartNew();
                TweenPresetRegistry.Refresh();
                report.registryInitializationMs = startupClock.Elapsed.TotalMilliseconds;
                report.registryCount = TweenPresetRegistry.Count;
                bool customPresetSmoke = Environment.GetCommandLineArgs().Contains("--custom-preset-smoke");
                Require(report.registryCount == (customPresetSmoke ? 301 : 300), "Expected all built-ins and only the requested customer fixture.");
                foreach (ITweenPreset preset in TweenPresetRegistry.Presets)
                    Require(TweenPresetRegistry.GetPresetByName(preset.PresetName) == preset, "Name lookup differs from registry.");

                var target = new GameObject("Name-only preset target");
                try
                {
                    if (customPresetSmoke)
                    {
                        ITweenPreset custom = TweenPresetRegistry.GetPresetByName("CustomerOnlyPreserved");
                        Require(custom != null, "Preserved customer preset was stripped.");
                        await TweenAsync.AwaitCompletion(custom.CreateTween(target, 0.02f).Play());
                        Require(Mathf.Approximately(target.transform.localPosition.x, 3f), "Customer preset did not run.");
                        report.customPresetPassed = true;
                    }
                    Tween retained = TweenPresetRegistry.GetPresetByName("PopIn").CreateTween(target, 0.02f).SetAutoKill(false).Play();
                    await TweenAsync.AwaitCompletion(retained);
                    Require(await TweenAsync.AwaitCompletionWithTimeout(retained, 1f), "Retained completion failed.");
                    retained.Kill();
                    using var cancellation = new CancellationTokenSource();
                    Tween canceled = target.transform.DOMoveX(3f, 10f);
                    int mainThread = Thread.CurrentThread.ManagedThreadId;
                    int killThread = 0;
                    canceled.onKill += () => killThread = Thread.CurrentThread.ManagedThreadId;
                    Task wait = TweenAsync.AwaitCompletion(canceled, cancellation.Token);
                    await Task.Run(() => cancellation.Cancel());
                    try { await wait; }
                    catch (OperationCanceledException) { }
                    Require(wait.IsCanceled && killThread == mainThread, "Worker cancellation failed.");
                    var cancellationClock = Stopwatch.StartNew();
                    for (int cycle = 0; cycle < 100; cycle++)
                    {
                        using var source = new CancellationTokenSource();
                        Tween pending = target.transform.DOMoveX(3f, 10f);
                        Task pendingWait = TweenAsync.AwaitCompletion(pending, source.Token);
                        source.Cancel();
                        try { await pendingWait; }
                        catch (OperationCanceledException) { }
                        Require(pendingWait.IsCanceled && !pending.IsActive(), "Repeated cancellation did not clean up.");
                        report.cancellationCycles++;
                    }
                    report.cancellationCycleMs = cancellationClock.Elapsed.TotalMilliseconds;
                }
                finally { Destroy(target); }

                foreach (int size in new[] { 10, 100, 500 })
                {
                    Measure(report, "transform", size, false);
                    Measure(report, "collection recipe", size, false);
                }
                foreach (int size in new[] { 32, 256, 1024 }) Measure(report, "rich TMP wave", size, true);
                report.result = "Passed";
            }
            catch (Exception exception)
            {
                report.result = exception.ToString();
                exitCode = 1;
                Debug.LogException(exception);
            }
            string path = Path.Combine(Application.dataPath, "..", "PublishingPlayerSmoke.json");
            File.WriteAllText(path, JsonUtility.ToJson(report, true));
            Debug.Log("Publishing player smoke: " + report.result + ". " + path);
            Application.Quit(exitCode);
        }

        private static void Measure(Report report, string workload, int size, bool text)
        {
            var targets = new List<GameObject>();
            var tweens = new List<Tween>();
            TweenRecipe recipe = null;
            int baseline = DOTween.TotalActiveTweens();
            try
            {
                if (text)
                {
                    var target = new GameObject("TMP benchmark");
                    targets.Add(target);
                    var label = target.AddComponent<TextMeshPro>();
                    label.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
                    Require(label.font != null, "Import TMP Essential Resources before the benchmark.");
                    label.text = "<b>" + new string('M', size) + "</b>";
                    label.rectTransform.sizeDelta = new Vector2(20000, 1000);
                    label.ForceMeshUpdate();
                }
                else
                {
                    for (int i = 0; i < size; i++) targets.Add(new GameObject("Benchmark target " + i));
                }
                if (workload == "collection recipe")
                {
                    recipe = ScriptableObject.CreateInstance<TweenRecipe>();
                    JsonUtility.FromJsonOverwrite("{\"bindings\":[{\"id\":\"items\",\"displayName\":\"Items\",\"kind\":1}],\"nodes\":[{\"id\":\"enter\",\"operation\":22,\"bindingId\":\"items\",\"duration\":2,\"parameters\":{\"stringValue\":\"PopIn\",\"floatValue\":0}}]}", recipe);
                }
                var measurement = new Measurement { workload = workload, size = size };
                var clock = new Stopwatch();
                for (int pass = 0; pass < 2; pass++)
                {
                    if (!text) foreach (GameObject target in targets) target.transform.localPosition = Vector3.zero;
                    long allocated = GC.GetAllocatedBytesForCurrentThread();
                    clock.Restart();
                    if (text) tweens.Add(targets[0].Tween().TextWave(duration: 2f).Build().Tween);
                    else if (recipe != null) tweens.Add(TweenRecipeExecutor.Build(recipe, new[] { new TweenPlayerBinding("items", targets) }, targets[0], updateType: UpdateType.Manual).Tween);
                    else foreach (GameObject target in targets) tweens.Add(target.Tween().MoveX(2f, 2f).Build().Tween);
                    foreach (Tween tween in tweens) tween.SetUpdate(UpdateType.Manual).SetAutoKill(false).Play();
                    measurement.constructionMs = clock.Elapsed.TotalMilliseconds;
                    measurement.constructionBytes = report.allocationCounterSupported ? GC.GetAllocatedBytesForCurrentThread() - allocated : -1;

                    var samples = new double[120];
                    allocated = GC.GetAllocatedBytesForCurrentThread();
                    for (int frame = 0; frame < samples.Length; frame++)
                    {
                        clock.Restart();
                        DOTween.ManualUpdate(1f / 60f, 1f / 60f);
                        samples[frame] = clock.Elapsed.TotalMilliseconds;
                    }
                    measurement.stepBytes = report.allocationCounterSupported ? GC.GetAllocatedBytesForCurrentThread() - allocated : -1;
                    Array.Sort(samples);
                    measurement.medianStepMs = samples[60];
                    measurement.p95StepMs = samples[114];
                    clock.Restart();
                    foreach (Tween tween in tweens) tween.Kill();
                    tweens.Clear();
                    measurement.cleanupMs = clock.Elapsed.TotalMilliseconds;
                    measurement.remainingTweens = DOTween.TotalActiveTweens() - baseline;
                    Require(measurement.remainingTweens == 0, "Benchmark leaked active tweens.");
                }
                report.measurements.Add(measurement);
            }
            finally
            {
                foreach (Tween tween in tweens) if (tween.IsActive()) tween.Kill();
                foreach (GameObject target in targets) Destroy(target);
                if (recipe != null) Destroy(recipe);
            }
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }
    }
}
