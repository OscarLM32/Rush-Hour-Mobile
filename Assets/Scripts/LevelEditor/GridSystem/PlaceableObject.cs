using System;
using System.Collections;
using LevelEditor.VehicleCreator;
using UnityEngine;
using UnityEngine.Serialization;

namespace LevelEditor.GridSystem
{
    public class PlaceableObject : MonoBehaviour
    {
        public PlaceableObjectType type;
        public VehicleType vehicleType = VehicleType.NONE;
        public Vector2Int size { get; private set; }

        public int design = -1;
        
        private Vector2[] _vertices;

        private bool _isInitializing = true;

        private void Start()
        {
            _isInitializing = true;
            SetUpPlacementVariables();
            _isInitializing = false;
        }

        public void Rotate()
        {
            StartCoroutine(RotateCoroutine());
        }

        private IEnumerator RotateCoroutine()
        {
            while (_isInitializing)
            {
                yield return null;
            }
            
            transform.Rotate(new Vector3(0,0,-90));
            size = new Vector2Int(size.y, size.x);

            Vector2[] vertices = new Vector2[_vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = _vertices[(i+1) % vertices.Length];
            }

            _vertices = vertices; 
        }

        /// <summary>
        /// Start position (in world coordinates) from which the object is "drawn"
        /// </summary>
        /// <returns>Position in world space coordinates</returns>
        public Vector2 GetStartPosition()
        {
            return transform.TransformPoint(_vertices[0]);
        }

        #region INITIALIZING METHODS
        
        private void SetUpPlacementVariables()
        {
            GetColliderVertexPositionsLocal();
            CalculateSizeInCells();
        } 

        private void GetColliderVertexPositionsLocal()
        {
            BoxCollider2D coll = GetComponent<BoxCollider2D>();
            _vertices = new Vector2[4];
            _vertices[0] = coll.offset + new Vector2(-coll.size.x, -coll.size.y) * 0.5f;
            _vertices[1] = coll.offset + new Vector2( coll.size.x, -coll.size.y) * 0.5f ;
            _vertices[2] = coll.offset + new Vector2( coll.size.x,  coll.size.y) * 0.5f ;
            _vertices[3] = coll.offset + new Vector2(-coll.size.x,  coll.size.y) * 0.5f ;
        }
        
        
        
        /*
         * This approach is not working since this method is being called by the "DragBegin" event
         * handled by the vehicles creators. This makes the initial position variable and it makes the
         * function return different values different times, since the functions was thought to do
         * the calculus with the placeable object's position set to Vector2/3.zero
         */
        /* 
        private void CalculateSizeInCells()
        {
            Vector2Int[] vertices = new Vector2Int[_vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                //Debug.Log(_vertices[i]);
                Vector2 worldPos = transform.TransformPoint(_vertices[i]);
                vertices[i] = (Vector2Int)GridManager.Instance.gridLayout.WorldToCell(worldPos);
                //Debug.Log(vertices[i]); 
            }
            size = new Vector2Int( Math.Abs((vertices[0] - vertices[1]).x), Math.Abs((vertices[0] - vertices[3]).y));
            //Debug.Log(size);
        } */

        private void CalculateSizeInCells()
        {
            var cellSize = EditorGridManager.Instance.gridLayout.cellSize;
            
            int xSize = (int)Mathf.Ceil(Math.Abs((_vertices[0] - _vertices[1]).x)/cellSize.x);
            int ySize = (int)Mathf.Ceil(Math.Abs((_vertices[0] - _vertices[3]).y)/cellSize.y);
            size = new Vector2Int(xSize, ySize);
        }

        #endregion

    }
}