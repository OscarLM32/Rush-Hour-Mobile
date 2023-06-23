using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LevelEditor.GridSystem.ObjectPlacers
{
    /// <summary>
    /// Factory that creates the specific object placers imposed by the editor grid manager 
    /// </summary>
    public class ObjectPlacerFactory
    {
        private readonly EditorGridManager _context;
        
        public ObjectPlacerFactory(EditorGridManager context)
        {
            _context = context;
        }
        
        public ObjectPlacer CreatePlacer(PlaceableObjectType type)
        {
            switch (type)
            {
                case PlaceableObjectType.VEHICLE:
                    return VehiclePlacer();
                case PlaceableObjectType.EXIT:
                    return ExitPlacer();
                default:
                    Debug.LogError("[ObjectPlacerFactory]: A placer of that type cannot be created");
                    return null;
            }
        } 
        
        private ObjectPlacer VehiclePlacer()
        {
            return new VehiclePlacer(_context);
        }

        private ObjectPlacer ExitPlacer()
        {
            return new ExitPlacer(_context);
        }
        
    }
}