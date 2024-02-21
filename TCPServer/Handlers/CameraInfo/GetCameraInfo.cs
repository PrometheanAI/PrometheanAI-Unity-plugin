using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEditor;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    //Called by command "face me", "move to me"
    //Raw incoming command example : get_camera_info
    /// <summary>
    /// Sends to Promethean data about Scene View camera
    /// </summary>
    public class GetCameraInfo : ICommand
    {
        public string GetToken => "get_camera_info";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            var sceneView = SceneView.lastActiveSceneView;
            var sceneCamera = sceneView.camera;
            var sceneViewCamera = sceneView.camera;
            var cameraTransform = sceneCamera.transform;
            var rect = sceneViewCamera.pixelRect;
            var allSceneObjects = CommandUtility.GetAllValidObjectsFromScene();
            var visibleObjects = CommandUtility.GetVisibleObjects(allSceneObjects, sceneViewCamera, rect);
            var names = CommandUtility.GetNames(visibleObjects);
            var cameraPos = cameraTransform.position;
            var cameraDirection = cameraTransform.forward;
            var fieldOfView = Convert.ToInt32(sceneCamera.fieldOfView);
            var namesString = CommandUtility.StringArrayToString(names);
            var data = new CameraInfo(cameraPos, cameraDirection, fieldOfView, namesString);
            var output = JsonOutputFormatUtility.GenerateJsonString(data);
            callback.Invoke(CommandHandleProcessState.Success, output);
        }
    }
}