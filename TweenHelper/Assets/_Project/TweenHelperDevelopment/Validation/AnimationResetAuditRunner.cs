using System.Collections;
using System.Linq;
using UnityEngine;

namespace LB.TweenHelper.Demo
{
    [AddComponentMenu("LB/TweenHelper/Development/Animation Gallery Audit Runner")]
    public sealed class AnimationResetAuditRunner : MonoBehaviour
    {
        [SerializeField] private bool autoRunOnStart;
        [SerializeField, Min(0f)] private float previewDuration = 0.08f;

        private Coroutine _auditCoroutine;

        public bool IsRunning => _auditCoroutine != null;
        public bool LastRunPassed { get; private set; }
        public int AuditedEntryCount { get; private set; }

        private void Start()
        {
            if (autoRunOnStart) RunGalleryResetAudit();
        }

        [ContextMenu("Run Gallery Reset Audit")]
        public void RunGalleryResetAudit()
        {
            if (_auditCoroutine != null) StopCoroutine(_auditCoroutine);
            _auditCoroutine = StartCoroutine(RunAudit());
        }

        private IEnumerator RunAudit()
        {
            LastRunPassed = false;
            AuditedEntryCount = 0;
            AnimationGalleryPlayer player = FindAnyObjectByType<AnimationGalleryPlayer>();
            if (player == null)
            {
                Debug.LogError("Animation Gallery audit requires the gallery scene to be open in Play Mode.", this);
                _auditCoroutine = null;
                yield break;
            }

            AnimationGalleryEntry[] entries = AnimationGalleryCatalog.Build()
                .GroupBy(entry => entry.Category)
                .SelectMany(group => group.Take(1))
                .ToArray();

            foreach (AnimationGalleryEntry entry in entries)
            {
                var configuration = new AnimationGalleryConfiguration(entry);
                player.Play(configuration);
                if (previewDuration > 0f) yield return new WaitForSecondsRealtime(previewDuration);
                player.ResetPreview(configuration);
                yield return null;
                AuditedEntryCount++;
            }

            LastRunPassed = AuditedEntryCount == entries.Length;
            _auditCoroutine = null;
            Debug.Log($"Animation Gallery reset audit passed for {AuditedEntryCount} category representatives.", this);
        }
    }
}
