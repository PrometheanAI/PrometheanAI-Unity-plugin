using System;
using System.Collections.Generic;
using System.Linq;
using PrometheanAI.Modules.Utils;
using UnityEngine.SceneManagement;

namespace PrometheanAI.Modules.TCPServer
{
    /// <summary>
    /// Removes Promethean naming format from a given gameObjects name
    /// </summary>
    public class Fix : ICommand
    {
        public string GetToken => "fix";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1
                || commandParametersString[0] != "names") {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var currentScene = SceneManager.GetActiveScene();
            var allSceneObjects = currentScene.GetRootGameObjects().ToList();
            allSceneObjects = CommandUtility.AddChildObjects(allSceneObjects);
            foreach (var obj in allSceneObjects) {
                var id = $"#{obj.GetInstanceID()}";
                if (obj.name.Contains(id)) {
                    UndoUtility.RecordUndo(GetToken, obj, false);
                    obj.name = obj.name.Replace(id, "");
                }
            }

            EditorUtils.RefreshEditorWindows();
            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}