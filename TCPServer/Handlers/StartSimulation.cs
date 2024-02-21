using System;
using System.Collections.Generic;
using UnityEditor;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    //Called by command "simulate"
    //Raw incoming command example : start_simulation

    /// <summary>
    /// Used to enter a Play Mode
    /// </summary>
    public class StartSimulation : ICommand
    {
        public string GetToken => "start_simulation";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            EditorApplication.EnterPlaymode();
            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}