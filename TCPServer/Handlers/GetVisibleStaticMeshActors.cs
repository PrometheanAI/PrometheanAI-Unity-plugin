using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEditor;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Sends to Promethean string of Names  of all valid for Promethean GameObjects currently  visible in Scene view
    /// </summary>
    public class GetVisibleStaticMeshActors : ICommand
    {
        public string GetToken => "get_visible_static_mesh_actors";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            var sceneView = SceneView.lastActiveSceneView;
            var allSceneObjects = CommandUtility.GetAllValidObjectsFromScene();
            var sceneViewCamera = sceneView.camera;
            var rect = sceneViewCamera.pixelRect;
            var visibleObjects = CommandUtility.GetVisibleObjects(allSceneObjects, sceneViewCamera, rect);
            var names = CommandUtility.GetNames(visibleObjects);
            var output = CommandUtility.StringArrayToString(names);
            callback.Invoke(CommandHandleProcessState.Success, output);
        }
    }
}