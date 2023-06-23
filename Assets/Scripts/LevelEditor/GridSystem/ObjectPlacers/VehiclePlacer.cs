using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LevelEditor.GridSystem.ObjectPlacers
{
    /// <summary>
    /// Specific placer class of the vehicles
    /// </summary>
    public class VehiclePlacer : ObjectPlacer
    {
        private bool _vehicleInsideBounds;
        
        public VehiclePlacer(EditorGridManager context) 
            : base(context)
        {
        } 
        
        public override bool Place(PlaceableObject placeableObject)
        {
            if (!placeableObject)
            {
                Debug.LogWarning("You are trying to place a null object");
                return false;
            }
            
            placeableObject.transform.position = Context.SnapCoordinateToGrid(placeableObject.transform.position);
        
            if (!CanBePlaced(placeableObject))
            {
                if (placeableObject == Context.MovingObject && _vehicleInsideBounds)
                {
                    ReturnObjectToPreviousPosition();
                    return true;
                }
                return false;
            }
            
            var start = Context.gridLayout.WorldToCell(placeableObject.GetStartPosition());
            TakeArea((Vector2Int)start, placeableObject.size);
            return true;
        }

        protected override bool CanBePlaced(PlaceableObject placeableObject)
        {
            BoundsInt area = new BoundsInt
            {
                position = Context.gridLayout.WorldToCell(placeableObject.GetStartPosition()),
                size = (Vector3Int)placeableObject.size + new Vector3Int(0,0,1)
            };

            TileBase[] baseArray = Context.GetTilesBlock(area);

            _vehicleInsideBounds = IsPlaceableObjectInsideGridBound(area);
            
            return _vehicleInsideBounds && !IsPlaceableObjectOverOtherObject(baseArray); 
        }
        
        /// <summary>
        /// Checks if the vehicle is being placed inside the bounds of grid
        /// </summary>
        /// <param name="area">The area the vehicle is going to take</param>
        /// <returns></returns>
        private bool IsPlaceableObjectInsideGridBound(BoundsInt area)
        {
            if (area.x < Context.XGridBounds.min || area.xMax > Context.XGridBounds.max ||
                area.y < Context.YGridBounds.min || area.yMax > Context.YGridBounds.max)
            {
                Debug.Log("It is outside grid bound");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if the vehicle is being placed over other vehicle
        /// </summary>
        /// <param name="tiles">The tiles the object is going to occupy</param>
        /// <returns></returns>
        private bool IsPlaceableObjectOverOtherObject(TileBase[] tiles)
        {
            foreach (var tile in tiles)
            {
                if (tile == Context.baseTile)
                {
                    return true;
                }
            }
            return false; 
        }

        [Obsolete]
        private bool IsPlacedInsideBounds(PlaceableObject placeableObject)
        {
            Vector2 pos = placeableObject.transform.position;
            Vector2 gridPos = Context.SnapCoordinateToGrid(pos);

            Debug.Log(gridPos);
            
            var xBounds = Context.XGridBounds;
            var yBounds = Context.YGridBounds;

            //Bounds are min inclusive max exclusive --> [min, max)
            if (gridPos.x < xBounds.min || gridPos.x >= xBounds.max ||
                gridPos.y < yBounds.min || gridPos.y >= yBounds.max)
            {
                return false;
            }
            
            return true;
        }


    }
}