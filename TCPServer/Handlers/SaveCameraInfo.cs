using System;
using System.Collections.Generic;
using System.IO;
using PrometheanAI.Modules.Utils;
using UnityEditor;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    //Called by command "learn"
    //Raw incoming command example : save_camera_info C:/Users/UserName/Documents/PrometheanAI/temp/c_cache

    /// <summary>
    /// Used to save SceneView camera's transform data to a local file
    /// </summary>
    public class SaveCameraInfo : ICommand
    {
        public string GetToken => "save_camera_info";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var sceneView = SceneView.lastActiveSceneView;
            var sceneCamera = sceneView.camera;
            var allSceneObjects = CommandUtility.GetAllValidObjectsFromScene();
            var sceneViewCamera = sceneView.camera;
            var cameraTransform = sceneCamera.transform;
            var rect = sceneViewCamera.pixelRect;
            var visibleObjects = CommandUtility.GetVisibleObjects(allSceneObjects, sceneViewCamera, rect);
            var names = CommandUtility.GetNames(visibleObjects);
            var cameraPos = cameraTransform.position;
            var cameraDirection = cameraTransform.forward;
            var fieldOfView = Convert.ToInt32(sceneCamera.fieldOfView);
            var namesString = CommandUtility.StringArrayToString(names);
            var data = new CameraInfo(cameraPos, cameraDirection, fieldOfView, namesString);
            var output = JsonOutputFormatUtility.GenerateJsonString(data);
            var path = commandParametersString[0];
            File.Delete(path);
            var writer = new StreamWriter(path, true);
            writer.WriteLine(output);
            writer.Close();
            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}