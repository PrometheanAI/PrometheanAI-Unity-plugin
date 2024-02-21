using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Sets a material to GameObjects mesh at specified path at given index
    /// </summary>
    public class SetMeshAssetMaterial : ICommand
    {
        public string GetToken => "set_mesh_asset_material";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var obj =
                JsonConvert.DeserializeObject<Dictionary<string, object[][]>>(
                    commandParametersString[0]);
            foreach (var pair in obj) {
                var targetObject = AssetDatabase.LoadAssetAtPath<GameObject>(pair.Key);
                var renderer = targetObject.GetComponent<Renderer>();
                var materials = new List<Material>();
                foreach (var data in pair.Value) {
                    var materialPath = (string) data[0];
                    var index = 0;
                    if (commandParametersString.Count > 1) {
                        callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                        index = Convert.ToInt32(data[1]);
                    }

                    if (renderer.sharedMaterials.Length > index) {
                        var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                        materials.Add(material);
                        renderer.sharedMaterials[index] = material;
                    }
                    else {
                        Debug.LogWarning("Targets Mesh materials number is lesser then given index");
                    }
                }

                UndoUtility.RecordUndo(GetToken, renderer, false);
                renderer.sharedMaterials = materials.ToArray();

                EditorUtility.SetDirty(targetObject);
                AssetDatabase.SaveAssets();
            }

            EditorUtils.RefreshEditorWindows();
            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}