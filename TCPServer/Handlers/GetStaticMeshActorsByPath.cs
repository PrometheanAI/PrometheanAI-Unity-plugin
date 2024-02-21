using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;

namespace PrometheanAI.Modules.TCPServer
{
    /// <summary>
    /// Sends to Promethean string of Names  of  GameObjects currently in Scene with specified path
    /// </summary>
    public class GetStaticMeshActorsByPath : ICommand
    {
        public string GetToken => "get_static_mesh_actors_by_path";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var path = commandParametersString[0];
            var allSceneObjects = CommandUtility.GetAllValidObjectsFromScene();
            allSceneObjects = CommandUtility.SortGameObjectsByPath(path, allSceneObjects);
            var names = CommandUtility.GetNames(allSceneObjects);
            string output;
            if (names.Count == 0) {
                output = JsonOutputFormatUtility.GenerateJsonString("None");
            }
            else {
                var namesString = CommandUtility.StringArrayToString(names);
                output = JsonOutputFormatUtility.GenerateJsonString(namesString);
            }

            callback.Invoke(CommandHandleProcessState.Success, output);
        }
    }
}