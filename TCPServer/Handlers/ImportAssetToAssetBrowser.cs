using System;
using System.Collections.Generic;
using UnityEditor;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Imports assets at path of currently selected object
    /// </summary>
    public class ImportAssetToAssetBrowser : ICommand
    {
        public string GetToken => "import_asset_to_asset_browser";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (Selection.objects.Length < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var currentObj = Selection.objects[0];
            var currentPath = AssetDatabase.GetAssetPath(currentObj);
            if (currentPath != string.Empty) {
                AssetDatabase.ImportAsset(currentPath);
            }

            EditorApplication.RepaintProjectWindow();
            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}