using System;

namespace LevelEditor
{
    /// <summary>
    /// Events that can occur during level edition
    /// </summary>
    public static class LevelEditorEvents
    {
        public static Action LevelEdited;
        public static Action<VehicleType> VehicleDestroy;
        public static Action EditedLevelCompleted;
        public static Action EditedLevelNotCompletable;
    }
}