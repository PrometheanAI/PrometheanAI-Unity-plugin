using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PrometheanAI.Modules.Utils;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    //called by Command "sync"
    //Raw incoming command example :Received command get_all_existing_assets_by_type StaticMesh
    //C:\Users\UserName\AppData\Local\Temp\tmpd7_zatqx

    /// <summary>
    /// Used to save array of Paths of all Object of specified Type to a local file
    /// </summary>
    public class GetAllExistingAssetsByType : ICommand
    {
        public string GetToken => "get_all_existing_assets_by_type";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 2) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var pathToSave = commandParametersString[1];
            var targetType = commandParametersString[0];
            var sortedType = new List<string>();
            switch (targetType) {
                case "StaticMesh":
                    sortedType.Add(".prefab");
                    sortedType.Add(".fbx");
                    sortedType.Add((".obj"));
                    break;
                case "Material":
                    sortedType.Add(".mat");
                    break;
                case "MaterialInstanceConstant":
                    sortedType.Add(".mat");
                    break;
                case "Texture2D":
                    sortedType.Add(".jpg");
                    sortedType.Add(".png");
                    sortedType.Add(".bmp");
                    sortedType.Add(".exr");
                    sortedType.Add(".hdr");
                    sortedType.Add(".gif");
                    sortedType.Add(".hdr");
                    sortedType.Add(".iff");
                    sortedType.Add(".psd");
                    sortedType.Add(".tga");
                    sortedType.Add(".pict");
                    sortedType.Add(".tiff");
                    break;
                case "Blueprint":
                    sortedType.Add("");
                    break;
            }

            //Querying all object by given type using its path 
            var assetFolderPath = "Assets/";
            var dataPath = Application.dataPath;
            var folderPath = dataPath.Substring(0, dataPath.Length - 6) + assetFolderPath;
            var filePaths = Directory
                .GetFiles(folderPath, searchPattern: "*", searchOption: SearchOption.AllDirectories).ToList();
            var sortedPath = new List<string>();
            foreach (var sFilePath in filePaths) {
                if (sFilePath.EndsWith(".meta") && sFilePath.EndsWith(".cs")) {
                    continue;
                }

                foreach (var type in sortedType) {
                    if (sFilePath.EndsWith(type)) {
                        var path = sFilePath.Replace("\\", "/").Replace(" ", "_");
                        path = path.Substring(path.IndexOf("Assets/"));
                        sortedPath.Add(path);
                        break;
                    }
                }
            }

            var dictionary = new Dictionary<string, string[]>();
            dictionary.Add("Assets", sortedPath.ToArray());
            var output = JsonOutputFormatUtility.GenerateJsonString(dictionary);
            File.Delete(pathToSave);
            var writer = new StreamWriter(pathToSave, true);
            writer.WriteLine(output);
            writer.Close();
            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}