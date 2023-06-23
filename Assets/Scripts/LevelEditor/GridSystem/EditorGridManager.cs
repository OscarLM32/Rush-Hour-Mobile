using LevelEditor.GridSystem.ObjectPlacers;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LevelEditor.GridSystem
{
    /// <summary>
    /// Gris manager of the level editing
    /// </summary>
    public class EditorGridManager : GridManager<EditorGridManager>
    {
        public TileBase baseTile; 
        
        [Space]
        public Transform placedObjects;
        public Transform exit;
        
        
        private ObjectPlacerFactory _factory;

        //GRID EDITING (MOVING OBJECTS) ATTRIBUTES
        //'mo' stands for moving object
        private PlaceableObject _movingObject;
        private Vector2 _moPreviousPosition;
        private Vector2 _moPreviousStartingPosition;
        

        public PlaceableObject MovingObject {
            get => _movingObject;
            set => _movingObject = value;
        }
        
        public Vector2 MOPreviousPosition {
            get => _moPreviousPosition;
            set => _moPreviousPosition = value;
        }

        public Vector2 MOPreviousStartingPosition
        {
            get => _moPreviousStartingPosition;
            set => _moPreviousStartingPosition = value;
        }


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

        #region  UTILS

        public LevelRepresentation GetLevelRepresentation()
        {
            var size = levelSize;
            LevelRepresentation lr = new LevelRepresentation(size);
            for (int i = 0; i < placedObjects.transform.childCount; i++)
            {
                var placeableObject = placedObjects.transform.GetChild(i).GetComponent<PlaceableObject>();
                if (!placeableObject) continue;

                Vector2Int cellPos = (Vector2Int)grid.WorldToCell(placeableObject.transform.position);
                lr.data.Add(
                    new PlaceableObjectData
                    (
                        placeableObject.type,
                        cellPos,
                        placeableObject.transform.rotation,
                        placeableObject.size,
                        placeableObject.vehicleType, 
                        placeableObject.design
                    )
                );
            }

            return lr;
        }

        //TODO: This method can be considered deprecated since Unity has it's own "_tilemap.GetTilesBlock(area)"
        public TileBase[] GetTilesBlock(BoundsInt area)
        {
            TileBase[] array = new TileBase[area.size.x * area.size.y];
            int counter = 0;
            
            foreach (var p in area.allPositionsWithin)
            {
                Vector2Int pos = new Vector2Int(p.x, p.y);
                array[counter] = tilemap.GetTile((Vector3Int)pos);
                counter++;
            }
            return array;
        }

        public void ClearGrid()
        {
            
        }

        #endregion

        #region GRID EDITING (MOVING VEHICLES)

        public void TakeObject(PlaceableObject movingObject)
        {
            movingObject.transform.SetParent(transform);
            
            ObjectPlacer placer = _factory.CreatePlacer(movingObject.type);
            placer.TakeObject(movingObject);
        }

        #endregion
        
        #region OBJECT PLACEMENT

        public void Place(PlaceableObject placeableObject)
        {
            placeableObject.transform.SetParent(placedObjects); 
            
            ObjectPlacer placer = _factory.CreatePlacer(placeableObject.type);

            if (!placer.Place(placeableObject))
            {
                if (placeableObject.vehicleType != VehicleType.NONE)
                {
                    LevelEditorEvents.VehicleDestroy?.Invoke(placeableObject.vehicleType);
                }
                Destroy(placeableObject.gameObject);
            }
        }

        public void SetInitialExitPosition()
        {
            //Making the supposition of all tiles being the same size 
            var tileSize = tilemap.cellSize.x;
            var exitInitialPos = new Vector2(tileSize * xGridBounds.max, tileSize * yGridBounds.max-3);
            exitInitialPos += (Vector2)transform.position; //offset
            
            exit.position = SnapCoordinateToGrid(exitInitialPos);
        }

        #endregion

        #region PRIVATE FUNCTIONS

        private void ConfigureInstance()
        {
            Instance = this;
            grid = gridLayout.gameObject.GetComponent<Grid>();
            _factory = new ObjectPlacerFactory(this);
            
            //ConfigurePlaceableGridBounds();
            //SetInitialExitPosition();
        }
        
        //TODO: refactor so both values of each bound are inclusive (will have to change ExitPlacer and some other classes)
        private void ConfigurePlaceableGridBounds()
        {
            var coll = GetComponent<Collider2D>();
            var offset = transform.position;

            xGridBounds.min = gridLayout.WorldToCell(offset + new Vector3(-coll.bounds.extents.x, 0)).x;
            xGridBounds.max = gridLayout.WorldToCell(offset + new Vector3( coll.bounds.extents.x, 0)).x;
            
            yGridBounds.min = gridLayout.WorldToCell(offset + new Vector3(0, -coll.bounds.extents.y)).y;
            yGridBounds.max = gridLayout.WorldToCell(offset + new Vector3(0,  coll.bounds.extents.y)).y;
           
            //Debug.Log($"X vehicle bounds = ({_xGridBounds.min},{_xGridBounds.max})");
            //Debug.Log($"Y vehicle bounds = ({_yGridBounds.min},{_yGridBounds.max})");
        }

        #endregion
    }
}