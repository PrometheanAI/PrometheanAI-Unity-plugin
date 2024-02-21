using System;
using System.Collections.Generic;
using System.IO;
using PrometheanAI.Modules.Utils;
using UnityEditor;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Saving data about given object to a local file with specified path
    /// </summary>
    public class GetAssetDataFromBrowserSelection : ICommand
    {
        public string GetToken => "get_asset_data_from_browser_selection";

        public void Handle(string rawCommandData, List<string> commandParametersString, Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var pathToSave = commandParametersString[0];
            var paths = new List<string>();
            foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets)) {
                var path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path)) {
                    paths.Add(path);
                }
            }

            var outputData = new Dictionary<string, Dictionary<string, object>>();
            foreach (var path in paths) {
                var data = AssetDataUtility.GetAssetDataWithoutType(path);
                outputData.Add(path, data);
            }

            var output = JsonOutputFormatUtility.GenerateJsonString(outputData);
            File.Delete(pathToSave);
            var writer = new StreamWriter(pathToSave, true);
            writer.WriteLine(output);
            writer.Close();
            callback.Invoke(CommandHandleProcessState.Success, "Done");
        }
    }
}