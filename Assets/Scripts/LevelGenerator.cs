using System;
using System.Collections;
using System.Collections.Generic;
using CoreSystems;
using Gameplay;
using LevelEditor.GridSystem;
using LevelEditor.VehicleCreator;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Generates a level from a level representation
/// </summary>
public class LevelGenerator : SingletonScene<LevelGenerator>
{
    public GameObject exitPrefab;
    
    [Header("Vehicle placing")]
    [Tooltip("Expresses the order in which the playable vehicle prefabs and vehicle designs are going to be inserted")]
    public VehicleType[] typesOrder;
    public GameObject[] playableVehiclePrefabs;
    public SOVehicleDesigns[] vehicleDesigns;

    private readonly Dictionary<VehicleType, GameObject> _playableVehiclesTable = new ();

    private readonly Dictionary<VehicleType, SOVehicleDesigns> _vehicleDesignsTable = new ();

    private new void Awake()
    {
        base.Awake();
        for (int i = 0; i < typesOrder.Length; i++)
        {
            _playableVehiclesTable.Add(typesOrder[i], playableVehiclePrefabs[i]);
            _vehicleDesignsTable.Add(typesOrder[i], vehicleDesigns[i]);
        }  
    }

    /// <summary>
    /// Clears all the objects in the level
    /// </summary>
    public void ClearLevel()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }


    public void GenerateLevelFromJson(string json)
    {
        var level = JsonConvert.DeserializeObject<LevelRepresentation>(json);
        GenerateLevelFromLeveLRepresentation(level);
    }

    /// <summary>
    /// Generates the level from a level representation
    /// </summary>
    /// <param name="level">The level representation</param>
    //TODO: this should be refactored to a factory pattern
    public void GenerateLevelFromLeveLRepresentation(LevelRepresentation level)
    {
        GameplayGridManager.Instance.SetSize(level.levelSize);
        foreach (var obj in level.data)
        {
            GenerateObject(obj.type, obj.cellPos,obj.rotation, obj.size, obj.vehicleType, obj.design);
        }
    }

    /// <summary>
    /// Handles the generation of all the objects
    /// </summary>
    /// <param name="type">The placeableobject type</param>
    /// <param name="cellPos">the cell position</param>
    /// <param name="rotation">rotation</param>
    /// <param name="size">size in tiles</param>
    /// <param name="vehicleType">the vehicle type (if it is a vehicle)</param>
    /// <param name="design">the vehicle design (if it has any)</param>
    private void GenerateObject(PlaceableObjectType type, Vector2Int cellPos, Quaternion rotation, Vector2 size, VehicleType vehicleType , int design)
    {
        switch(type)
        {
            case PlaceableObjectType.EXIT:
                GenerateExit(cellPos);
                break; 
            case PlaceableObjectType.VEHICLE:
                GenerateVehicle(cellPos, rotation, size, vehicleType, design);
                break;
            default:
                Debug.LogError("[LevelGenerator]: I have received an object of type NONE");
                return;
        }
    }

    /// <summary>
    /// Generates and places the exit
    /// </summary>
    /// <param name="cellPos"></param>
    private void GenerateExit(Vector2Int cellPos)
    {
        var worldCellPos = GameplayGridManager.Instance.gridLayout.CellToWorld((Vector3Int)cellPos);
        var pos = GameplayGridManager.Instance.SnapCoordinateToGrid(worldCellPos);
        Instantiate(exitPrefab, pos, new Quaternion(), transform);
        (GameplayGridManager.Instance as GameplayGridManager).GenerateBorders(cellPos);
    }

    /// <summary>
    /// Generates vehicles
    /// </summary>
    /// <param name="cellPos"></param>
    /// <param name="rotation"></param>
    /// <param name="size"></param>
    /// <param name="type"></param>
    /// <param name="design"></param>
    private void GenerateVehicle(Vector2Int cellPos, Quaternion rotation, Vector2 size, VehicleType type, int design)
    {
        if (!_playableVehiclesTable.ContainsKey(type))
        {
            return;
        }
        
        var worldCellPos = GameplayGridManager.Instance.gridLayout.CellToWorld((Vector3Int)cellPos);
        var pos = GameplayGridManager.Instance.SnapCoordinateToGrid(worldCellPos);
        var obj = Instantiate(_playableVehiclesTable[type], pos, rotation, transform);

        var vehicle = obj.GetComponent<Vehicle>();

        var designSprite = (_vehicleDesignsTable[type])?.sprites[design];
        vehicle.SetVehicleUp(type ,size, designSprite);
    }
}