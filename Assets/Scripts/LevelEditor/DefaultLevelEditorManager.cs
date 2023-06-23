using System;
using System.Linq;
using CoreSystems;
using Database;
using Gameplay;
using LevelEditor.GridSystem;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace LevelEditor
{
     /// <summary>
     /// Handles the logic of the game when building default levels 
     /// </summary>
    public class DefaultLevelEditorManager : SingletonScene<LevelEditorManager>
    {
        [Space] 
        public LevelSize levelSize;
        public LevelDifficulties difficulty;
        
        [Space]
        public bool levelCompleted = false;
        public Button uploadLevelButton;

        [Space]
        public EditorGridManager editorGridManager;

        [Space]
        public GameObject editingScene;
        public GameObject playingScene;

        private LevelRepresentation _levelRepresentation;
        
        #region UNITY FUNTIONS

        private void Start()
        {
            SetGridsSize((int)levelSize);
        }

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

        private void SetGridsSize(int size)
        {
            Debug.Log($"The desired size = {size}");
            var sizes = (LevelSize[]) Enum.GetValues(typeof(LevelSize));
            editorGridManager.SetSize(sizes[size]);
            editorGridManager.SetInitialExitPosition();
            Camera.main.orthographicSize = 7 + size*2;
        }
        
        public void PlayEditedLevel()
        {
            editingScene.SetActive(false);
            _levelRepresentation = (EditorGridManager.Instance as EditorGridManager)?.GetLevelRepresentation();
            playingScene.SetActive(true); 
            LevelGenerator.Instance.ClearLevel();
            LevelGenerator.Instance.GenerateLevelFromLeveLRepresentation(_levelRepresentation);
        }

        public void UploadLevel()
        {
            DatabaseManager.Instance.UploadAppDefaultLevel(_levelRepresentation, difficulty);
        }
        
        public void ReturnToLevelEdition()
        {
            LevelGenerator.Instance.ClearLevel();
            playingScene.SetActive(false);
            editingScene.SetActive(true);
        }  

        #region PRIVATE FUNCTIONS

        private void OnLevelEdited()
        {
            levelCompleted = false;
            uploadLevelButton.interactable = false;
        }

        private void OnEditedLevelCompleted()
        {
            levelCompleted = true;
            uploadLevelButton.interactable = true;
            ReturnToLevelEdition();
        }

        private void OnLevelNotCompletable()
        {
            ReturnToLevelEdition();
        }

        #endregion
    }
}