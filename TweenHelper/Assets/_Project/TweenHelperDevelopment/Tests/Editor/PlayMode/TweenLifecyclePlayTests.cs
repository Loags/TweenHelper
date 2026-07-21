using System.Collections;
using System.Threading;
using DG.Tweening;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LB.TweenHelper.Tests.PlayMode
{
    public class TweenLifecyclePlayTests
    {
        [TearDown]
        public void TearDown() => DOTween.KillAll();

        [UnityTest]
        [Timeout(5000)]
        public IEnumerator AwaitCompletion_CancellationKillsRunningTween()
        {
            var target = new GameObject("TweenHelperCancellationTarget");
            var tween = target.transform.DOMoveX(1f, 2f).SetAutoKill(false);
            int killCalls = 0;
            tween.onKill += () => killCalls++;

            using (var cancellation = new CancellationTokenSource())
            {
                var task = TweenAsync.AwaitCompletion(tween, cancellation.Token);
                yield return null;

                cancellation.Cancel();
                yield return null;

                Assert.That(task.IsCanceled, Is.True);
                Assert.That(killCalls, Is.EqualTo(1));
                Assert.That(tween.IsActive(), Is.False);
            }

            Object.Destroy(target);
        }

        [UnityTest]
        [Timeout(5000)]
        public IEnumerator FiniteYoyo_CompletesAndCleansUp()
        {
            var target = new GameObject("TweenHelperYoyoTarget");
            int completionCalls = 0;
            var tween = target.transform.DOMoveX(1f, 0.02f)
                .SetLoops(2, LoopType.Yoyo)
                .OnComplete(() => completionCalls++);

            yield return new WaitForSeconds(0.08f);

            Assert.That(completionCalls, Is.EqualTo(1));
            Assert.That(tween.IsActive(), Is.False);
            Assert.That(target.transform.position.x, Is.EqualTo(0f).Within(0.01f));
            Object.Destroy(target);
        }

        [UnityTest]
        [Timeout(5000)]
        public IEnumerator DestroyedTarget_KillsLinkedBuilderTween()
        {
            var target = new GameObject("TweenHelperLinkedTarget");
            var handle = target.Tween().MoveX(1f, 1f).Play();
            yield return null;

            Object.Destroy(target);
            yield return null;

            Assert.That(handle.IsActive, Is.False);
        }

        [UnityTest]
        [Timeout(5000)]
        public IEnumerator OneShotPreset_CompletesAtExpectedState()
        {
            var target = new GameObject("TweenHelperOneShotTarget");
            Vector3 originalScale = target.transform.localScale;
            ITweenPreset preset = TweenPresetRegistry.GetPreset("PopIn");

            Assert.That(preset, Is.Not.Null);
            Tween tween = preset.CreateTween(target, 0.03f);
            Assert.That(tween, Is.Not.Null);

            yield return new WaitForSeconds(0.08f);

            Assert.That(tween.IsActive(), Is.False);
            Assert.That(Vector3.Distance(target.transform.localScale, originalScale), Is.LessThan(0.01f));
            Object.Destroy(target);
        }

        [UnityTest]
        [Timeout(5000)]
        public IEnumerator Handle_RewindRestoresStartingState()
        {
            var target = new GameObject("TweenHelperRewindTarget");
            Vector3 startingPosition = target.transform.position;
            TweenHandle handle = target.Tween().MoveX(2f, 0.2f).Play();

            yield return new WaitForSeconds(0.08f);
            Assert.That(Vector3.Distance(target.transform.position, startingPosition), Is.GreaterThan(0.01f));

            handle.Rewind();
            yield return null;

            Assert.That(Vector3.Distance(target.transform.position, startingPosition), Is.LessThan(0.001f));
            handle.Kill(false);
            Object.Destroy(target);
        }

        [UnityTest]
        [Timeout(5000)]
        public IEnumerator InfinitePresets_KillStopsEntireLoop()
        {
            string[] presetNames = { "Sway", "PendulumZ", "Breathe", "Heartbeat", "Float", "Blink" };

            foreach (string presetName in presetNames)
            {
                var target = new GameObject($"TweenHelper{presetName}Target");
                CanvasGroup canvasGroup = presetName == "Blink" ? target.AddComponent<CanvasGroup>() : null;
                ITweenPreset preset = TweenPresetRegistry.GetPreset(presetName);

                Assert.That(preset, Is.Not.Null, presetName);
                Tween tween = preset.CreateTween(target, 0.04f);
                Assert.That(tween, Is.Not.Null, presetName);

                yield return new WaitForSeconds(0.08f);
                tween.Kill(false);
                Vector3 positionAfterKill = target.transform.localPosition;
                Vector3 scaleAfterKill = target.transform.localScale;
                Quaternion rotationAfterKill = target.transform.localRotation;
                float alphaAfterKill = canvasGroup != null ? canvasGroup.alpha : 1f;
                yield return new WaitForSeconds(0.1f);

                Assert.That(tween.IsActive(), Is.False, presetName);
                Assert.That(Vector3.Distance(target.transform.localPosition, positionAfterKill), Is.LessThan(0.001f), presetName);
                Assert.That(Vector3.Distance(target.transform.localScale, scaleAfterKill), Is.LessThan(0.001f), presetName);
                Assert.That(Quaternion.Angle(target.transform.localRotation, rotationAfterKill), Is.LessThan(0.01f), presetName);
                if (canvasGroup != null) Assert.That(Mathf.Abs(canvasGroup.alpha - alphaAfterKill), Is.LessThan(0.001f), presetName);

                Object.Destroy(target);
                yield return null;
            }
        }

        [UnityTest]
        [Timeout(5000)]
        public IEnumerator AwaitCompletion_MultipleAwaitersCompleteTogether()
        {
            var target = new GameObject("TweenHelperMultipleAwaitTarget");
            Tween tween = target.transform.DOMoveX(1f, 0.03f).SetAutoKill(false);
            var first = TweenAsync.AwaitCompletion(tween);
            var second = TweenAsync.AwaitCompletion(tween);

            yield return new WaitForSeconds(0.08f);

            Assert.That(first.IsCompleted && !first.IsCanceled && !first.IsFaulted, Is.True);
            Assert.That(second.IsCompleted && !second.IsCanceled && !second.IsFaulted, Is.True);
            tween.Kill(false);
            Object.Destroy(target);
        }

        [UnityTest]
        [Timeout(5000)]
        public IEnumerator AwaitWithTimeout_KillsTweenOnce()
        {
            var target = new GameObject("TweenHelperTimeoutTarget");
            Tween tween = target.transform.DOMoveX(1f, 2f).SetAutoKill(false);
            int killCalls = 0;
            tween.onKill += () => killCalls++;
            var task = TweenAsync.AwaitCompletionWithTimeout(tween, 0.02f);

            yield return new WaitUntil(() => task.IsCompleted);

            Assert.That(task.IsCanceled, Is.False);
            Assert.That(task.IsFaulted, Is.False);
            Assert.That(task.Result, Is.False);
            Assert.That(killCalls, Is.EqualTo(1));
            Assert.That(tween.IsActive(), Is.False);
            Object.Destroy(target);
        }
    }
}
