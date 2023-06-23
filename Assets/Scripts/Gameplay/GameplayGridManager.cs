using System;
using System.Collections;
using CoreSystems.AudioSystem;
using Extensions;
using UnityEngine;
using UnityEngine.Tilemaps;
using AudioType = CoreSystems.AudioSystem.AudioType;

namespace Gameplay
{
    /// <summary>
    /// Specific grid manager that handles the logic of the gris when the user is playing
    /// </summary>
    public class GameplayGridManager : GridManager<GameplayGridManager>
    {
        /// <summary>
        /// Tilemap that will contain the borders on the right side
        /// </summary>
        [Space]
        public Tilemap extraBorders;
        /// <summary>
        /// The tile to fill extraBorders with
        /// </summary>
        public Tile borderTile;
        
        [Space]
        public Transform _vehiclesParent;

        /// <summary>
        /// The layerMask the vehicle can collision with
        /// </summary>
        public LayerMask checkVehicleCollisionWith;

        #region UNITY FUNCTIONS

        private new void Awake()
        {
            if (!Instance)
            {
                ConfigureInstance();
                return;
            }
            Destroy(gameObject);
        } 

        #endregion


        #region PUBLIC FUNCTIONS

        /// <summary>
        /// Check if a vehicle can be moved and moves it in case it can be done
        /// </summary>
        /// <param name="vehicleObj">The vehicle object to move</param>
        /// <param name="direction">The direction in which to move the vehicle</param>
        public void CheckMoveVehicle(GameObject vehicleObj, Vector2 direction)
        {
            var vehicle = vehicleObj.GetComponent<Vehicle>();
            var pos = vehicle.GetFinalPositionMovingTo(direction);

            if (CanVehicleBeMovedTo(vehicleObj, pos))
            {
                MoveVehicle(vehicleObj, pos);
            }
        }

        /// <summary>
        /// Generates the border in extraBorders
        /// </summary>
        /// <param name="exitPos">The position of the exit tile</param>
        public void GenerateBorders(Vector2Int exitPos)
        {
            Debug.Log("Generate borders called");
            //This is done to clear the the tilemap
            extraBorders.ClearAllTiles();
            
            int xPosition = xGridBounds.max;

            int dif = yGridBounds.max - yGridBounds.min;
            Debug.Log("Exit pos -->" + exitPos);
            for (int i = 0; i < dif; i++)
            {
                Vector2Int pos = new Vector2Int(xPosition, yGridBounds.min + i);

                if (pos != exitPos)
                {
                    Debug.Log(pos); 
                    extraBorders.BoxFill((Vector3Int)pos, borderTile, pos.x, pos.y, pos.x, pos.y);
                }
            }
        }

        #endregion
        
        
        #region PRIVATE FUNCTIONS

        /// <summary>
        /// Configures an initial representation of the instance
        /// </summary>
        private void ConfigureInstance()
        {
            Instance = this;
            grid = GetComponent<Grid>();
        }

        /// <summary>
        /// Checks if the vehicle can be moved
        /// </summary>
        /// <param name="vehicleObj">The vehicle to move</param>
        /// <param name="gridPos">The grid position to which move the vehicle</param>
        /// <returns>Whether it can be moved ot not</returns>
        private bool CanVehicleBeMovedTo(GameObject vehicleObj, Vector2 gridPos)
        {
            //Debug.Log("CanVehicleBeMovedBeingCalled");
            //TODO: refactor this method. Split it in different parts and solve the offset problem in a better way
            
            var vehicle = vehicleObj.GetComponent<Vehicle>();
            var offset = vehicleObj.GetComponent<Collider2D>().offset;
            var angle = 0f;
            var size = vehicle.size * 0.95f;
            
            if (vehicle.movableDirection == VehicleDirection.HORIZONTAL)
            {
                angle = 90;
                size = new Vector2(size.y, size.x);
                offset = new Vector2(offset.y, offset.x);
            }

            var vehicleRotation = vehicleObj.transform.rotation.eulerAngles.z; 
            if ( vehicleRotation == 90 || vehicleRotation == 180 )
            {
                offset *= -1;
            }
            
            var boxPos = tilemap.GetCellCenterWorld(gridPos.ToVector3Int()) + (Vector3)offset;
            var collision = Physics2D.OverlapBoxAll(boxPos, size, angle, checkVehicleCollisionWith);
            
            //Debugging purposes
            /*var box = new GameObject().AddComponent<BoxCollider2D>();
            box.transform.position = boxPos;
            box.size = size;
            box.transform.eulerAngles = new Vector3(0, 0, angle);*/
            
            foreach (var c in collision)
            {
                if (c.gameObject != vehicleObj)
                {
                    Debug.Log("I am colliding with something there");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Moves a vehicle and plays the proper sound
        /// </summary>
        /// <param name="vehicle">The vehicle to move</param>
        /// <param name="to">Where to move it to</param>
        private void MoveVehicle(GameObject vehicle, Vector2 to)
        {
            vehicle.transform.position = tilemap.GetCellCenterWorld(to.ToVector3Int());
            var type = vehicle.GetComponent<Vehicle>().type;

            switch (type)
            {
                case VehicleType.TRUCK:
                    AudioController.instance.RestartAudio(AudioType.SFX_TRUCK);
                    break;
                case VehicleType.CAR:
                case VehicleType.PLAYER_VEHICLE:
                    AudioController.instance.RestartAudio(AudioType.SFX_CAR);
                    break;
            }
        }

        #endregion


    }
}