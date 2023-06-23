using System;
using Extensions;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

namespace Gameplay
{
    /// <summary>
    /// Class that handles the logic of the vehicles during gameplay
    /// </summary>
    public class Vehicle : MonoBehaviour
    {
        public VehicleType type;
        
        /// <summary>
        /// The number of tiles it occupies in each axis
        /// </summary>
        public Vector2 size;
        //TODO: it may be useless, check if it can be deleted
        /// <summary>
        /// The direction in which it can only move
        /// </summary>
        public VehicleDirection movableDirection = VehicleDirection.VERTICAL;
        /// <summary>
        /// The Vector2 representation of the movableDirection
        /// </summary>
        private Vector2 _directionMultiplier = Vector2.up;

        /// <summary>
        /// Sets up the vehicle
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="size">Size</param>
        /// <param name="design">Sprite</param>
        public void SetVehicleUp(VehicleType type, Vector2 size, Sprite design)
        {
            this.type = type;
            this.size = size;
            
            var zAngles = (int)transform.rotation.eulerAngles.z;
            zAngles = zAngles > 180 ? zAngles - 360 : zAngles;
            
            if (Math.Abs(zAngles) == 90)
            {
                movableDirection = VehicleDirection.HORIZONTAL;
                _directionMultiplier = Vector2.right;
            }
            
            var spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = design;
        }

        /// <summary>
        /// Returns the position the vehicle is going to end up in if following the input
        /// </summary>
        /// <param name="direction">Input direction</param>
        /// <returns></returns>
        public Vector2 GetFinalPositionMovingTo(Vector2 direction)
        {
            var dir = direction * _directionMultiplier;
            if (dir == Vector2.zero)
            {
                return Vector2.zero;
            }

            //TODO: look for a decent way to convert from Vector3Int to Vector2
            Vector3 currentGridPos =
                (GameplayGridManager.Instance as GameplayGridManager).tilemap.WorldToCell(transform.position);
            var finalGridPos = (Vector2)currentGridPos + dir;
            //Debug.Log(finalGridPos);
            return finalGridPos;
        }
    }
}