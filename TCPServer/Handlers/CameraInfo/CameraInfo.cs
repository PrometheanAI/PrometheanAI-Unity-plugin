using PrometheanAI.Modules.Utils;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Data container for GetCameraInfo and SaveCameraInfo Handlers
    /// </summary>
    [System.Serializable]
    public class CameraInfo
    {
        public float[] camera_location;
        public float[] camera_direction;
        public int camera_fov;
        public string objects_on_screen;

        public CameraInfo(Vector3 cameraPosition, Vector3 cameraDirection, int fieldOfView, string objectsNames) {
            camera_location = new[] {
                CommandUtility.Round(CommandUtility.ConvertToCentimeters(cameraPosition.x), 4),
                CommandUtility.Round(CommandUtility.ConvertToCentimeters(cameraPosition.y), 4),
                CommandUtility.Round(CommandUtility.ConvertToCentimeters(cameraPosition.z), 4),
            };
            camera_direction = new[] {
                CommandUtility.Round(cameraDirection.x, 4),
                CommandUtility.Round(cameraDirection.y, 4),
                CommandUtility.Round(cameraDirection.z, 4),
            };
            camera_fov = fieldOfView;
            objects_on_screen = objectsNames;
        }
    }
}