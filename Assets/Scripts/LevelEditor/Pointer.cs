using LevelEditor.GridSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace LevelEditor
{
    /// <summary>
    /// Handles the selection and movement of the objects in scene
    /// </summary>
    public class Pointer : MonoBehaviour
    {
        /// <summary>
        /// singleton instance
        /// </summary>
        public static Pointer Instance;

        /// <summary>
        /// The layermask of the placeableobjects
        /// </summary>
        public LayerMask placeableObjectLayerMask;
        
        public GameObject draggingObject;

        private Touch _touch;
        private Camera _mainCamera;


        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                return;
            }
            Destroy(gameObject);
        }

        #region UNITY FUNCTIONS

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            if (Input.touches.Length != 1) return;
            
            _touch = Input.GetTouch(0);

            CheckPointerAction();
        } 

        #endregion

        #region PUBLIC FUNCTIONS

        /// <summary>
        /// Sets a placeableObject being dragged
        /// </summary>
        /// <param name="vehicle">The vehicle being dragged</param>
        public void SetNewDraggingObject(GameObject vehicle)
        {
            draggingObject = vehicle.gameObject;  
            draggingObject.transform.SetParent(transform);
            draggingObject.transform.localPosition = Vector3.zero;
        }

        #endregion

        #region PRIVATE FUNCTIONS
        
        /// <summary>
        /// Handle which logic of the pointer has to be executed
        /// </summary>
        private void CheckPointerAction()
        {
            switch (_touch.phase)
            {
                case TouchPhase.Began:
                    OnTouchBegin();
                    break;
                case TouchPhase.Moved:
                    MovePointer();
                    break;
                case TouchPhase.Ended:
                    OnTouchEnd();
                    break;
                default:
                    return;
            }
        }

        /// <summary>
        /// Logic to be executed when the pointer begins
        /// </summary>
        private void OnTouchBegin()
        {
            transform.position = _touch.position;
            
            var ray = _mainCamera.ScreenToWorldPoint(_touch.position);
            var rayCastHit = Physics2D.Raycast(ray, Vector2.zero, Mathf.Infinity, layerMask:placeableObjectLayerMask);

            //If nothing has been hit return
            if (!rayCastHit) return;

            var objectTransform = rayCastHit.transform;
            var placeableObject = objectTransform.GetComponent<PlaceableObject>();
            if (placeableObject)
            {
                (EditorGridManager.Instance as EditorGridManager)?.TakeObject(placeableObject);
                SetNewDraggingObject(objectTransform.gameObject);
            }
        }

        #region POINTER MOVEMENT

        /*private bool IsPointerInsideGrid()
        {
            var ray = _mainCamera.ScreenToWorldPoint(_touch.position);
            var rayCastHits = Physics2D.RaycastAll(ray, Vector2.zero);
            
            foreach (var t in rayCastHits)
            {
                if (t.transform.GetComponent<GridManager>())
                {
                    return true;
                }
            }
            return false; //Returns whether gridManager is null or not
        }*/
        
        /// <summary>
        /// Moves the pointer
        /// </summary>
        private void MovePointer()
        {
            var position = _mainCamera.ScreenToWorldPoint(_touch.position);
            position.z = -1; //If not set, the default values is going to be the cameras' value, so the pointer will no tbe visible
            transform.position = position;  
        }

        #endregion
        
        /// <summary>
        /// Handles the logic when the touch ends
        /// </summary>
        private void OnTouchEnd()
        {
            if (!draggingObject) return;

            //Check object placeable
            var placeableObject = draggingObject.GetComponent<PlaceableObject>();
            if (!placeableObject)
            {
                Debug.LogWarning("The object being tried to place is not a 'PlaceableObject'.");
                return;
            }

            //Try to place it in other case
            (EditorGridManager.Instance as EditorGridManager)?.Place(draggingObject.GetComponent<PlaceableObject>());
            draggingObject = null;
        }
         
        #endregion
    }
}