using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor.VehicleCreator
{
    /// <summary>
    /// Manages the vehiclo creators
    /// </summary>
    public class VehicleCreatorsManager : MonoBehaviour
    {
        //The amount of times the vehicles have to be rotated -90 degrees in the z axis
        private int _rotationsCounter = 0;

        private Dictionary<VehicleType, VehicleCreator> _creators = new Dictionary<VehicleType, VehicleCreator>(); //Relation between vehiclecreatortype and vehicleCreator

        #region UNITY FUNCTIONS

        private void Start()
        {
            PopulateVehicleCreators();
        }

        private void OnEnable()
        {
            LevelEditorEvents.VehicleDestroy += OnVehicleDestroy;
        }

        private void OnDisable()
        {
            LevelEditorEvents.VehicleDestroy -= OnVehicleDestroy; 
        }

        #endregion

        #region PUBLIC FUNCTION

        /// <summary>
        /// Adds a creation rotation to the vehicle creators
        /// </summary>
        public void AddVehicleCreationRotation()
        {
            _rotationsCounter = (_rotationsCounter + 1) % 4;
            foreach (var creator in _creators)
            {
                creator.Value.UpdateRotation(_rotationsCounter);
            }
        }

        #endregion

        #region PRIVATE FUNCTIONS

        /// <summary>
        /// Populates the vehicle creators table
        /// </summary>
        private void PopulateVehicleCreators()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var creator = transform.GetChild(i).GetComponent<VehicleCreator>();
                if (creator)
                {
                    if (_creators.ContainsKey(creator.type))
                    {
                        Debug.LogWarning("There can only be one creator of each creator type");
                        continue;
                    }
                    _creators.Add(creator.type, creator);
                }
            } 
        }

        /// <summary>
        /// Handles the logic when a vehicle is destroyed
        /// </summary>
        /// <param name="creatorType">The creator that created the vehicle</param>
        private void OnVehicleDestroy(VehicleType creatorType)
        {
            if (!_creators.ContainsKey(creatorType)) return;

            var creator = _creators[creatorType] as VehicleCreator;
            if (creator != null) creator.AddPlaceableAmount();
        }

        #endregion
    }
}