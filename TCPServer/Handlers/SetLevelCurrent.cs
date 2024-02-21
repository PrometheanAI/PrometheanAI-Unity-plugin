using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEngine.SceneManagement;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Sets a Scene by a given path to a currently active one
    /// </summary>
    public class SetLevelCurrent : ICommand
    {
        public string GetToken => "set_level_current";

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
                SceneManager.SetActiveScene(scene);
                callback.Invoke(CommandHandleProcessState.Success, string.Empty);
            }
            else {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
            }
        }
    }
}