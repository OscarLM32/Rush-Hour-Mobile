using System;
using System.Collections;
using CoreSystems;
using CoreSystems.SceneSystem;
using Database;
using Firebase.Database;
using Newtonsoft.Json;
using ScriptableObjects.GameBalancing;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random; 

namespace Gameplay
{
    /// <summary>
    /// Manager that handles the game when the user is playing a default level
    /// </summary>
    public class DefaultLevelGameManager : SingletonScene<DefaultLevelGameManager>
    {
        /// <summary>
        /// The rewards to give on level completion
        /// </summary>
        public SODifficultyRewards rewards;
        
        /// <summary>
        /// The difficulty of the level
        /// </summary>
        public static LevelDifficulties difficulty = LevelDifficulties.BEGINNER;
        
        private DataSnapshot data;

        private void Start()
        {
            DownloadLevel();
        }

        private void OnEnable()
        {
            GameplayEvents.LevelCompleted += OnLevelCompletion;
        }

        private void OnDisable()
        {
            GameplayEvents.LevelCompleted -= OnLevelCompletion;
        }

        /// <summary>
        /// Return to main menu
        /// </summary>
        public void ReturnToMainMenu()
        {
            SceneController.instance.Load(SceneType.MainMenu);
        }

        /// <summary>
        /// Downloads a level and instances it
        /// </summary>
        public void DownloadLevel()
        {
            Destroy(GameplayGridManager.Instance.visualObject); 
            LevelGenerator.Instance.ClearLevel();
            StartCoroutine(DownloadLevelAction());
        }
        
        /// <summary>
        /// The action of retrieving the level from the database
        /// </summary>
        /// <returns>A Coroutine</returns>
        private IEnumerator DownloadLevelAction()
        {
            var task = DatabaseManager.Instance.db.Child(DatabaseFields.LEVEL.ToString()).Child(difficulty.ToString()).GetValueAsync();

            yield return new WaitUntil(() => task.IsCompleted || task.IsCanceled);

            if (task.IsCompleted)
            {
                int max = (int)task.Result.ChildrenCount;
                var random = new Random();
                int randomInt = random.Next(0, max);
                Debug.Log(randomInt);
                int currentIndex = 0;
                foreach (DataSnapshot childSnapshot in task.Result.Children)
                {
                    if (currentIndex == randomInt)
                    {
                        // Found the random entry
                        string entryValue = childSnapshot.GetRawJsonValue();
                        var lr = JsonConvert.DeserializeObject<LevelRepresentation>(entryValue);
                        Camera.main.orthographicSize = 7 + (int)lr.levelSize*2;
                        LevelGenerator.Instance.GenerateLevelFromLeveLRepresentation(lr);

                        break;
                    }
                    currentIndex++;
                }  
            }
        }

        /// <summary>
        /// Action to be executed when the level is completed
        /// </summary>
        private void OnLevelCompletion()
        {
            StartCoroutine(UpdatePlayerScore());
            DownloadLevel();
        }

        /// <summary>
        /// Sets the new player score when the user completes a level
        /// </summary>
        /// <returns></returns>
        private IEnumerator UpdatePlayerScore()
        {
            User.Instance.data.score += rewards.rewards[(int) difficulty];
            var score = User.Instance.data.score;
            var task = DatabaseManager.Instance.db.Child(DatabaseFields.USER.ToString()).Child(User.Instance.uid)
                .Child("score").SetValueAsync(score);
            
            yield return new WaitUntil(() => task.IsCompleted || task.IsCanceled);

            if (task.IsCompleted)
            {
                Debug.Log("Score properly setup");
            }
        }
    }
}