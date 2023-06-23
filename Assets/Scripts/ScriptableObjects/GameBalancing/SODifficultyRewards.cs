using UnityEngine;

namespace ScriptableObjects.GameBalancing
{
    /// <summary>
    /// The rewards per difficulty
    /// </summary>
    [CreateAssetMenu(menuName = "GameBalancing/DifficultyRewards", fileName = "DifficultyRewards")]
    public class SODifficultyRewards : ScriptableObject
    {
        public int[] rewards;
    }
}