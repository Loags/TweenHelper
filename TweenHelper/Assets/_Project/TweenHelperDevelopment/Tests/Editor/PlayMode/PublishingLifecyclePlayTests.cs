using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using DG.Tweening;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace LB.TweenHelper.Tests.PlayMode
{
    public sealed class PublishingLifecyclePlayTests
    {
        private GameObject _target;
        private Tween _tween;

        [SetUp]
        public void SetUp()
        {
            _target = new GameObject("PublishingLifecycleTarget");
            _tween = _target.transform.DOMoveX(5f, 10f).SetAutoKill(false);
        }

        [TearDown]
        public void TearDown()
        {
            Time.timeScale = 1f;
            TweenMotion.Preference = TweenMotionPreference.UseProjectDefault;
            DOTween.Kill(_target);
            DOTween.Kill(_target.transform);
            if (_tween != null && _tween.IsActive()) _tween.Kill();
            Object.Destroy(_target);
        }

        [UnityTest, Timeout(5000)]
        public IEnumerator WorkerCancellation_MutatesTweenOnUnityThread()
        {
            int mainThread = Thread.CurrentThread.ManagedThreadId;
            int killThread = 0;
            _tween.onKill += () => killThread = Thread.CurrentThread.ManagedThreadId;
            using var source = new CancellationTokenSource();
            Task wait = TweenAsync.AwaitCompletion(_tween, source.Token);
            Task worker = Task.Run(() => source.Cancel());
            yield return new WaitUntil(() => worker.IsCompleted && wait.IsCompleted);
            Assert.That(worker.IsFaulted, Is.False);
            Assert.That(wait.IsCanceled, Is.True);
            Assert.That(killThread, Is.EqualTo(mainThread));
            Assert.That(_tween.onComplete, Is.Null);
        }

        [TestCase(1, 1f)]
        [TestCase(3, 0.25f)]
        public void ReducedSpin_PreservesFinalOrientation(int loops, float strength)
        {
            _target.transform.localRotation = Quaternion.Euler(15f, 20f, 30f);
            Quaternion initial = _target.transform.localRotation;
            Tween full = TweenPresetRegistry.GetPresetByName("SpinY").CreateTween(_target, 1f, TweenOptions.WithMotionPreference(TweenMotionPreference.Full).SetStrength(strength).SetLoops(loops, LoopType.Incremental));
            full.Complete();
            Quaternion expected = _target.transform.localRotation;
            _target.transform.localRotation = initial;
            Tween tween = TweenPresetRegistry.GetPresetByName("SpinY").CreateTween(_target, 1f, TweenOptions.WithMotionPreference(TweenMotionPreference.Reduced).SetStrength(strength).SetLoops(loops, LoopType.Incremental));
            tween.Complete();
            Assert.That(Quaternion.Angle(expected, _target.transform.localRotation), Is.LessThan(0.01f));
        }

        [Test]
        public void ProgressRecipe_ValidatesValueAndComponentAndRestoresOnRewind()
        {
            var target = new GameObject("Progress", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            var recipe = ScriptableObject.CreateInstance<TweenRecipe>();
            TweenHandle handle = null;
            try
            {
                JsonUtility.FromJsonOverwrite("{\"bindings\":[{\"id\":\"progress\",\"displayName\":\"Progress\",\"kind\":0}],\"nodes\":[{\"id\":\"fill\",\"bindingId\":\"progress\",\"operation\":27,\"duration\":1,\"parameters\":{\"floatValue\":0.8}}]}", recipe);
                var bindings = new[] { new TweenPlayerBinding("progress", target) };
                Image image = target.GetComponent<Image>();
                image.fillAmount = 0.2f;
                Assert.That(TweenRecipeValidator.Validate(recipe, bindings).IsValid, Is.False);
                image.type = Image.Type.Filled;
                handle = TweenRecipeExecutor.Build(recipe, bindings, target, motionPreference: TweenMotionPreference.Reduced);
                handle.Complete();
                Assert.That(image.fillAmount, Is.EqualTo(0.8f).Within(0.001f));
                handle.Rewind();
                Assert.That(image.fillAmount, Is.EqualTo(0.2f).Within(0.001f));
                JsonUtility.FromJsonOverwrite("{\"floatValue\":1.1}", recipe.Nodes[0].Parameters);
                Assert.That(TweenRecipeValidator.Validate(recipe, bindings).IsValid, Is.False);
            }
            finally
            {
                handle?.Kill();
                Object.Destroy(target);
                Object.Destroy(recipe);
            }
        }

        [UnityTest, Timeout(5000)]
        public IEnumerator AwaitAny_DetachesLoserWithoutKillingIt()
        {
            Tween winner = DOVirtual.DelayedCall(0.01f, () => { });
            Task wait = TweenAsync.AwaitAny(new[] { winner, _tween });
            yield return new WaitUntil(() => wait.IsCompleted);
            Assert.That(wait.IsFaulted || wait.IsCanceled, Is.False);
            Assert.That(_tween.IsActive(), Is.True);
            Assert.That(_tween.onComplete, Is.Null);
            Assert.That(_tween.onKill, Is.Null);
        }

        [UnityTest, Timeout(5000)]
        public IEnumerator AwaitAny_PropagatesCanceledWinner()
        {
            using var source = new CancellationTokenSource();
            Task wait = TweenAsync.AwaitAny(new[] { _tween }, source.Token);
            source.Cancel();
            yield return new WaitUntil(() => wait.IsCompleted);
            Assert.That(wait.IsCanceled, Is.True);
        }

        [UnityTest, Timeout(5000)]
        public IEnumerator Timeout_StillSettlesAtZeroTimeScale()
        {
            Time.timeScale = 0f;
            Task<bool> wait = TweenAsync.AwaitCompletionWithTimeout(_tween, 0.02f);
            yield return new WaitUntil(() => wait.IsCompleted);
            Assert.That(wait.Result, Is.False);
            Assert.That(_tween.IsActive(), Is.False);
        }

        [UnityTest, Timeout(5000)]
        public IEnumerator CompletionWinsBeforeCancellation_AndCallbacksStayAdditive()
        {
            using var source = new CancellationTokenSource();
            int calls = 0;
            _tween.onComplete += () => calls++;
            Task first = TweenAsync.AwaitCompletion(_tween, source.Token);
            Task second = TweenAsync.AwaitCompletion(_tween, source.Token);
            _tween.Complete();
            source.Cancel();
            yield return new WaitUntil(() => first.IsCompleted && second.IsCompleted);
            Assert.That(first.IsCanceled || second.IsCanceled, Is.False);
            Assert.That(calls, Is.EqualTo(1));
            Assert.That(_tween.IsActive(), Is.True);
            Assert.That(_tween.onComplete.GetInvocationList().Length, Is.EqualTo(1));
        }
        [Test]
        public void AlreadyCanceledToken_KillsActiveTweenOnce()
        {
            Tween tween = _target.transform.DOMoveX(2f, 1f).SetAutoKill(false);
            int kills = 0;
            tween.onKill += () => kills++;
            using var source = new CancellationTokenSource();
            source.Cancel();
            Assert.That(TweenAsync.AwaitCompletion(tween, source.Token).IsCanceled, Is.True);
            Assert.That(kills, Is.EqualTo(1));
        }

        [Test]
        public void ReducedMotion_ScalesFeedbackAndKeepsSemanticDestination()
        {
            Tween full = _target.Tween().ErrorReject(1f).WithMotionPreference(TweenMotionPreference.Full).Build().Tween;
            full.Goto(0.18f);
            Vector3 fullOffset = _target.transform.localPosition;
            full.Kill();
            Tween reduced = _target.Tween().ErrorReject(1f).WithMotionPreference(TweenMotionPreference.Reduced).Build().Tween;
            reduced.Goto(0.18f);
            Assert.That(Vector3.Distance(_target.transform.localPosition, fullOffset * 0.2f), Is.LessThan(0.001f));
            reduced.Kill();
            TweenMotion.Preference = TweenMotionPreference.Reduced;
            Tween destination = _target.Tween().MoveX(12f, 1f).Build().Tween;
            destination.Complete();
            Assert.That(_target.transform.position.x, Is.EqualTo(12f).Within(0.001f));
            destination.Kill();
        }

        [Test]
        public void ActiveMotion_DoesNotChangeWhenPreferenceChanges()
        {
            TweenMotion.Preference = TweenMotionPreference.Full;
            Tween tween = _target.Tween().ErrorReject(1f).Build().Tween;
            tween.Goto(0.18f);
            Vector3 before = _target.transform.localPosition;
            TweenMotion.Preference = TweenMotionPreference.Reduced;
            tween.Goto(0.18f);
            Assert.That(Vector3.Distance(before, _target.transform.localPosition), Is.LessThan(0.001f));
            tween.Kill();
        }
    }
}
