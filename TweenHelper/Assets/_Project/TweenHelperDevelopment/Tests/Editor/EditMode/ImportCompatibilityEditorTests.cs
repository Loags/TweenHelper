using System;
using System.Reflection;
using DG.Tweening;
using LB.TweenHelper.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace LB.TweenHelper.Tests.Editor
{
    public sealed class ImportCompatibilityEditorTests
    {
        private static readonly Type Utility = typeof(TweenPlayer).Assembly.GetType("LB.TweenHelper.TweenTargetUtility");

        private static Tween Create(string method, params object[] arguments) => (Tween)Utility.GetMethod(method, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).Invoke(null, arguments);

        [TestCase(true)]
        [TestCase(false)]
        public void AnchoredAxisTween_PreservesOtherAxisAndOwnership(bool xAxis)
        {
            var target = new GameObject("Anchor regression", typeof(RectTransform));
            var rect = (RectTransform)target.transform;
            rect.anchoredPosition = new Vector2(11f, 19f);
            try
            {
                Tween tween = Create(xAxis ? "CreateAnchoredPositionXTween" : "CreateAnchoredPositionYTween", rect, 3.6f, 1f, true).Pause().SetAutoKill(false);
                tween.Complete();
                Assert.That(rect.anchoredPosition, Is.EqualTo(xAxis ? new Vector2(4f, 19f) : new Vector2(11f, 4f)));
                Assert.That(tween.target, Is.SameAs(rect));
            }
            finally
            {
                DOTween.Kill(rect);
                Object.DestroyImmediate(target);
            }
        }

        [Test]
        public void AnchoredTween_PreservesRelativeMovement()
        {
            var target = new GameObject("Relative anchor regression", typeof(RectTransform));
            var rect = (RectTransform)target.transform;
            rect.anchoredPosition = new Vector2(11f, 19f);
            try
            {
                Tween tween = Create("CreateAnchoredPositionTween", rect, new Vector2(3f, 5f), 1f, false).SetRelative().Pause().SetAutoKill(false);
                tween.Complete();
                Assert.That(rect.anchoredPosition, Is.EqualTo(new Vector2(14f, 24f)));
            }
            finally
            {
                DOTween.Kill(rect);
                Object.DestroyImmediate(target);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void VisualTweens_PreserveFadeRgbAndApplyFullColor(bool sprite)
        {
            var target = sprite ? new GameObject("Sprite regression", typeof(SpriteRenderer)) : new GameObject("UI regression", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            Color Read() => sprite ? target.GetComponent<SpriteRenderer>().color : target.GetComponent<Image>().color;
            void Write(Color value)
            {
                if (sprite) target.GetComponent<SpriteRenderer>().color = value;
                else target.GetComponent<Image>().color = value;
            }
            Color start = new Color(0.2f, 0.4f, 0.6f, 0.8f);
            Write(start);
            try
            {
                Create("CreateFadeTween", target, 0.25f, 1f).Pause().Complete();
                Assert.That(Read(), Is.EqualTo(new Color(start.r, start.g, start.b, 0.25f)));
                Color end = new Color(0.7f, 0.3f, 0.1f, 0.5f);
                Create("CreateColorTween", target, end, 1f).Pause().Complete();
                Color actual = Read();
                Assert.That(actual.r, Is.EqualTo(end.r).Within(0.000001f));
                Assert.That(actual.g, Is.EqualTo(end.g).Within(0.000001f));
                Assert.That(actual.b, Is.EqualTo(end.b).Within(0.000001f));
                Assert.That(actual.a, Is.EqualTo(end.a).Within(0.000001f));
            }
            finally
            {
                DOTween.Kill(target.GetComponent<Component>());
                Object.DestroyImmediate(target);
            }
        }

        [Test]
        public void RuntimeAssembly_DoesNotReferenceOptionalDotweenModules()
        {
            Assert.That(Array.Exists(typeof(TweenPlayer).Assembly.GetReferencedAssemblies(), assembly => assembly.Name == "DOTween.Modules"), Is.False);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void PlayerPreview_RestoresPropertiesAndPreservesSceneDirtyState(bool originallyDirty)
        {
            string path = "Assets/_Project/TweenHelperDevelopment/Validation/Scenes/PreviewImportRegression.unity";
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            GameObject target = new GameObject("Preview regression");
            SceneManager.MoveGameObjectToScene(target, scene);
            TweenPlayer player = target.AddComponent<TweenPlayer>();
            player.SetPreset("Squash");
            EditorSceneManager.SaveScene(scene, path);
            if (originallyDirty) EditorSceneManager.MarkSceneDirty(scene);
            Vector3 scale = target.transform.localScale;
            Type session = typeof(PresetBrowserWindow).Assembly.GetType("LB.TweenHelper.Editor.TweenRecipePreviewSession");
            object[] arguments = { player, null };
            try
            {
                Assert.That(session.GetMethod("Start").Invoke(null, arguments), Is.EqualTo(true), arguments[1] as string);
                DOTween.ManualUpdate(0.1f, 0.1f);
                session.GetMethod("Stop").Invoke(null, new object[] { "Regression complete" });
                Assert.That(target.transform.localScale, Is.EqualTo(scale));
                Assert.That(scene.isDirty, Is.EqualTo(originallyDirty));
            }
            finally
            {
                session.GetMethod("Stop").Invoke(null, new object[] { "Regression cleanup" });
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                AssetDatabase.DeleteAsset(path);
            }
        }
    }
}
