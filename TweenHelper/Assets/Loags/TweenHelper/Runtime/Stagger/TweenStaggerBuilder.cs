using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Builds one owner-linked sequence that staggers a finite tween across a collection.
    /// </summary>
    public sealed class TweenStaggerBuilder
    {
        private readonly List<GameObject> _targets;
        private readonly GameObject _owner;
        private Func<GameObject, int, Tween> _animationFactory;
        private Func<GameObject, int, float> _customDelay;
        private ITweenPreset _preset;
        private string _animationName;
        private StaggerOrder _order = StaggerOrder.FirstToLast;
        private float _delayBetween = 0.05f;
        private float _rootDelay;
        private float _tailDelay;
        private int _randomSeed;
        private int _loops = 1;
        private LoopType _loopType = LoopType.Restart;
        private UpdateType? _updateType;
        private bool? _unscaledTime;
        private string _id;
        private Action _onPlay;
        private Action _onComplete;
        private Action _onKill;

        /// <summary>
        /// Creates a collection builder and snapshots the supplied targets immediately.
        /// </summary>
        public TweenStaggerBuilder(IEnumerable<GameObject> targets, GameObject owner)
        {
            if (targets == null) throw new ArgumentNullException(nameof(targets));
            _owner = owner != null ? owner : throw new ArgumentNullException(nameof(owner));
            _targets = SnapshotTargets(targets);
        }

        /// <summary>
        /// Uses a registered preset type for every collection item.
        /// </summary>
        public TweenStaggerBuilder Preset<TPreset>(float? duration = null, TweenOptions options = default) where TPreset : class, ITweenPreset
        {
            var preset = TweenPresetRegistry.GetPreset<TPreset>();
            if (preset == null)
            {
                throw new InvalidOperationException($"TweenStaggerBuilder: Preset type '{typeof(TPreset).Name}' is not registered.");
            }

            return Preset(preset, duration, options);
        }

        /// <summary>
        /// Uses an already resolved preset for every collection item.
        /// </summary>
        public TweenStaggerBuilder Preset(ITweenPreset preset, float? duration = null, TweenOptions options = default)
        {
            _preset = preset ?? throw new ArgumentNullException(nameof(preset));
            _animationName = preset.PresetName;
            _animationFactory = (target, _) => preset.CreateTween(target, duration, options);
            return this;
        }

        /// <summary>
        /// Resolves a registered preset by name for dynamic selection.
        /// </summary>
        public TweenStaggerBuilder PresetByName(string presetName, float? duration = null, TweenOptions options = default)
        {
            if (string.IsNullOrWhiteSpace(presetName)) throw new ArgumentException("A preset name is required.", nameof(presetName));
            var preset = TweenPresetRegistry.GetPresetByName(presetName);
            if (preset == null) throw new InvalidOperationException($"TweenStaggerBuilder: Preset '{presetName}' is not registered.");
            return Preset(preset, duration, options);
        }

        /// <summary>
        /// Uses a custom finite DOTween factory. The index is the original collection index.
        /// </summary>
        public TweenStaggerBuilder Animate(Func<GameObject, int, Tween> animationFactory)
        {
            _animationFactory = animationFactory ?? throw new ArgumentNullException(nameof(animationFactory));
            _preset = null;
            _animationName = "custom animation";
            return this;
        }

        /// <summary>
        /// Sets the time between ordered collection starts.
        /// </summary>
        public TweenStaggerBuilder DelayBetween(float seconds)
        {
            StaggerDelayUtility.ValidateDelay(seconds, nameof(seconds));
            _delayBetween = seconds;
            _customDelay = null;
            return this;
        }

        /// <summary>
        /// Selects a built-in collection order. This replaces a previously configured custom delay.
        /// </summary>
        public TweenStaggerBuilder Order(StaggerOrder order)
        {
            if (!Enum.IsDefined(typeof(StaggerOrder), order)) throw new ArgumentOutOfRangeException(nameof(order));
            _order = order;
            _customDelay = null;
            return this;
        }

        /// <summary>
        /// Supplies absolute start delays for each item. A later Order or DelayBetween call replaces it.
        /// </summary>
        public TweenStaggerBuilder DelayBy(Func<GameObject, int, float> delaySelector)
        {
            _customDelay = delaySelector ?? throw new ArgumentNullException(nameof(delaySelector));
            return this;
        }

        /// <summary>
        /// Sets the deterministic seed used by Random ordering.
        /// </summary>
        public TweenStaggerBuilder Seed(int seed)
        {
            _randomSeed = seed;
            return this;
        }

        /// <summary>
        /// Adds a delay before the first item starts.
        /// </summary>
        public TweenStaggerBuilder WithDelay(float seconds)
        {
            StaggerDelayUtility.ValidateDelay(seconds, nameof(seconds));
            _rootDelay = seconds;
            return this;
        }

        /// <summary>
        /// Adds idle time after the final child tween, useful between root loops.
        /// </summary>
        public TweenStaggerBuilder WithTailDelay(float seconds)
        {
            StaggerDelayUtility.ValidateDelay(seconds, nameof(seconds));
            _tailDelay = seconds;
            return this;
        }

        /// <summary>
        /// Sets update mode and time-scale behavior on the root sequence.
        /// </summary>
        public TweenStaggerBuilder WithUpdate(UpdateType updateType, bool unscaledTime = false)
        {
            _updateType = updateType;
            _unscaledTime = unscaledTime;
            return this;
        }

        /// <summary>
        /// Controls whether the root sequence ignores Time.timeScale.
        /// </summary>
        public TweenStaggerBuilder WithUnscaledTime(bool enabled = true)
        {
            _unscaledTime = enabled;
            return this;
        }

        /// <summary>
        /// Loops the complete collection sequence. Infinite child tweens are not supported.
        /// </summary>
        public TweenStaggerBuilder WithLoops(int loops, LoopType loopType = LoopType.Restart)
        {
            if (loops == 0 || loops < -1) throw new ArgumentOutOfRangeException(nameof(loops), "Loops must be -1 or greater than zero.");
            _loops = loops;
            _loopType = loopType;
            return this;
        }

        /// <summary>
        /// Sets a DOTween identifier on the root sequence.
        /// </summary>
        public TweenStaggerBuilder WithId(string id)
        {
            _id = id;
            return this;
        }

        public TweenStaggerBuilder OnPlay(Action callback)
        {
            _onPlay += callback;
            return this;
        }

        public TweenStaggerBuilder OnComplete(Action callback)
        {
            _onComplete += callback;
            return this;
        }

        public TweenStaggerBuilder OnKill(Action callback)
        {
            _onKill += callback;
            return this;
        }

        /// <summary>
        /// Builds and starts the owner-linked collection sequence.
        /// </summary>
        public TweenHandle Play()
        {
            var handle = Build();
            handle.Tween?.Play();
            return handle;
        }

        /// <summary>
        /// Builds the owner-linked collection sequence in a paused state.
        /// </summary>
        public TweenHandle Build()
        {
            if (_animationFactory == null) throw new InvalidOperationException("TweenStaggerBuilder: Select a preset or custom animation before building.");
            if (_targets.Count == 0)
            {
                Debug.LogWarning("TweenStaggerBuilder: The target collection is empty.");
                return new TweenHandle(null);
            }

            ValidateLiveTargets();
            ValidatePresetCompatibility();
            float[] delays = ResolveDelays();
            var sequence = DOTween.Sequence();
            sequence.Pause();
            Tween pendingChild = null;

            try
            {
                for (int i = 0; i < _targets.Count; i++)
                {
                    pendingChild = _animationFactory(_targets[i], i);
                    if (pendingChild == null || !pendingChild.IsActive())
                    {
                        pendingChild?.Kill();
                        throw new InvalidOperationException($"TweenStaggerBuilder: {_animationName} returned no active tween for '{_targets[i].name}'.");
                    }

                    pendingChild.Pause();
                    if (pendingChild.Loops() < 0)
                    {
                        pendingChild.Kill();
                        throw new InvalidOperationException($"TweenStaggerBuilder: '{_animationName}' creates an infinite child tween. Use a finite child animation and apply WithLoops(-1) to the stagger root instead.");
                    }

                    sequence.Insert(delays[i], pendingChild);
                    pendingChild = null;
                }

                if (_tailDelay > 0f) sequence.AppendInterval(_tailDelay);
                ConfigureRoot(sequence);
                return new TweenHandle(sequence);
            }
            catch
            {
                pendingChild?.Kill();
                sequence.Kill();
                throw;
            }
        }

        /// <summary>
        /// Builds, starts, and awaits the collection sequence.
        /// </summary>
        public async Task<TweenHandle> PlayAsync(CancellationToken cancellationToken = default)
        {
            var handle = Play();
            await handle.AwaitCompletion(cancellationToken);
            return handle;
        }

        private static List<GameObject> SnapshotTargets(IEnumerable<GameObject> targets)
        {
            var result = new List<GameObject>();
            var uniqueTargets = new HashSet<GameObject>();
            int index = 0;

            foreach (GameObject target in targets)
            {
                if (target == null) throw new ArgumentException($"Target at index {index} is null.", nameof(targets));
                if (!uniqueTargets.Add(target)) throw new ArgumentException($"Target '{target.name}' occurs more than once.", nameof(targets));
                result.Add(target);
                index++;
            }

            return result;
        }

        private void ValidatePresetCompatibility()
        {
            if (_preset == null) return;

            for (int i = 0; i < _targets.Count; i++)
            {
                if (!_preset.CanApplyTo(_targets[i]))
                {
                    throw new InvalidOperationException($"TweenStaggerBuilder: Preset '{_preset.PresetName}' cannot be applied to target '{_targets[i].name}' at index {i}.");
                }
            }
        }

        private void ValidateLiveTargets()
        {
            if (_owner == null) throw new InvalidOperationException("TweenStaggerBuilder: The collection owner was destroyed before the group was built.");

            for (int i = 0; i < _targets.Count; i++)
            {
                if (_targets[i] == null) throw new InvalidOperationException($"TweenStaggerBuilder: Target at index {i} was destroyed before the group was built.");
            }
        }

        private float[] ResolveDelays()
        {
            if (_customDelay == null) return StaggerDelayUtility.CalculateDelays(_targets.Count, _delayBetween, _order, _randomSeed);

            var delays = new float[_targets.Count];
            for (int i = 0; i < _targets.Count; i++)
            {
                delays[i] = _customDelay(_targets[i], i);
                StaggerDelayUtility.ValidateDelay(delays[i], $"delay for target index {i}");
            }

            return delays;
        }

        private void ConfigureRoot(Sequence sequence)
        {
            var settings = TweenHelperSettings.Instance;
            sequence.SetEase(Ease.Linear);
            sequence.SetUpdate(_updateType ?? settings.DefaultUpdateType, _unscaledTime ?? settings.DefaultUnscaledTime);
            if (_rootDelay > 0f) sequence.SetDelay(_rootDelay);
            if (_loops != 1) sequence.SetLoops(_loops, _loopType);
            if (!string.IsNullOrEmpty(_id)) sequence.SetId(_id);
            sequence.SetTarget(_owner);
            sequence.SetLink(_owner);
            if (_onPlay != null) sequence.onPlay += _onPlay.Invoke;
            if (_onComplete != null) sequence.onComplete += _onComplete.Invoke;
            if (_onKill != null) sequence.onKill += _onKill.Invoke;
        }
    }
}
