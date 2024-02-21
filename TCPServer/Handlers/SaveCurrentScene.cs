using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Saves currently opened Scene
    /// </summary>
    public class SaveCurrentScene : ICommand
    {
        public string GetToken => "save_current_scene";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            AssetDatabase.SaveAssets();
            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}