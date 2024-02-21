using System;
using System.Collections.Generic;
using System.Linq;
using PrometheanAI.Modules.Utils;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Sends to Promethean string of Names  of all GameObjects currently  visible in Scene view
    /// </summary>
    public class GetVisibleActors : ICommand
    {
        public string GetToken => "get_visible_actors";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            var sceneView = SceneView.lastActiveSceneView;
            var currentScene = SceneManager.GetActiveScene();
            var allSceneObjects = currentScene.GetRootGameObjects().ToList();
            allSceneObjects = CommandUtility.AddChildObjects(allSceneObjects);
            var sceneViewCamera = sceneView.camera;
            var rect = sceneViewCamera.pixelRect;
            var visibleObjects = CommandUtility.GetVisibleObjects(allSceneObjects, sceneViewCamera, rect);
            var names = CommandUtility.GetNames(visibleObjects);
            var output = CommandUtility.StringArrayToString(names);
            callback.Invoke(CommandHandleProcessState.Success, output);
        }
    }
}