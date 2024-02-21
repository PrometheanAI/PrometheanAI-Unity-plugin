using System;
using System.Collections.Generic;
using UnityEditor;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    //Called by command "undo"
    //Raw incoming command example "undo"
    
    /// <summary>
    /// Triggers Undo on last recorded action
    /// </summary>
    public class UndoHandler : ICommand
    {
        public string GetToken => "undo";

        public void Handle(string rawCommandData, List<string> commandParametersString, Action<CommandHandleProcessState, string> callback) {
            EditorApplication.ExecuteMenuItem("Edit/Undo");
            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}