using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Sends to Promethean string of Names  of selected GameObjects 
    /// </summary>
    public class GetSelectedStaticMeshActors : ICommand
    {
        public string GetToken => "get_selected_static_mesh_actors";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            var selectedObjects = CommandUtility.GetSelectedObjects();
            selectedObjects = CommandUtility.ValidateObjects(selectedObjects);
            var names = CommandUtility.GetNames(selectedObjects);
            var output = CommandUtility.StringArrayToString(names);
            callback.Invoke(CommandHandleProcessState.Success, output);
        }
    }
}