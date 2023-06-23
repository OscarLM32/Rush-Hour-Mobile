using UnityEngine;

namespace ScriptableObjects
{
    /// <summary>
    /// Set of visual aspect of levels
    /// </summary>
    [CreateAssetMenu(menuName = "LevelVisuals", fileName = "LevelVisuals")]
    public class SOLevelVisuals : ScriptableObject
    {
        public GameObject[] visuals;
    }
}