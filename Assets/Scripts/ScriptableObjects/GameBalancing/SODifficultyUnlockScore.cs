using UnityEngine;

namespace ScriptableObjects.GameBalancing
{
    /// <summary>
    /// The score needed to unlock a difficulty
    /// </summary>
    [CreateAssetMenu(menuName = "GameBalancing/DifficultyUnlockScore", fileName = "DifficultyUnlockScore")]
    public class SODifficultyUnlockScore : ScriptableObject
    {
        public int[] scores;
    }
}