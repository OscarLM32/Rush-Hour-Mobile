using System;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LevelEditor.GridSystem.ObjectPlacers
{
    /// <summary>
    /// Specific placer for the exit object
    /// </summary>
    public class ExitPlacer : ObjectPlacer
    {
        public ExitPlacer(EditorGridManager context) 
            : base(context)
        {
        }  

        public override bool Place(PlaceableObject exit)
        {

            if (!CanBePlaced(exit))
            {
                ReturnObjectToPreviousPosition();
                return true;
            }
            
            exit.transform.position = Context.SnapCoordinateToGrid(exit.transform.position);
            var start = Context.gridLayout.WorldToCell(exit.GetStartPosition());
            TakeArea((Vector2Int)start, exit.size);
            return true;
        }

        protected override bool CanBePlaced(PlaceableObject exit)
        {
            exit.transform.position = Context.SnapCoordinateToGrid(exit.transform.position);
            var gridPos = (Vector2Int)Context.gridLayout.WorldToCell(exit.transform.position);
            return ExitInBoundsRingRight(gridPos) && !ExitInCorner(gridPos);
        }

        /// <summary>
        /// The exit can only be placed in the right side of the grid outside the vehicles grid
        /// </summary>
        /// <param name="gridPos">The position it which it is being tried to place</param>
        /// <returns></returns>
        private bool ExitInBoundsRingRight(Vector2Int gridPos)
        {
            return gridPos.x == Context.XGridBounds.max;
            
            /*if (gridPos.x < Context.XGridBounds.min - 1 || gridPos.x > Context.XGridBounds.max ||
                gridPos.y < Context.YGridBounds.min - 1 || gridPos.y > Context.YGridBounds.max)
            {
                Debug.Log("Exit not in ring");
                return false;
            }
            Debug.Log("Exit in ring");
            return true;*/
        }

        /// <summary>
        /// Checks if the exit is being placed in a corner
        /// </summary>
        /// <param name="gridPos">The position in which it is being placed</param>
        /// <returns></returns>
        private bool ExitInCorner(Vector2Int gridPos)
        {
            if ((gridPos.x == Context.XGridBounds.max || gridPos.x == Context.XGridBounds.min - 1) &&
                (gridPos.y == Context.YGridBounds.max || gridPos.y == Context.YGridBounds.min - 1))
            {
                Debug.Log("Exit in corner");
                return true;
            }
            Debug.Log("Exit not in corner");
            return false;
        }
    }
}