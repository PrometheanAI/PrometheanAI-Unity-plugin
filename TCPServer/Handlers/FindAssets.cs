using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEditor;
using Object = UnityEngine.Object;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Find assets in Project window and selects them
    /// </summary>
    public class FindAssets : ICommand
    {
        public string GetToken => "find_assets";

        public void Handle(string rawCommandData, List<string> commandParametersString, Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var paths = commandParametersString[0].Split(',');
            var foundedObjects = new List<Object>();
            foreach (var path in paths) {
                var currentPath = path;
                if (currentPath.StartsWith("StaticMesh")) {
                    currentPath = path.Replace("StaticMesh", "");
                }

                if (currentPath.StartsWith("Material")) {
                    currentPath = path.Replace("Material", "");
                }

                if (currentPath.StartsWith("MaterialInstanceConstant")) {
                    currentPath = path.Replace("MaterialInstanceConstant", "");
                }

                if (currentPath.StartsWith("Texture2D")) {
                    currentPath = path.Replace("Texture2D", "");
                }

                currentPath = currentPath.Replace("'", "");
                var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(currentPath);
                if (asset != null) {
                    foundedObjects.Add(asset);
                }
            }

            UndoUtility.RecordUndo(GetToken, Selection.objects, false);
            Selection.objects = foundedObjects.ToArray();
            EditorApplication.ExecuteMenuItem("Window/Windows/Project");
            EditorUtils.RefreshEditorWindows();
            EditorApplication.RepaintProjectWindow();
            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}