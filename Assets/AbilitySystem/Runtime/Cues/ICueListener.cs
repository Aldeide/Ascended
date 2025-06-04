using AbilitySystem.Runtime.Tags;
using UnityEngine;

namespace AbilitySystem.Runtime.Cues
{
    public interface ICueListener
    {
        public GameplayTagQuery TagQuery { get; set; }
    }
}