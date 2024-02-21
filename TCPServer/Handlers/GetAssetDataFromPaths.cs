using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    //called by Command "add assets to library from scene selection","add assets to the library from content browser"
    //Raw incoming command example : get_asset_data_from_paths Assets/PrometheanAI/Prefabs/Capsule.prefab

    /// <summary>
    /// Used to generate data about Object with given path and send it back to PrometheanAI
    /// </summary>
    public class GetAssetDataFromPaths : ICommand
    {
        public string GetToken => "get_asset_data_from_paths";

        public void Handle(string rawCommandData, List<string> commandParametersString, Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var paths = commandParametersString[0].Split(',');
            var outputData = new Dictionary<string, Dictionary<string, object>>();
            foreach (var path in paths) { //actually atm we don't know how many object will be provided by this command

                if (path.StartsWith("StaticMesh")) {
                    var data = AssetDataUtility.GetGameObjectData(path);
                    outputData.Add(path, data);
                }
                else if (path.StartsWith("MaterialInstanceConstant")) {
                    var data = AssetDataUtility.GetMaterialData(path);
                    outputData.Add(path, data);
                }
                else if (path.StartsWith("Material")) {
                    var data = AssetDataUtility.GetMaterialData(path);
                    outputData.Add(path, data);
                }
                else if (path.StartsWith("Texture2D")) {
                    var data = AssetDataUtility.GetTexture2dData(path);
                    outputData.Add(path, data);
                }
                else if (path.StartsWith("World")) {
                    var data = AssetDataUtility.GetSceneData(path);
                    outputData.Add(path, data);
                }
                else if (path.StartsWith("Blueprint")) {
                    var data = AssetDataUtility.GetGameObjectData(path);
                    outputData.Add(path, data);
                }
                else {
                    var data = AssetDataUtility.GetAssetDataWithoutType(path);
                    outputData.Add(path, data);
                }
            }

            var output = JsonOutputFormatUtility.GenerateJsonString(outputData);
            var finalOutput = "update_cmd_line_dcc_progress " + output;
            callback.Invoke(CommandHandleProcessState.SingleState, finalOutput);
        }
    }
}