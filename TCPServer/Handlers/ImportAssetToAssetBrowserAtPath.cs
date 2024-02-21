using System;
using System.Collections.Generic;
using UnityEditor;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Imports assets at given path 
    /// </summary>
    public class ImportAssetToAssetBrowserAtPath : ICommand
    {
        public string GetToken => "import_asset_to_asset_browser_at_path";

        public void Handle(string rawCommandData, List<string> commandParametersString, Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var path = commandParametersString[0];
            AssetDatabase.ImportAsset(path);
            EditorApplication.RepaintProjectWindow();
            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}