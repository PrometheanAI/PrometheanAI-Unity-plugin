using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEditor;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    //Called by command "build *object*"
    //Raw incoming command example : raytrace_from_camera

    /// <summary>
    /// Used to get the location in front of SceneView camera
    /// </summary>
    public class RaytraceFromCamera : ICommand
    {
        public string GetToken => "raytrace_from_camera";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            var sceneView = SceneView.lastActiveSceneView;
            var sceneViewCamera = sceneView.camera;
            var cameraDirection = sceneViewCamera.transform.forward;
            var cameraPosition = sceneViewCamera.transform.position;
            var hitPoint = cameraPosition + cameraDirection * 10;
            if (Physics.Raycast(cameraPosition, cameraDirection, out var hit)) {
                hitPoint = hit.point;
            }

            hitPoint = CommandUtility.ConvertToCentimetersVector(hitPoint);
            var hitData = new[] {
                CommandUtility.Round(hitPoint.x, 4),
                CommandUtility.Round(hitPoint.y, 4),
                CommandUtility.Round(hitPoint.z, 4)
            };
            var outPutData = new Dictionary<string, object> {{"hit_location", hitData}};
            var output = JsonOutputFormatUtility.GenerateJsonString(outPutData);
            callback.Invoke(CommandHandleProcessState.Success, output);
        }
    }
}