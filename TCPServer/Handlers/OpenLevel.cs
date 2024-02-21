using System;
using System.Collections.Generic;
using System.IO;
using PrometheanAI.Modules.Utils;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Loads Scene with a given path,saving currently open scene if necessary and user decides to
    /// </summary>
    public class OpenLevel : ICommand
    {
        public string GetToken => "open_level";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var paramsData = JsonOutputFormatUtility.ProduceParametersData(commandParametersString);
            paramsData = JsonOutputFormatUtility.SplitStringOnNewLine(paramsData);
            var path = paramsData[0];
            var name = Path.GetFileName(path);
            var guids = AssetDatabase.FindAssets("t:Scene", null);
            var foundedPaths = new List<string>();
            foreach (string guid in guids) {
                foundedPaths.Add(AssetDatabase.GUIDToAssetPath(guid));
            }

            foreach (var foundedPath in foundedPaths) {
                var foundedName = Path.GetFileName(foundedPath).Replace(".unity", "");
                if (foundedName.ToLower() == name) {
                    var currentScene = SceneManager.GetActiveScene();

                    if (currentScene.name == string.Empty || currentScene.isDirty) {
                        var scenesToSave = new[] {currentScene};
                        if (!EditorSceneManager.SaveModifiedScenesIfUserWantsTo(scenesToSave)) {
                            Debug.Log($"User declined scene saving.Stopping loading new scene");
                            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
                            return;
                        }
                    }

                    EditorSceneManager.OpenScene(foundedPath, OpenSceneMode.Single);
                    callback.Invoke(CommandHandleProcessState.Success, string.Empty);
                    return;
                }
            }

            Debug.Log($"Theres no scene with name {path} found in the project!");
            callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
        }
    }
}