using System;
using System.Linq;
using CoreSystems;
using CoreSystems.SceneSystem;
using Database;
using Gameplay;
using LevelEditor.GridSystem;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace LevelEditor
{
    /// <summary>
    /// Manages the game when the user is editing a level
    /// </summary>
    public class LevelEditorManager : SingletonScene<LevelEditorManager>
    {
        /// <summary>
        /// if the level has been completed in play mode
        /// </summary>
        public bool levelCompleted = false;
        public Button uploadLevelButton;

        [Space]
        public EditorGridManager editorGridManager;
        public GameplayGridManager gameplayManager;

        [Space]
        public GameObject editingScene;
        public GameObject playingScene;

        private LevelRepresentation _levelRepresentation;
        
        #region UNITY FUNTIONS

        private void OnEnable()
        {
            LevelEditorEvents.LevelEdited += OnLevelEdited;
            LevelEditorEvents.EditedLevelCompleted += OnEditedLevelCompleted;
            LevelEditorEvents.EditedLevelNotCompletable += OnLevelNotCompletable;
        }

        private void OnDisable()
        {
            LevelEditorEvents.LevelEdited -= OnLevelEdited;
            LevelEditorEvents.EditedLevelCompleted -= OnEditedLevelCompleted;
            LevelEditorEvents.EditedLevelNotCompletable -= OnLevelNotCompletable; 
        }

        #endregion

        /// <summary>
        /// Sets the size of the grids
        /// </summary>
        /// <param name="size">The size</param>
        public void SetGridsSize(int size)
        {
            var sizes = (LevelSize[]) Enum.GetValues(typeof(LevelSize));
            editorGridManager.SetSize(sizes[size]);
            editorGridManager.SetInitialExitPosition();
            Camera.main.orthographicSize = 7 + size*2;
        }
        
        /// <summary>
        /// Switches from editing mode to playing mode
        /// </summary>
        public void PlayEditedLevel()
        {
            editingScene.SetActive(false);
            _levelRepresentation = (EditorGridManager.Instance as EditorGridManager)?.GetLevelRepresentation();
            playingScene.SetActive(true); 
            LevelGenerator.Instance.ClearLevel();
            LevelGenerator.Instance.GenerateLevelFromLeveLRepresentation(_levelRepresentation);
        }

        /// <summary>
        /// Uploads the level edited
        /// </summary>
        public void UploadLevel()
        {
            DatabaseManager.Instance.UploadUserCreatedLevel(_levelRepresentation);
            OnLevelEdited();
        }

        /// <summary>
        /// Return to the main menu
        /// </summary>
        public void ReturnToMainMenu()
        {
            SceneController.instance.Load(SceneType.MainMenu);
        }
        
        /// <summary>
        /// Switches from play mode to edit mode
        /// </summary>
        public void ReturnToLevelEdition()
        {
            LevelGenerator.Instance.ClearLevel();
            playingScene.SetActive(false);
            editingScene.SetActive(true);
        }  

        #region PRIVATE FUNCTIONS

        /// <summary>
        /// Logic to be executed when the level is considered to be edited
        /// </summary>
        private void OnLevelEdited()
        {
            levelCompleted = false;
            uploadLevelButton.interactable = false;
        }

        /// <summary>
        /// Logic to be executed when the level is completed in play mode
        /// </summary>
        private void OnEditedLevelCompleted()
        {
            levelCompleted = true;
            uploadLevelButton.interactable = true;
            ReturnToLevelEdition();
        }

        /// <summary>
        /// Logic to be executed when it is detected that the level is not completable
        /// </summary>
        private void OnLevelNotCompletable()
        {
            ReturnToLevelEdition();
        }

        #endregion
    }
}