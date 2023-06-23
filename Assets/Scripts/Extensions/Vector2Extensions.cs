using System;
using UnityEngine;

namespace Extensions
{
    /// <summary>
    /// Extension methods for the Vector2 class
    /// </summary>
    public static class Vector2Extensions
    {
        public static void Abs(this Vector2 input)
        {
            input.x = Math.Abs(input.x);
            input.y = Math.Abs(input.y);
        }

        public static Vector3Int ToVector3Int(this Vector2 input)
        {
            return Vector3Int.RoundToInt(input);
        }
    }
}