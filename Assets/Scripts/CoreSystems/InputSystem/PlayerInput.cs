using System;
using Extensions;
using Gameplay;
using UnityEngine;

namespace CoreSystems.InputSystem
{
    /// <summary>
    /// Manages the player input to control the vehicles
    /// </summary>
    public class PlayerInput : MonoBehaviour
    {
        /// <summary>
        /// The vehicle layer to handle the input
        /// </summary>
        public LayerMask vehicleLayer;
        
        public GameObject selectedVehicle;

        /// <summary>
        /// Max touches admitted
        /// </summary>
        private const int _admittedTouchCount = 1;
        
        /// <summary>
        /// The touch to be handled
        /// </summary>
        private Touch _touch;

        /// <summary>
        /// The scene's gameplay grid manager
        /// </summary>
        private GameplayGridManager _gameplayGridManagerInstance;

        /// <summary>
        /// Las grid pos that was selected
        /// </summary>
        private Vector2 _lastGridPos;

        #region UNITY FUNCTIONS

        private void Update()
        {
            if (Input.touches.Length != _admittedTouchCount) return;

            _touch = Input.touches[0];

            HandleTouch();
        }

        private void Start()
        {
            _gameplayGridManagerInstance = GameplayGridManager.Instance as GameplayGridManager;
        }

        #endregion
        
        #region TOUCH HANDLING

        /// <summary>
        /// Handles the possible touch phases
        /// </summary>
        private void HandleTouch()
        {
            switch (_touch.phase)
            {
                case TouchPhase.Began:
                    OnTouchBegan();
                    break;
                case TouchPhase.Moved:
                    OnTouchMoved();
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    OnTouchEnded();
                    break;
                default:
                    break;
            }
        }
        
        /// <summary>
        /// Handles the action when the touch just begins
        /// </summary>
        private void OnTouchBegan()
        {
            var touchWorldPosition = Camera.main.ScreenToWorldPoint(_touch.position); 
            _lastGridPos = _gameplayGridManagerInstance.SnapCoordinateToGrid(touchWorldPosition);
            
            var ray = Camera.main.ScreenToWorldPoint(_touch.position);
            var rayCastHit = Physics2D.Raycast(ray, Vector2.zero, Mathf.Infinity, vehicleLayer);

            if (rayCastHit)
            {
                selectedVehicle = rayCastHit.transform.gameObject;
            }
        }

        /// <summary>
        /// Handles the logic when the touch moves
        /// </summary>
        private void OnTouchMoved()
        {
            if (!selectedVehicle) return;

            var touchWorldPosition = Camera.main.ScreenToWorldPoint(_touch.position);
            var currentGridPos = _gameplayGridManagerInstance.SnapCoordinateToGrid(touchWorldPosition);
            if (currentGridPos == _lastGridPos) return;

            var direction = CalculateMovingDirectionUnitary(_lastGridPos, currentGridPos);
            _gameplayGridManagerInstance.CheckMoveVehicle(selectedVehicle, direction);
            //Debug.Log(direction);
            _lastGridPos = currentGridPos;
        }

        /// <summary>
        /// Handles the logic when the touch ends
        /// </summary>
        private void OnTouchEnded()
        {
            selectedVehicle = null;
        }

        #endregion

        #region PRIVATE FUNCTIONS

        /// <summary>
        /// Returns a unitary vector representing the input done
        /// </summary>
        /// <param name="from">Grid tile moving from</param>
        /// <param name="to">Grid tile moving to</param>
        /// <returns></returns>
        private Vector2 CalculateMovingDirectionUnitary(Vector2 from, Vector2 to)
        {
            var dif = to - from;

            int xDif = dif.x == 0 ? 0 : (int)(Math.Abs(dif.x) / dif.x);
            int yDif = dif.y == 0 ? 0 : (int)(Math.Abs(dif.y) / dif.y);

            return new Vector2(xDif, yDif);
        }

        #endregion
    }
}