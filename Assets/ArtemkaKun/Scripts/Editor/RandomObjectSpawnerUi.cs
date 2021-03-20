using ArtemkaKun.Scripts.RangeChancesSystem;
using UnityEditor;
using UnityEngine;

namespace ArtemkaKun.Scripts.Editor
{
    /// <summary>
    /// Separate class, that inherits RangeChancesControllerUi because of generics.
    /// </summary>
    [CustomEditor(typeof(RandomObjectSpawner))]
    public class RandomObjectSpawnerUi : RangeChancesControllerUi<GameObject>
    {
        
    }
}