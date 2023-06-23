using System;
using LevelEditor;
using UnityEngine;

namespace Gameplay
{
    /// <summary>
    /// Behaviour of the object Exit when a vehicle is placed on top of it
    /// </summary>
    public class Exit : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D other)
        {
            var vehicle = other.gameObject.GetComponent<Vehicle>();
            
            if (!vehicle) return;
            
            if (vehicle.type == VehicleType.PLAYER_VEHICLE)
            {
                LevelEditorEvents.EditedLevelCompleted?.Invoke();
                GameplayEvents.LevelCompleted?.Invoke();
            }
            else
            {
                LevelEditorEvents.EditedLevelNotCompletable?.Invoke();
            }
        }
    }
}