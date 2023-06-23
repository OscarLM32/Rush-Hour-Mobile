using System;
using UnityEngine;

namespace CoreSystems.SceneSystem
{
    /// <summary>
    /// A test class for testing the scene controller
    /// </summary>
    public class TestScene : MonoBehaviour
    {
        public SceneController sceneController;
        
    #region Unity Functions
    #if UNITY_EDITOR

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                sceneController.Load(SceneType.NONE);
            }

            if (Input.GetKeyDown(KeyCode.N))
            {
                
            }
        }

    #endif    
    #endregion
    }
}