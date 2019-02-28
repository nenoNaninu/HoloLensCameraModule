using UnityEngine;

namespace HoloLensCameraModule
{
    public class DetectedObjectData3D
    {
        public Vector3 WorldPosition { get; set; }
        public DetectedObjectData2D DetectedObjectData2D { get; set; }
        public string ClassLabel { get; set; }

        public DetectedObjectData3D(Vector3 worldPos, DetectedObjectData2D detectedObjectData2D)
        {
            WorldPosition = worldPos;
            DetectedObjectData2D = detectedObjectData2D;
            ClassLabel = detectedObjectData2D.ClassLabel;
        }
    }
}