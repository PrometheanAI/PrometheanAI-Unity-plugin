using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Gets hold of the names of all GameObjects in the Scene which are valid for Promethean
    /// </summary>
    public class GetAllValidSceneActors : ICommand
    {
        public string GetToken => "get_all_valid_scene_actors";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            var allSceneObjects = CommandUtility.GetAllValidObjectsFromScene();
            var names = CommandUtility.GetNames(allSceneObjects).ToArray();
            var output = JsonOutputFormatUtility.GenerateJsonString(names);
            callback.Invoke(CommandHandleProcessState.Success, output);
        }
    }
}