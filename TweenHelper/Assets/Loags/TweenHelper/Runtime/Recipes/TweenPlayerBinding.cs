using System;
using System.Collections.Generic;
using UnityEngine;

namespace LB.TweenHelper
{
    [Serializable]
    public sealed class TweenPlayerBinding
    {
        [SerializeField] private string bindingId;
        [SerializeField] private GameObject target;
        [SerializeField] private List<GameObject> targets = new List<GameObject>();

        public string BindingId => bindingId;
        public GameObject Target => target;
        public IReadOnlyList<GameObject> Targets => targets;

        public TweenPlayerBinding()
        {
        }

        public TweenPlayerBinding(string bindingId, GameObject target)
        {
            this.bindingId = bindingId;
            this.target = target;
        }

        public TweenPlayerBinding(string bindingId, IEnumerable<GameObject> targets)
        {
            this.bindingId = bindingId;
            this.targets = targets == null ? new List<GameObject>() : new List<GameObject>(targets);
        }
    }
}
