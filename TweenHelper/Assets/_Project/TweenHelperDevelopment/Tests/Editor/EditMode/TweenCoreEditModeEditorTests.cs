using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DG.Tweening;
using NUnit.Framework;
using UnityEngine;

namespace LB.TweenHelper.Tests.Editor
{
    public class TweenCoreEditModeEditorTests
    {
        private const string CapturePresetName = "__TweenHelperTestCapture";

        private GameObject _target;

        [SetUp]
        public void SetUp()
        {
            DOTween.Init();
            _target = new GameObject("TweenHelperTestTarget");
        }

        [TearDown]
        public void TearDown()
        {
            TweenPresetRegistry.UnregisterPreset(CapturePresetName);
            DOTween.KillAll();
            if (_target != null) UnityEngine.Object.DestroyImmediate(_target);
        }

        [Test]
        public void Registry_HasUniqueConstructiblePresetNames()
        {
            var presetTypes = typeof(ITweenPreset).Assembly.GetTypes()
                .Where(type => !type.IsAbstract &&
                               typeof(ITweenPreset).IsAssignableFrom(type) &&
                               Attribute.IsDefined(type, typeof(AutoRegisterPresetAttribute)))
                .ToList();

            var presets = presetTypes.Select(type => (ITweenPreset)Activator.CreateInstance(type)).ToList();
            var duplicateNames = presets.GroupBy(preset => preset.PresetName, StringComparer.Ordinal)
                .Where(group => string.IsNullOrWhiteSpace(group.Key) || group.Count() > 1)
                .Select(group => group.Key)
                .ToList();

            Assert.That(presetTypes, Has.Count.EqualTo(300));
            Assert.That(duplicateNames, Is.Empty);

            TweenPresetRegistry.Refresh();
            Assert.That(TweenPresetRegistry.Count, Is.EqualTo(presetTypes.Count));
        }

        [Test]
        public void Registry_ConstructsEveryPresetForCompatibleTarget()
        {
            TweenPresetRegistry.Refresh();

            foreach (ITweenPreset preset in TweenPresetRegistry.Presets)
            {
                var target = new GameObject($"TweenHelperPresetTest_{preset.PresetName}");
                target.AddComponent<CanvasGroup>();

                try
                {
                    Assert.That(preset.CanApplyTo(target), Is.True, $"{preset.PresetName} rejected a compatible target.");
                    Tween tween = null;
                    Assert.DoesNotThrow(() => tween = preset.CreateTween(target, 0.05f), preset.PresetName);
                    Assert.That(tween, Is.Not.Null, $"{preset.PresetName} returned no tween.");
                    tween.Kill(false);
                }
                finally
                {
                    DOTween.Kill(target);
                    UnityEngine.Object.DestroyImmediate(target);
                }
            }
        }

        [Test]
        public void Registry_HandlesUnknownPresetAndIncompatibleFadeTarget()
        {
            TweenPresetRegistry.Refresh();
            Assert.That(TweenPresetRegistry.GetPreset("__UnknownTweenHelperPreset"), Is.Null);

            ITweenPreset fadeIn = TweenPresetRegistry.GetPreset("FadeIn");
            Assert.That(fadeIn, Is.Not.Null);
            Assert.That(fadeIn.CanApplyTo(_target), Is.False);
        }

        [Test]
        public void Builder_AppliesOptionsDeclaredAfterStep()
        {
            var handle = _target.Tween()
                .Move(Vector3.one)
                .WithOptions(TweenOptions.WithDuration(0.4f))
                .WithEase(Ease.InQuad)
                .Build();

            Assert.That(handle.Duration, Is.EqualTo(0.4f).Within(0.001f));
            handle.Tween.Goto(0.2f, false);
            Assert.That(_target.transform.position.x, Is.EqualTo(0.25f).Within(0.01f));
        }

        [Test]
        public void Builder_AppliesPendingOptionsToNextStep()
        {
            var handle = _target.Tween()
                .Move(Vector3.one, 0.2f)
                .Then()
                .WithOptions(TweenOptions.WithDuration(0.4f))
                .Scale(2f)
                .Build();

            Assert.That(handle.Duration, Is.EqualTo(0.6f).Within(0.001f));
        }

        [Test]
        public void Builder_ExplicitDurationOverridesOptionDuration()
        {
            var handle = _target.Tween()
                .WithOptions(TweenOptions.WithDuration(0.8f))
                .Move(Vector3.one, 0.25f)
                .Build();

            Assert.That(handle.Duration, Is.EqualTo(0.25f).Within(0.001f));
        }

