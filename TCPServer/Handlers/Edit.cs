using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEditor;
using Object = UnityEngine.Object;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Opens object with corresponding path
    /// </summary>
    public class Edit : ICommand
    {
        public string GetToken => "edit";

        public void Handle(string rawCommandData, List<string> commandParametersString, Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var path = commandParametersString[0];
            if (path.StartsWith("StaticMesh")) {
                path = path.Replace("StaticMesh", "");
            }

            if (path.StartsWith("Material")) {
                path = path.Replace("Material", "");
            }

            if (path.StartsWith("MaterialInstanceConstant")) {
                path = path.Replace("MaterialInstanceConstant", "");
            }

            if (path.StartsWith("Texture2D")) {
                path = path.Replace("Texture2D", "");
            }

            AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<Object>(path));
            EditorUtils.RefreshEditorWindows();
            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}