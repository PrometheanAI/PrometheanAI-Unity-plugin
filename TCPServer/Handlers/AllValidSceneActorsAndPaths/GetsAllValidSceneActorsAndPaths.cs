using System;
using System.Collections.Generic;
using System.Linq;
using PrometheanAI.Modules.Utils;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    //called by "unhide" to gather info
    //Raw incoming command example : get_all_valid_scene_actors_and_paths
    /// <summary>
    /// Send data to Promethean about all valid objects in Scene with corresponding paths to those objects
    /// </summary>
    public class GetsAllValidSceneActorsAndPaths : ICommand
    {
        public string GetToken => "get_all_valid_scene_actors_and_paths";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            // Finding all Valid scene objects,including disabled one for "Unhide" operation
            var allSceneObjects = Resources.FindObjectsOfTypeAll<GameObject>().ToList();
            var sortedObject = new List<GameObject>();
            foreach (var obj in allSceneObjects) {
                if (obj.scene.IsValid()) {
                    sortedObject.Add(obj);
                }
            }

            allSceneObjects = CommandUtility.ValidateObjects(sortedObject);
            var names = CommandUtility.GetNames(allSceneObjects);
            var paths = CommandUtility.GetPaths(allSceneObjects);
            var data = new AllValidSceneActorsAndPaths(names, paths);
            var output = JsonOutputFormatUtility.GenerateJsonString(data);
            callback.Invoke(CommandHandleProcessState.Success, output);
        }
    }
}