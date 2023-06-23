using System.Collections;
using CoreSystems;
using CoreSystems.SceneSystem;
using Database;
using Firebase.Database;
using Newtonsoft.Json;
using UnityEngine;
using Random = System.Random;

namespace Gameplay
{
    /// <summary>
    /// Manager that handles the game when the user is playing a level created by other user
    /// </summary>
    public class UserLevelGameManager : SingletonScene<UserLevelGameManager>
    {
        private DataSnapshot data;
        private void Start()
        {
            StartCoroutine(DownloadLevelAction());
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
        /// Downloads a level to play
        /// </summary>
        public void DownloadLevel()
        {
            Destroy(GameplayGridManager.Instance.visualObject); 
            LevelGenerator.Instance.ClearLevel();
            StartCoroutine(DownloadLevelAction());
        }

        /// <summary>
        /// Returns to main menu
        /// </summary>
        public void ReturnToMainMenu()
        {
            SceneController.instance.Load(SceneType.MainMenu);
        }
        
        /// <summary>
        /// The action that retrieves a level from the database
        /// </summary>
        /// <returns></returns>
        private IEnumerator DownloadLevelAction()
        {
            var task = DatabaseManager.Instance.db.Child(DatabaseFields.USER_CREATED_LEVEL.ToString()).GetValueAsync();

            yield return new WaitUntil(() => task.IsCompleted || task.IsCanceled);

            if (task.IsCompleted)
            {
                int max = (int)task.Result.ChildrenCount;
                var random = new Random();
                int randomInt = random.Next(0, max);
                //Debug.Log(randomInt);
                int currentIndex = 0;
                foreach (DataSnapshot childSnapshot in task.Result.Children)
                {
                    if (currentIndex == randomInt)
                    {
                        // Found the random entry
                        string entryValue = childSnapshot.GetRawJsonValue();
                        var lr = JsonConvert.DeserializeObject<LevelRepresentation>(entryValue);
                        Camera.main.orthographicSize = 7 + (int)lr.levelSize;
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
            DownloadLevel();
        }
    }
}