using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    //called by Command "add assets to library from content browser"
    //Raw incoming command example : get_asset_browser_selection

    /// <summary>
    /// Used to get a paths of currently selected object in Project view
    /// </summary>
    public class GetAssetBrowserSelection : ICommand
    {
        public string GetToken => "get_asset_browser_selection";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            //Generating Promethean typed path for next usage by AssetDataUtility
            var paths = new List<string>();
            foreach (var obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets)) {
                var path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path)) {
                    var prometheanPath = "";
                    var type = AssetDatabase.GetMainAssetTypeAtPath(path).ToString();
                    if (type.EndsWith("GameObject")) {
                        prometheanPath = $"StaticMesh'{path}'";
                    }
                    else if (type.EndsWith("Material")) {
                        prometheanPath = $"Material'{path}'";
                    }
                    else if (type.EndsWith("SceneAsset")) {
                        prometheanPath = $"World'{path}'";
                    }
                    else if (type.EndsWith("Texture2D")) {
                        prometheanPath = $"Texture2D'{path}'";
                    }

                    paths.Add(prometheanPath);
                }
            }

            var output = "";
            foreach (var objPath in paths) {
                output += objPath + "\n";
            }

            if (output.Length == 0) {
                output = "None";
            }

            callback.Invoke(CommandHandleProcessState.Success, output);
        }
    }
}