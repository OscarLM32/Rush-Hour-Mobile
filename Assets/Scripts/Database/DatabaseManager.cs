using System;
using System.Collections;
using System.Threading.Tasks;
using CoreSystems;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Database
{
    
    /// <summary>
    /// handles the database operations
    /// </summary>
    public class DatabaseManager : Singleton<DatabaseManager>
    {
        public GameObject debug;
        
        /// <summary>
        /// A reference to the root database
        /// </summary>
        public DatabaseReference db;
        
        /// <summary>
        /// Reference to the auth system
        /// </summary>
        public FirebaseAuth auth; 
        
        private Firebase.FirebaseApp _app;

        /// <summary>
        /// Tells if the database passes the requirements
        /// </summary>
        private bool _firebaseProperlySetUp = false;

        private bool _taskResult;

        #region UNITY FUNCTIONS
        protected new void Awake()
        {
            base.Awake();

            db = FirebaseDatabase.DefaultInstance.RootReference;
            auth = FirebaseAuth.DefaultInstance; 
            CheckDependencies(); 
            //FirebaseApp.LogLevel = LogLevel.Debug;
        }

        private void Start()
        {
            if (!_firebaseProperlySetUp)
            {
                //Close app?
                Debug.LogError("[DATABASE_MANAGER]: Cannot connect to firebase");
                return;
            }
            else
            {
                debug.SetActive(false);
            }
        }

        #endregion

        #region UTILS FUNCTIONS

        /// <summary>
        /// Uploads a default level
        /// </summary>
        /// <param name="level">The level representation</param>
        /// <param name="difficulty">The difficulty to upload it to</param>
        public void UploadAppDefaultLevel(LevelRepresentation level, LevelDifficulties difficulty)
        {
            var jsonLevel = JsonConvert.SerializeObject(level, Formatting.None,
                new JsonSerializerSettings()
                { 
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }
            );

            db.Child(DatabaseFields.LEVEL.ToString())
                .Child(difficulty.ToString())
                .Push()
                .SetRawJsonValueAsync(jsonLevel); 
        }
        
        /// <summary>
        /// Upload a level created by the user in the game
        /// </summary>
        /// <param name="level">The level representation</param>
        public void UploadUserCreatedLevel(LevelRepresentation level)
        {
            var jsonLevel = JsonConvert.SerializeObject(level, Formatting.None,
                new JsonSerializerSettings()
                { 
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }
            );

            db.Child(DatabaseFields.USER_CREATED_LEVEL.ToString()).Push().SetRawJsonValueAsync(jsonLevel);
        }

        #endregion
        
        #region USER MANAGEMENT FUNCTIONS

        /// <summary>
        /// Creates a user with email and password
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="password">Password</param>
        /// <param name="OnSuccess">Action on registry success</param>
        /// <param name="OnFailure">Action on registry failure</param>
        public void CreateEmailPasswordAccount(string email, string password, Action OnSuccess, Action OnFailure)
        {
            auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
                if (task.IsCanceled) {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                    _taskResult = false;
                    OnFailure();
                    return;
                }
                if (task.IsFaulted) {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    _taskResult = false;
                    OnFailure();
                    return;
                }

                // Firebase user has been created.
                Firebase.Auth.AuthResult result = task.Result;
                Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                    result.User.DisplayName, result.User.UserId);

                OnSuccess();
                
                User.Instance.SetUpUser(result.User);
                db.Child(DatabaseFields.USER.ToString()).Child(User.Instance.uid).SetRawJsonValueAsync(User.Instance.GetJsonUserData());
            });
        }
        

        public void SignInWithEmailPassword(string email, string password, Action OnSuccess, Action OnFailure)
        {
            StartCoroutine(SignInWithEmailPassWordAction(email, password, OnSuccess, OnFailure));
        }

        /// <summary>
        /// Signs in a user with email and password
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="password">Password</param>
        /// <param name="OnSuccess">Action on success</param>
        /// <param name="OnFailure">Action on success</param> 
        private IEnumerator SignInWithEmailPassWordAction(string email, string password, Action OnSuccess,
            Action OnFailure)
        {
            var task = auth.SignInWithEmailAndPasswordAsync(email, password);
            
            yield return new WaitUntil(() => task.IsCompleted || task.IsCanceled);

            if (task.IsCompleted)
            {
                User.Instance.SetUpUser(auth.CurrentUser, OnSuccess);
            }
            else
            {
                OnFailure();
            }
        }


        #endregion

        #region PRIVATE FUNCTIONS

        private IEnumerator TaskWrapper(Task task)
        {
            task.Start();

            yield return new WaitUntil(() => task.IsCompleted);

            _taskResult = task.Exception == null;
        }

        /// <summary>
        /// Checks if the system complies all the requirements
        /// </summary>
        private void CheckDependencies()
        {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available) {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    _app = FirebaseApp.DefaultInstance;
                    
                    // Set a flag here to indicate whether Firebase is ready to use by your app.
                    _firebaseProperlySetUp = true;

                } else {
                    UnityEngine.Debug.LogError(System.String.Format(
                        "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                }
            }); 
        }

        #endregion
    }
}