using UnityEngine;
using UnityEngine.Tilemaps;

namespace LevelEditor.GridSystem.ObjectPlacers
{
    /// <summary>
    /// Abstract class that defines the structure of the placers that handle the logic behind placing
    /// and moving placeableObjects in the grid
    /// </summary>
    public abstract class ObjectPlacer
    {
        protected EditorGridManager Context;

        protected ObjectPlacer(EditorGridManager context)
        {
            Context = context;
        }
        
        #region PLACING METHODS
        
        public abstract bool Place(PlaceableObject placeableObject);
        protected abstract bool CanBePlaced(PlaceableObject placeableObject);

        protected void TakeArea(Vector2Int start, Vector2Int size)
        {
            Context.tilemap.BoxFill((Vector3Int)start, Context.baseTile, start.x, start.y, start.x + size.x-1, start.y + size.y-1);
            //This always the last step in placing an object
            LevelEditorEvents.LevelEdited?.Invoke();
        }
        
        #endregion

        #region MOVING METHODS

        /// <summary>
        /// Takes a placed object from the grid
        /// </summary>
        /// <param name="placeableObject">The placeableObject being taken</param>
        public void TakeObject(PlaceableObject placeableObject)
        {
            Context.MovingObject = placeableObject;
            Context.MOPreviousPosition = placeableObject.transform.position;
            Context.MOPreviousStartingPosition = placeableObject.GetStartPosition();
            
            var start = Context.gridLayout.WorldToCell(Context.MOPreviousStartingPosition);
            RemoveTakenArea((Vector2Int)start, Context.MovingObject.size); 
        }

        /// <summary>
        /// Removes the taken area by the object
        /// </summary>
        /// <param name="start">Start position</param>
        /// <param name="size">size</param>
        private void RemoveTakenArea(Vector2Int start, Vector2Int size)
        {
            Context.tilemap.BoxFill(
                (Vector3Int)start, 
                null,
                start.x, 
                start.y, 
                start.x + size.x-1, 
                start.y + size.y-1
            );
        } 
        
        /// <summary>
        /// Returns an object to the position it was taken from if it cannot be placed back
        /// </summary>
        protected void ReturnObjectToPreviousPosition()
        {
            Context.MovingObject.transform.position = Context.MOPreviousPosition;

            var start = Context.gridLayout.WorldToCell(Context.MOPreviousStartingPosition); 
            TakeArea((Vector2Int)start, Context.MovingObject.size);
            
            DisposeMovingObjectInformation(); 
        }
        
        /// <summary>
        /// Destroys a placeableobject
        /// </summary>
        private void DisposeMovingObjectInformation()
        {
            Context.MovingObject = null;
            Context.MOPreviousPosition = Context.MOPreviousStartingPosition = Vector2.zero;
        }  

        #endregion
        
    }
}