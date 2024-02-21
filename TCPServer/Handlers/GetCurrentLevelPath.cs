using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Returns path to current opened Scene object
    /// </summary>
    public class GetCurrentLevelPath : ICommand
    {
        public string GetToken => "get_current_level_path";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            callback.Invoke(CommandHandleProcessState.Success, SceneManager.GetActiveScene().path);
        }
    }
}