using System.Threading;
using DG.Tweening;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Reflection;
using LB.TweenHelper.Editor;
using Object = UnityEngine.Object;

namespace LB.TweenHelper.Tests.Editor
{
    public sealed class PublishingReadinessEditorTests
    {
        private GameObject _target;

        [SetUp]
        public void SetUp()
        {
            DOTween.Init();
            _target = new GameObject("PublishingReadinessTarget");
        }

        [TearDown]
        public void TearDown()
        {
            TweenMotion.Preference = TweenMotionPreference.UseProjectDefault;
            DOTween.Kill(_target.transform);
            DOTween.Kill(_target);
            Object.DestroyImmediate(_target);
        }

        [Test]
        public void RetainedCompletedTween_AllAwaitEntriesSettleImmediately()
        {
            Tween tween = _target.transform.DOMoveX(2f, 1f).SetAutoKill(false);
            tween.Complete();
            using var source = new CancellationTokenSource();
            source.Cancel();
            Assert.That(TweenAsync.AwaitCompletion(tween, source.Token).IsCompleted, Is.True);
            Assert.That(TweenAsync.GetAwaiter(tween).GetResult(), Is.True);
            Assert.That(TweenAsync.AwaitCompletionWithTimeout(tween, 1f).Result, Is.True);
            Assert.That(tween.IsActive(), Is.True);
        }

        [Test]
        public void EditorHandleKill_CleansOnlyOwnedTweenAndPreservesCallbackIdentity()
        {
            var sharedId = new object();
            Tween owned = _target.transform.DOMoveX(1f, 1f).SetId(sharedId);
            Tween unrelated = _target.transform.DOMoveY(2f, 1f).SetId(sharedId);
            object callbackId = null;
            owned.onKill += () => callbackId = owned.id;
            new TweenHandle(owned).Kill();
            Assert.That(owned.IsActive(), Is.False);
            Assert.That(unrelated.IsActive(), Is.True);
            Assert.That(callbackId, Is.SameAs(sharedId));
            new TweenHandle(unrelated).Kill();
        }

        [Test]
        public void EditorCancellation_KillsWithoutWaitingForPlayMode()
        {
            Tween tween = _target.transform.DOMoveX(1f, 1f);
            using var source = new CancellationTokenSource();
            var wait = TweenAsync.AwaitCompletion(tween, source.Token);
            source.Cancel();
            Assert.That(tween.IsActive(), Is.False);
        }

        [TestCase("ProgressReward")]
        [TestCase("RewardPresentation")]
        [TestCase("CollectionEntrance")]
        public void WorkflowSample_HasValidStableData(string name)
        {
            var recipe = AssetDatabase.LoadAssetAtPath<TweenRecipe>("Assets/Loags/TweenHelper/Samples/TweenHelper Demos/Recipes/" + name + ".asset");
            Assert.That(recipe, Is.Not.Null);
            TweenRecipeValidationResult validation = TweenRecipeValidator.Validate(recipe);
            Assert.That(validation.IsValid, Is.True, validation.GetSummary());
        }

        [Test]
        public void BrowserPreferences_PersistFavoritesAndBoundRecentEntries()
        {
            string key = $"LB.TweenHelper.Browser.{Hash128.Compute(Application.dataPath)}";
            bool existed = EditorPrefs.HasKey(key);
            string original = EditorPrefs.GetString(key);
            Type type = typeof(PresetBrowserWindow).Assembly.GetType("LB.TweenHelper.Editor.PresetBrowserPreferences");
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
            object Call(object instance, string method, params object[] args) => type.GetMethod(method, flags).Invoke(instance, args);
            try
            {
                EditorPrefs.DeleteKey(key);
                object preferences = Call(null, "Load");
                Call(preferences, "ToggleFavorite", "stable-id");
                for (int i = 0; i < 25; i++) Call(preferences, "Visit", "entry-" + i);
                preferences = Call(null, "Load");
                Assert.That(Call(preferences, "IsFavorite", "stable-id"), Is.EqualTo(true));
                Assert.That(Call(preferences, "RecentIndex", "entry-0"), Is.EqualTo(-1));
                Assert.That(Call(preferences, "RecentIndex", "entry-24"), Is.EqualTo(0));
                Assert.That(Call(preferences, "RecentIndex", "entry-5"), Is.EqualTo(19));
                Call(preferences, "ToggleFavorite", "stable-id");
                Assert.That(Call(Call(null, "Load"), "IsFavorite", "stable-id"), Is.EqualTo(false));

                Type catalog = type.Assembly.GetType("LB.TweenHelper.Editor.PresetBrowserCatalog");
                var entries = (IEnumerable)catalog.GetMethod("Build", flags).Invoke(null, null);
                foreach (string useCase in new[] { "Menus", "Inventory", "Rewards", "Text", "Progress", "Camera" })
                {
                    int count = 0;
                    foreach (object entry in entries) if ((bool)Call(null, "MatchesUseCase", entry, useCase)) count++;
                    Assert.That(count, Is.GreaterThan(0), useCase);
                }
            }
            finally
            {
                if (existed) EditorPrefs.SetString(key, original);
                else EditorPrefs.DeleteKey(key);
            }
        }

        [Test]
        public void AuthoredSample_HasValidBindingsAndPersistentMotionEvent()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Loags/TweenHelper/Samples/TweenHelper Demos/Prefabs/UI/ProgressRewardDemo.prefab");
            TweenPlayer player = prefab.GetComponent<TweenPlayer>();
            Assert.That(player.Validate().IsValid, Is.True);
            var toggle = prefab.GetComponentInChildren<UnityEngine.UI.Toggle>();
            Assert.That(toggle.onValueChanged.GetPersistentEventCount(), Is.EqualTo(1));
            Assert.That(toggle.onValueChanged.GetPersistentTarget(0), Is.EqualTo(player));
            Assert.That(toggle.onValueChanged.GetPersistentMethodName(0), Is.EqualTo(nameof(TweenPlayer.SetReducedMotion)));
            Assert.That(player.Bindings[0].Target.GetComponent<UnityEngine.UI.Image>().sprite, Is.Not.Null);
        }

        [Test]
        public void Bootstrapper_PreservesHostDefaultsAndRawTween()
        {
            AutoPlay autoplay = DOTween.defaultAutoPlay;
            bool autokill = DOTween.defaultAutoKill;
            Ease ease = DOTween.defaultEaseType;
            try
            {
                DOTween.defaultAutoPlay = AutoPlay.None;
                DOTween.defaultAutoKill = false;
                DOTween.defaultEaseType = Ease.Linear;
                Tween raw = _target.transform.DOMoveX(2f, 1f);
                Assert.That(TweenHelperSettings.Instance.ConfigureDotweenEngine, Is.False);
                TweenHelperBootstrapper.Reinitialize();
                Assert.That(DOTween.defaultAutoPlay, Is.EqualTo(AutoPlay.None));
                Assert.That(DOTween.defaultAutoKill, Is.False);
                Assert.That(DOTween.defaultEaseType, Is.EqualTo(Ease.Linear));
                Assert.That(raw.IsActive(), Is.True);
                Assert.That(raw.IsPlaying(), Is.False);
                TweenHandle built = _target.Tween().MoveX(3f, 1f).Build();
                Assert.That(built.IsPlaying, Is.False);
                built.Resume();
                Assert.That(built.IsPlaying, Is.True);
                built.Kill();
            }
            finally
            {
                DOTween.defaultAutoPlay = autoplay;
                DOTween.defaultAutoKill = autokill;
                DOTween.defaultEaseType = ease;
            }
        }

    }
}
