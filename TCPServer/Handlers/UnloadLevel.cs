using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEngine.SceneManagement;

namespace PrometheanAI.Modules.TCPServer
{
    /// <summary>
    /// Unloads a Scene with a given path
    /// </summary>
    public class UnloadLevel : ICommand
    {
        public string GetToken => "unload_level";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var paramsData = JsonOutputFormatUtility.ProduceParametersData(commandParametersString);
            paramsData = JsonOutputFormatUtility.SplitStringOnNewLine(paramsData);
            var path = paramsData[0];
            var scene = SceneManager.GetSceneByPath(path);
            if (scene.IsValid()) {
                SceneManager.UnloadSceneAsync(scene);
                callback.Invoke(CommandHandleProcessState.Success, string.Empty);
            }
            else {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
            }
        }
    }
}