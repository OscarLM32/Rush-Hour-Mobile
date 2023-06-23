using System;
using System.Collections;
using System.Threading.Tasks;
using CoreSystems.MenuSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CoreSystems.SceneSystem
{
    /// <summary>
    /// Handles the logic behind switching scenes
    /// </summary>
    public class SceneController : MonoBehaviour
    {
        /// <summary>
        /// An action to be executed after the scene is loaded
        /// </summary>
        public delegate void SceneLoadDelegate();
        
        /// <summary>
        /// Singleton instance
        /// </summary>
        public static SceneController instance;
        
        public bool debug;
        
        private PageController _menu;
        private SceneType _targetScene;
        /// <summary>
        /// The page to be used as loading page
        /// </summary>
        private PageType _loadingPage;
        
        /// <summary>
        /// Function to invoke when the scene is loaded
        /// </summary>
        private SceneLoadDelegate _sceneLoadDelegate;
        private bool _sceneIsLoading;

        private PageController menu
        {
            get
            {
                if (_menu == null)
                {
                    _menu = PageController.instance;
                }

                if (_menu == null)
                {
                    LogWarning("You are trying to access the PageController, but no instance was found");
                }

                return _menu;
            }
        }

        /// <summary>
        /// Retrieves the name of the current scene
        /// </summary>
        private string currentSceneName
        {
            get
            {
                return SceneManager.GetActiveScene().name;
            }
        }
        
        #region Unity functions

        private void Awake()
        {
            if (!instance)
            {
                Configure();
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDisable()
        {
            Dispose();
        }

        #endregion
    
        #region Public functions

        /// <summary>
        /// Sets up the necessary values to load a scene
        /// </summary>
        /// <param name="scene">The scene to be loaded</param>
        /// <param name="sceneLoadDelegate">Action to execute on load completion</param>
        /// <param name="reload">Whether the scene is being reloaded</param>
        /// <param name="loadingPage">The loading page to be used</param>
        public void Load(SceneType scene,
                         SceneLoadDelegate sceneLoadDelegate = null,
                         bool reload = false,
                         PageType loadingPage = PageType.NONE)
        {
            if (loadingPage != PageType.NONE && !menu)
            {
                return;
            }
            

            if (!SceneCanBeLoaded(scene, reload))
            {
                LogWarning("Scene cannot be loaded");
                return;
            }
            
            
            _sceneIsLoading = true;
            _targetScene = scene;
            _loadingPage = loadingPage;
            _sceneLoadDelegate = sceneLoadDelegate;
            StartCoroutine(LoadScene());
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Configures an instance of the class
        /// </summary>
        private void Configure()
        {
            instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        /// <summary>
        /// Disposes of the scene controller
        /// </summary>
        private void Dispose()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        /// <summary>
        /// Handles the action specified when the scene is loaded
        /// </summary>
        private async void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (_targetScene == SceneType.NONE)
            {
                return;
            }

            //TODO: this is not valid in the current solution
            SceneType sceneType = StringToSceneType(scene.name);
            if (_targetScene != sceneType)
            {
                return;
            }

            //This null is not necessary 
            if (_sceneLoadDelegate != null)
            {
                try
                {
                    _sceneLoadDelegate();
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    LogWarning("Unable to respond with sceneLoadDelegate after scene ["+sceneType+"] loaded.");
                }
            }

            if (_loadingPage != PageType.NONE)
            {
                await Task.Delay(500);
                menu.TurnPageOff(_loadingPage);
            }

            _sceneIsLoading = false;
        }

        /// <summary>
        /// Loads the set up scene
        /// </summary>
        /// <returns>A coroutine</returns>
        private IEnumerator LoadScene()
        {
            if (_loadingPage != PageType.NONE)
            {
                menu.TurnPageOn(_loadingPage);
                while (!menu.PageIsOn(_loadingPage))
                {
                    yield return null;
                }
            }
            string targetSceneName = _targetScene.ToString();
            Log("Loading " + targetSceneName);
            SceneManager.LoadScene(targetSceneName);
        }

        /// <summary>
        /// Returns if the scene can be loaded
        /// </summary>
        /// <param name="scene">The scene to load</param>
        /// <param name="reload">Is a reload</param>
        /// <returns>Yes or no</returns>
        private bool SceneCanBeLoaded(SceneType scene, bool reload)
        {
            string targetSceneName = scene.ToString();
            if (currentSceneName == targetSceneName && !reload)
            {
                LogWarning("You are trying to load a scene ["+scene+"] that is already loaded.");
                return false;
            }
            if (targetSceneName == string.Empty)
            {
                LogWarning("The scene you are trying to load ["+scene+"] is not valid.");
                return false;
            }
            if (_sceneIsLoading)
            {
                LogWarning("Unable to load scene ["+scene+"]. Another scene ["+_targetScene+"] is loading.");
                return false;
            }

            return true;
        } 

        //Not needed since I will be calling the enums the same way the scenes are called
        /*private string SceneTypeToString(SceneType scene)
        {
            switch (scene)
            {
                case SceneType.MAIN_MENU: return "MainMenu";
                case SceneType.LEVEL: return "Level";
                default: 
                    LogWarning("Scene ["+scene+"] does not contain a type for a valid scene");
                    return string.Empty;
            } 
        }*/
        
        /// <summary>
        /// Turns the scene name into type
        /// </summary>
        /// <param name="scene">The name of the scene</param>
        /// <returns></returns>
        private SceneType StringToSceneType(string scene)
        {
            foreach (string sceneName in Enum.GetNames(typeof(SceneType))) {
                if (sceneName == scene) {
                    return (SceneType) Enum.Parse(typeof(SceneType), sceneName);
                }
            }

            return SceneType.NONE;

            /*switch (name)
            {
                case "MainMenu": return SceneType.MAIN_MENU;
                case "Level": return SceneType.LEVEL;
                default: 
                    LogWarning("Scene ["+name+"] does not contain a name for a valid scene"); 
                    return SceneType.NONE;
            }*/
        }
    
        private void Log(string msg)
        {
            Debug.Log("[SceneController]: " + msg);
        }

        private void LogWarning(string msg)
        {
            Debug.LogWarning("[SceneController]: " + msg);
        }

    #endregion
    }
}