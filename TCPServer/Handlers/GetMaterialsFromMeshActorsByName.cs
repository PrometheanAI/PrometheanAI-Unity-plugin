using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEditor;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer
{
    /// <summary>
    /// Send Promethean paths of the materials of  the specified GameObject
    /// </summary>
    public class GetMaterialsFromMeshActorsByName : ICommand
    {
        public string GetToken => "get_materials_from_mesh_actors_by_name";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var targetNames = JsonOutputFormatUtility.ProduceParametersData(commandParametersString);
            var allSceneObjects = CommandUtility.GetAllValidObjectsFromScene();
            var sortedGameObjects = new List<GameObject>();
            foreach (var obj in allSceneObjects) {
                foreach (var name in targetNames) {
                    if (JsonOutputFormatUtility.GeneratePrometheanObjectName(obj) == name
                        || obj.name == name) {
                        sortedGameObjects.Add(obj);
                    }
                }
            }

            var allMaterials = Resources.FindObjectsOfTypeAll<Material>();
            var materialsData = new Dictionary<string, string[]>();
            foreach (var obj in sortedGameObjects) {
                var paths = new List<string>();
                var objectsMaterials = obj.GetComponent<Renderer>().sharedMaterials;
                foreach (var material in allMaterials) {
                    foreach (var objMaterial in objectsMaterials) {
                        if (JsonOutputFormatUtility.CutMaterialName(objMaterial) == material.name) {
                            paths.Add(AssetDatabase.GetAssetPath(material));
                        }
                    }
                }

                materialsData.Add(JsonOutputFormatUtility.GeneratePrometheanObjectName(obj), paths.ToArray());
            }

            var output = JsonOutputFormatUtility.GenerateJsonString(materialsData);
            callback.Invoke(CommandHandleProcessState.Success, output);
        }
    }
}