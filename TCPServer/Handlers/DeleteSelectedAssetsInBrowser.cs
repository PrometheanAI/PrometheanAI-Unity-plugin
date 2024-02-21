using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEditor;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Deletes object from the project
    /// </summary>
    public class DeleteSelectedAssetsInBrowser : ICommand
    {
        public string GetToken => "delete_selected_assets_in_browser";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            foreach (var obj in Selection.GetFiltered(typeof(UnityEngine.Object),
                         SelectionMode.Assets)) {
                var pathToDelete = AssetDatabase.GetAssetPath(obj);
                AssetDatabase.DeleteAsset(pathToDelete);
            }

            EditorUtils.RefreshEditorWindows();
            EditorApplication.RepaintProjectWindow();
            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}