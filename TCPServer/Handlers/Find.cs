using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEditor;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Find object in ProjectView and selects it
    /// </summary>
    public class Find : ICommand
    {
        public string GetToken => "find";

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

            path = path.Replace("'", "");
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            UndoUtility.RecordUndo(GetToken, Selection.objects, false);
            EditorApplication.ExecuteMenuItem("Window/Windows/Project");
            if (asset != null) {
                Selection.activeObject = asset;
            }

            EditorUtils.RefreshEditorWindows();
            EditorApplication.RepaintProjectWindow();
            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}