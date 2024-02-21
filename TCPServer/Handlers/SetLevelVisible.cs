using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Opens a Scene with corresponding path
    /// </summary>
    public class SetLevelVisible : ICommand
    {
        public string GetToken => "set_level_visible";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var paramsData = JsonOutputFormatUtility.ProduceParametersData(commandParametersString);
            var path = paramsData[0];
            var scene = SceneManager.GetSceneByPath(path);
            if (scene.IsValid()) {
                EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                callback.Invoke(CommandHandleProcessState.Success, string.Empty);
            }
            else {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
            }
        }
    }
}