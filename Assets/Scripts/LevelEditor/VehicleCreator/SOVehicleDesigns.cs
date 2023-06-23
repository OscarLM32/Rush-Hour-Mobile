using UnityEngine;

namespace LevelEditor.VehicleCreator
{
    /// <summary>
    /// A list of sprites for a vehicle
    /// </summary>
    [CreateAssetMenu(menuName = "LevelEditor/SOVehicleDesigns", fileName = "VehicleDesigns")]
    public class SOVehicleDesigns : ScriptableObject
    {
        public Sprite[] sprites;
    }
}