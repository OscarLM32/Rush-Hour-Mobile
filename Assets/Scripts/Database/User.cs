using System;
using System.Collections;
using CoreSystems;
using Firebase.Auth;
using Newtonsoft.Json;
using UnityEngine;

namespace Database
{
    /// <summary>
    /// Class representation of the database user
    /// </summary>
    public class User : Singleton<User>
    {
        public string uid;
        public UserData data = new UserData();

        /// <summary>
        /// Sets up the user
        /// </summary>
        /// <param name="user">The firebase currentUser</param>
        /// <param name="action">An action to invoke on success</param>
        public void SetUpUser(FirebaseUser user, Action action = null)
        {
            uid = user.UserId;
            //TODO: action dependency
            StartCoroutine(SetUpUserAction(action));
            
        }

        /// <summary>
        /// Sets up the user data
        /// </summary>
        /// <param name="action">Action to execute on success</param>
        /// <returns>A Coroutine</returns>
        private IEnumerator SetUpUserAction(Action action)
        {
            var task = DatabaseManager.Instance.db.Child(DatabaseFields.USER.ToString()).Child(uid).GetValueAsync();

            yield return new WaitUntil(() => task.IsCompleted || task.IsCanceled);

            if (!task.IsCompleted) yield break;

            if (task.Result.GetRawJsonValue() != null)
            {
                data = JsonConvert.DeserializeObject<UserData>(task.Result.GetRawJsonValue()); 
            }
            
            action?.Invoke();
        }

        /// <summary>
        /// Transform the user data into a json
        /// </summary>
        /// <returns></returns>
        public string GetJsonUserData()
        {
            return JsonConvert.SerializeObject(data);
        }

        /// <summary>
        /// Destroys the user data
        /// </summary>
        public void DisposeUserData()
        {
            uid = null;
            data = null;
        }

        /// <summary>
        /// Representation of the user data
        /// </summary>
        [Serializable]
        public class UserData
        {
            public int score;

            public UserData()
            {
                score = 0;
            }
        }
    }
}