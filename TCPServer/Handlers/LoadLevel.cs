using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Loads Scene with a given path
    /// </summary>
    public class LoadLevel : ICommand
    {
        public string GetToken => "load_level";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var paramsData = JsonOutputFormatUtility.ProduceParametersData(commandParametersString);
            paramsData = JsonOutputFormatUtility.SplitStringOnNewLine(paramsData);
            var path = paramsData[0];
            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
            if (sceneAsset != null) {
                EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                callback.Invoke(CommandHandleProcessState.Success, string.Empty);
            }
            else {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
            }
        }
    }
}