using System;
using System.Collections.Generic;
using System.IO;
using PrometheanAI.Modules.Utils;
using Newtonsoft.Json;
using UnityEditor;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Imports Textures at given path and selects them
    /// </summary>
    public class ImportTexture : ICommand
    {
        public string GetToken => "import_texture";

        public void Handle(string rawCommandData, List<string> commandParametersString, Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var output = JsonConvert.DeserializeObject<Dictionary<string, string>>(commandParametersString[0]);
            var loadedObjects = new List<UnityEngine.Object>();
            foreach (var pair in output) {
                var destinationPath = pair.Value + Path.GetFileName(pair.Key);
                Directory.CreateDirectory(pair.Value);
                FileUtil.CopyFileOrDirectory(pair.Key, destinationPath);
                AssetDatabase.ImportAsset(destinationPath);
                var loadedObj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(destinationPath);
                if (loadedObj != null) {
                    loadedObjects.Add(loadedObj);
                }
            }

            Selection.objects = loadedObjects.ToArray();
            EditorUtils.RefreshEditorWindows();
            EditorApplication.RepaintProjectWindow();
            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}