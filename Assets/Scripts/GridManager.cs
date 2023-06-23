using System;
using CoreSystems;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Abstract class defining the structure of the different grid managers
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class GridManager<T> : SingletonScene<GridManager<T>>
{
    [Header("Grid attributes")] 
    public LevelSize levelSize = LevelSize.SMALL;
    public SOLevelVisuals levelVisuals;
    
    public GridLayout gridLayout;
    protected Grid grid; 
    public Tilemap tilemap; 
    
    //Bounds are min inclusive max exclusive --> [min,max)
    protected Bounds xGridBounds;
    //TODO: all levels are going to be square shaped, there is no need to have yGridBounds
    protected Bounds yGridBounds;
    
    public Bounds XGridBounds => xGridBounds;
    public Bounds YGridBounds => yGridBounds;
    
    //TODO: dependency
    [HideInInspector] public GameObject visualObject; 

    /// <summary>
    /// Sets the size of the grid and the bounds
    /// </summary>
    /// <param name="size"></param>
    public void SetSize(LevelSize size)
    {
        levelSize = size;
        SetBoundsSize();
        visualObject = Instantiate(levelVisuals.visuals[(int) size], transform);
    }
    
    /// <summary>
    /// Snaps a coordinate to a grid cell
    /// </summary>
    /// <param name="position">The world position</param>
    /// <returns></returns>
    public Vector2 SnapCoordinateToGrid(Vector2 position)
    {
        Vector3Int cellPos = gridLayout.WorldToCell(position);
        position = grid.GetCellCenterWorld(cellPos);
        return position;
    }

    /// <summary>
    /// Sets the size of the bounds
    /// </summary>
    //TODO: find A way to calculate it automatically
    private void SetBoundsSize()
    {
        switch (levelSize)
        {
            case LevelSize.SMALL:
                xGridBounds = new Bounds(-3, 3);
                yGridBounds = new Bounds(-3, 3);
                break;
            case LevelSize.MEDIUM:
                xGridBounds = new Bounds(-4, 4);
                yGridBounds = new Bounds(-4, 4); 
                break;
            case LevelSize.BIG:
                xGridBounds = new Bounds(-5, 5);
                yGridBounds = new Bounds(-5, 5); 
                break;
        }
    }
    
    /// <summary>
    /// Bounds structure
    /// </summary>
    public struct Bounds
    {
        public int min;
        public int max;

        public Bounds(int min, int max)
        {
            this.min = min;
            this.max = max;
        }
    } 
}