        [Test]
        public void Builder_ThenAndWithProduceExpectedDurations()
        {
            var sequential = _target.Tween()
                .Move(Vector3.one, 0.2f)
                .Then()
                .Scale(2f, 0.3f)
                .Build();

            Assert.That(sequential.Duration, Is.EqualTo(0.5f).Within(0.001f));
            sequential.Kill();

            var parallel = _target.Tween()
                .Move(Vector3.one, 0.2f)
                .With()
                .Scale(2f, 0.3f)
                .Build();

            Assert.That(parallel.Duration, Is.EqualTo(0.3f).Within(0.001f));
        }

        [Test]
        public void Builder_PresetReceivesOptionsCapturedForItsStep()
        {
            var preset = new CapturingPreset();
            TweenPresetRegistry.RegisterPreset(preset);

            var handle = _target.Tween()
                .Preset(CapturePresetName)
                .WithOptions(TweenOptions.WithDuration(0.45f))
                .Build();

            Assert.That(preset.CapturedDuration, Is.EqualTo(0.45f));
            Assert.That(handle.Duration, Is.EqualTo(0.45f).Within(0.001f));
        }

        [Test]
        public void Handle_CallbacksAreAdditive()
        {
            int originalCalls = 0;
            int handleCalls = 0;
            var tween = CreatePausedTween();
            tween.onComplete += () => originalCalls++;

            var handle = new TweenHandle(tween).OnComplete(() => handleCalls++);
            handle.Complete();

            Assert.That(originalCalls, Is.EqualTo(1));
            Assert.That(handleCalls, Is.EqualTo(1));
        }

        [Test]
        public void Builder_CallbackRegistrationsAreAdditive()
        {
            int firstCompletionCalls = 0;
            int secondCompletionCalls = 0;
            int firstKillCalls = 0;
            int secondKillCalls = 0;

            var handle = _target.Tween()
                .MoveX(1f, 0.1f)
                .OnComplete(() => firstCompletionCalls++)
                .OnComplete(() => secondCompletionCalls++)
                .OnKill(() => firstKillCalls++)
                .OnKill(() => secondKillCalls++)
                .Build();

            handle.Complete();
            handle.Kill();

            Assert.That(firstCompletionCalls, Is.EqualTo(1));
            Assert.That(secondCompletionCalls, Is.EqualTo(1));
            Assert.That(firstKillCalls, Is.EqualTo(1));
            Assert.That(secondKillCalls, Is.EqualTo(1));
        }

        [Test]
        [Timeout(5000)]
        public async Task AwaitCompletion_PreservesExistingCallbacks()
        {
            int originalCalls = 0;
            var tween = CreatePausedTween();
            tween.onComplete += () => originalCalls++;

            var task = TweenAsync.AwaitCompletion(tween);
            tween.Complete();
            await task;

            Assert.That(originalCalls, Is.EqualTo(1));
        }

        [Test]
        [Timeout(5000)]
        public async Task AwaitCompletion_CancellationCancelsTask()
        {
            var tween = CreatePausedTween();
            using (var cancellation = new CancellationTokenSource())
            {
                var task = TweenAsync.AwaitCompletion(tween, cancellation.Token);
                cancellation.Cancel();

                try
                {
                    await task;
                    Assert.Fail("Cancellation should cancel the await.");
                }
                catch (OperationCanceledException)
                {
                }

                Assert.That(task.IsCanceled, Is.True);
            }
        }

        [Test]
        [Timeout(5000)]
        public async Task AwaitWithTimeout_ReturnsFalseWhenTweenIsKilledExternally()
        {
            var tween = CreatePausedTween();
            var task = TweenAsync.AwaitCompletionWithTimeout(tween, 1f);
            tween.Kill();

            Assert.That(await task, Is.False);
        }

        private Tween CreatePausedTween()
        {
            var tween = _target.transform.DOMoveX(1f, 1f).SetAutoKill(false).SetUpdate(UpdateType.Manual);
            tween.Play();
            DOTween.ManualUpdate(0.1f, 0.1f);
            return tween.Pause();
        }

        private sealed class CapturingPreset : ITweenPreset
        {
            public string PresetName => CapturePresetName;
            public string Description => "Captures builder options for tests.";
            public float DefaultDuration => 1f;
            public float? CapturedDuration { get; private set; }

            public bool CanApplyTo(GameObject target) => target != null;

            public Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
            {
                CapturedDuration = duration ?? options.Duration;
                return target.transform.DOMoveX(1f, CapturedDuration ?? 1f);
            }
        }
    }
}
