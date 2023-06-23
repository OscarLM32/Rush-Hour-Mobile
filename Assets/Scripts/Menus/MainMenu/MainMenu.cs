using System;
using System.Collections;
using CoreSystems.MenuSystem;
using CoreSystems.SceneSystem;
using Database;
using Gameplay;
using LevelEditor;
using ScriptableObjects.GameBalancing;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Menus.MainMenu
{
    /// <summary>
    /// Logic of the main menu
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        public SODifficultyUnlockScore difficultyUnlockScore;
        
        private Array _difficulties;
        private int _difficulty;
        private int _maxPlayableDifficulty = (int)LevelDifficulties.BEGINNER ;
        [SerializeField]private Button[] _difficultyButtons;

        private int _levelSize = 0;

        private void Start()
        {
            _difficulties = Enum.GetValues(typeof(LevelDifficulties));
            
            CheckCanPlayDifficulties();
        }
        
        #region PUBLIC FUNCTIONS

        /// <summary>
        /// Loads the scene to play user-made levels
        /// </summary>
        public void PlayUserLevel()
        {
            SceneController.instance.Load(SceneType.UserLevel);
        }
        
        /// <summary>
        /// selects the level difficulty from the options in the menu
        /// </summary>
        /// <param name="difficulty"></param>
        public void SelectLevelDifficulty(int difficulty)
        {
            _difficulty = difficulty;
            SceneController.instance.Load(SceneType.DefaultLevel, SetDifficulty);
        } 

        /// <summary>
        /// selects a level size from the options in the meni
        /// </summary>
        /// <param name="size">The isze</param>
        public void SelectLevelSize(int size)
        {
            _levelSize = size;
            SceneController.instance.Load(SceneType.LevelEditor, SetGridSize);
        }
        
        #endregion

        #region PRIVATE FUNCTIONS
        
        /// <summary>
        /// Checks which difficulties the player can play
        /// </summary>
        private void CheckCanPlayDifficulties()
        {
            var score = User.Instance.data.score;

            int nextScore;
            foreach (var difficulty in _difficulties)
            {
                nextScore = difficultyUnlockScore.scores[(int) difficulty]; 
                Debug.Log(nextScore);
                if (score >= nextScore && _maxPlayableDifficulty <= (int)difficulty)
                {
                    Debug.Log(difficulty);
                    _maxPlayableDifficulty = (int) difficulty;
                }
                else
                {
                    break;
                }
            }

            for (int i = 0; i <= _maxPlayableDifficulty; i++)
            {
                _difficultyButtons[i].interactable = true;
            }
        }

        /// <summary>
        /// Sets the size of the grid the managers have to handle
        /// </summary>
        private void SetGridSize()
        {
            LevelEditorManager.Instance.SetGridsSize(_levelSize);
        }

        /// <summary>
        /// Sets the difficulty of the level the player want to play
        /// </summary>
        private void SetDifficulty()
        {
            Debug.Log((LevelDifficulties)Enum.Parse(typeof(LevelDifficulties), _difficulty.ToString()));
            DefaultLevelGameManager.difficulty = (LevelDifficulties)Enum.Parse(typeof(LevelDifficulties), _difficulty.ToString());
            //StartCoroutine(SetDifficultyAction());
        }

        private IEnumerator SetDifficultyAction()
        {
            while (DefaultLevelGameManager.Instance == null)
            {
                yield return null;
            }

        }

        #endregion
    }
}