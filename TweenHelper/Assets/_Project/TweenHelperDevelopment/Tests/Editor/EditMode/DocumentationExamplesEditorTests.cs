using System.IO;
using DG.Tweening;
using LB.TweenHelper.Editor;
using NUnit.Framework;
using UnityEngine;

namespace LB.TweenHelper.Tests.Editor
{
    public class DocumentationExamplesEditorTests
    {
        [Test]
        public void ApiExamples_BuildAgainstCurrentFluentSurface()
        {
            var target = new GameObject("TweenHelperDocumentationTarget");
            target.AddComponent<CanvasGroup>();

            try
            {
                TweenOptions options = TweenOptions.WithDuration(0.5f)
                    .SetEase(Ease.OutBack)
                    .SetDelay(0.1f)
                    .SetLoops(2, LoopType.Yoyo)
                    .SetStrength(1.25f)
                    .SetStartScale(Vector3.zero)
                    .SetTargetScale(Vector3.one);

                Tween rawTween = target.transform.DOPunchRotation(Vector3.forward * 8f, 0.2f).Pause();
                TweenHandle handle = target.Tween()
                    .MoveLocal(Vector3.up, 0.25f)
                    .With()
                    .FadeIn(0.25f)
                    .Then()
                    .Delay(0.1f)
                    .Then(rawTween)
                    .Call(() => { })
                    .Then()
                    .WithOptions(options)
                    .Preset<PopInPreset>()
                    .Build();

                Assert.That(handle.Tween, Is.Not.Null);
                handle.Kill();
            }
            finally
            {
                DOTween.Kill(target);
                Object.DestroyImmediate(target);
            }
        }

        [Test]
        public void PresetCatalog_MatchesDeterministicGenerator()
        {
            Assert.That(PresetDocumentationExporter.TryBuildCatalog(out string generated, out string error), Is.True, error);

            string path = Path.Combine(Application.dataPath, "Loags", "TweenHelper", "Documentation", "PresetCatalog.md");
            string current = File.ReadAllText(path);

            Assert.That(current.Replace("\r\n", "\n"), Is.EqualTo(generated.Replace("\r\n", "\n")));
        }
    }
}
