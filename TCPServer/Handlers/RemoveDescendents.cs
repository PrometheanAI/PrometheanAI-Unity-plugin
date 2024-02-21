using System;
using System.Collections.Generic;
using System.Linq;
using PrometheanAI.Modules.Utils;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    //call by commands "scatter"
    //Raw incoming command example : remove_descendents cube#-11200

    /// <summary>
    /// Used to delete all Child objects of specified GameObject
    /// </summary>
    public class RemoveDescendents : ICommand
    {
        public string GetToken => "remove_descendents";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var namedObjects = JsonOutputFormatUtility.ProduceParametersData(commandParametersString);
            var currentScene = SceneManager.GetActiveScene();
            var allSceneObjects = currentScene.GetRootGameObjects().ToList();
            foreach (var obj in allSceneObjects) {
                foreach (var name in namedObjects) {
                    if (JsonOutputFormatUtility.GeneratePrometheanObjectName(obj) == name
                        || obj.name == name) {
                        while (obj.transform.childCount != 0) {
                            foreach (UnityEngine.Transform child in obj.transform) {
                                UndoUtility.RegisterObjectDestruction(GetToken, child.gameObject);
                            }
                        }

                        break;
                    }
                }
            }

            EditorUtils.RefreshEditorWindows();
            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}