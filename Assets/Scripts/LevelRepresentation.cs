using System.Collections.Generic;
using LevelEditor.GridSystem;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Class representation of the data of the different objects in a level
/// </summary>

public class LevelRepresentation
{
        public LevelSize levelSize;
        public List<PlaceableObjectData> data;

        public LevelRepresentation(LevelSize levelSize)
        {
                this.levelSize = levelSize;
                data = new List<PlaceableObjectData>();
        }

        public string GetJsonRepresentation()
        {
                return JsonConvert.SerializeObject(this);
        }
}

/// <summary>
/// The necessary data to represent an object in the level
/// </summary>
public class PlaceableObjectData
{
        public PlaceableObjectType type;
        public Vector2Int cellPos;
        public Quaternion rotation;
        public Vector2 size;

        public VehicleType vehicleType;
        public int design;
        

        public PlaceableObjectData(PlaceableObjectType type, Vector2Int cellPos, Quaternion rotation, Vector2 size, VehicleType vehicleType = VehicleType.NONE , int design = -1)
        {
                this.type = type;
                this.cellPos = cellPos;
                this.rotation = rotation;
                this.size = size;

                this.vehicleType = vehicleType;
                this.design = design;
        }
}