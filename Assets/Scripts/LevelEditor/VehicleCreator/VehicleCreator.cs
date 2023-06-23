using System;
using LevelEditor.GridSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace LevelEditor.VehicleCreator
{
    /// <summary>
    /// A specific creator of vehicle
    /// </summary>
    public class VehicleCreator : MonoBehaviour
    {
        public VehicleType type;
        /*
         * All vehicle prefabs are going to be set up as follows: a parent object containing the placeable object
         * and collider. A child object containing the visual aspects of the vehicle. So, the spriterenderer
         * is in this child object
         */ 
        public GameObject vehiclePrefab;
        public SOVehicleDesigns vehicleDesigns;

        [Header("CreationChecks")] 
        public bool isInfinite = true;
        public int placeableAmount = -1;
        public TextMeshProUGUI placeableAmountText;
        public GameObject cover;
        
        private Transform _icon;
        private int _rotations = 0;

        #region UNITY FUNCTIONS

        private void Start()
        {
            _icon = transform.GetChild(0);
            SetUpTriggerEvents();
            SetUpPlaceableAmount();
        }

        #endregion

        #region PUBLIC FUNCTIONS

        /// <summary>
        /// Updates the rotation with which to create the vehicles
        /// </summary>
        /// <param name="rotations"></param>
        public void UpdateRotation(int rotations)
        {
            _rotations = rotations;
            _icon.Rotate(new Vector3(0,0,1), -90);
        }

        /// <summary>
        /// Adds a placeable amount 
        /// </summary>
        public void AddPlaceableAmount()
        {
            if (placeableAmount == 0)
            {
                cover.SetActive(false);
                GetComponent<EventTrigger>().enabled = true;
            }

            placeableAmount++;
            placeableAmountText.text = placeableAmount.ToString();
        }

        #endregion

        #region PRIVATE FUNCTIONS
        
        /// <summary>
        /// Logic to follow when the gradding starts
        /// </summary>
        private void OnDragBegin()
        {
            if (!vehiclePrefab)
            {
                Debug.LogWarning("No vehicle prefab has been set for this creator: ["+gameObject.name+"].");
                return;
            }
            
            //Set up variables
            var vehicle = Instantiate(vehiclePrefab, Pointer.Instance.transform.position, new Quaternion());
            var placeableObject = vehicle.GetComponent<PlaceableObject>();
            var vehicleSpriteRenderer = vehicle.transform.GetChild(0).GetComponent<SpriteRenderer>();
            
            //Set up creator
            placeableObject.vehicleType = type;
            
            //Set up random vehicle design (sprite)
            int designPos = Random.Range(0, vehicleDesigns.sprites.Length); 
            placeableObject.design = designPos;
            vehicleSpriteRenderer.sprite = GetRandomVehicleDesign(designPos);
            
            //Set up placeable object
            for (int i = 0; i < _rotations; i++)
            {
                placeableObject.Rotate(); 
            }
            
            Pointer.Instance.SetNewDraggingObject(vehicle);
            CheckPlaceableAmount();
        }

        /// <summary>
        /// Gets a random design of the specific vehicle
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private Sprite GetRandomVehicleDesign(int pos)
        {
            if (!vehicleDesigns)
            {
                Debug.LogError($"[{gameObject.name}]: There are no vehicle designs attached to the creator");
                return null;
            }

            return vehicleDesigns.sprites[pos];
        }

        /// <summary>
        /// Checks the placeable amount
        /// </summary>
        private void CheckPlaceableAmount()
        {
            if (isInfinite) return;

            placeableAmount--;
            placeableAmountText.text = placeableAmount.ToString();
            
            if (placeableAmount == 0)
            {
                cover.gameObject.SetActive(true);
                
                //Stop trigger events
                GetComponent<EventTrigger>().enabled = false;
            }
        }

        #endregion

        #region GAMEOBJECT SETUP

        /// <summary>
        /// Sets up the trigger events
        /// </summary>
        private void SetUpTriggerEvents()
        {
            EventTrigger eventTrigger = GetComponent<EventTrigger>();
            if (!eventTrigger)
            {
                eventTrigger = gameObject.AddComponent<EventTrigger>();
            } 
            CreateTriggerEvent(EventTriggerType.BeginDrag, delegate { OnDragBegin(); });
        }

        /// <summary>
        /// Created the triggers of the creator
        /// </summary>
        /// <param name="type">event trigger type</param>
        /// <param name="action">action</param>
        private void CreateTriggerEvent(EventTriggerType type, UnityAction<BaseEventData> action)
        {
            EventTrigger trigger = GetComponent<EventTrigger>();
            var eventTrigger = new EventTrigger.Entry();
            eventTrigger.eventID = type;
            eventTrigger.callback.AddListener(action);
            trigger.triggers.Add(eventTrigger);
        }

        /// <summary>
        /// Sets the visual placeable amount
        /// </summary>
        private void SetUpPlaceableAmount()
        {
            if (isInfinite)
            {
                placeableAmountText.gameObject.SetActive(false);
            }
            else
            {
                placeableAmountText.text = placeableAmount.ToString();
            }
        }

        #endregion
    }
}