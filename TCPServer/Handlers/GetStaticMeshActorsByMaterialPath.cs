using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;

namespace PrometheanAI.Modules.TCPServer
{
    /// <summary>
    /// Sends to Promethean string of Names  of  GameObjects with specified materials
    /// </summary>
    public class GetStaticMeshActorsByMaterialPath : ICommand
    {
        public string GetToken => "get_static_mesh_actors_by_material_path";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var materialPath = commandParametersString[0];
            var allSceneObjects = CommandUtility.GetAllValidObjectsFromScene();
            allSceneObjects = CommandUtility.SortObjectByMaterial(allSceneObjects, materialPath);
            var names = CommandUtility.GetNames(allSceneObjects);
            var output = CommandUtility.StringArrayToString(names);
            callback.Invoke(CommandHandleProcessState.Success, output);
        }
    }
}