using System;
using System.Collections.Generic;
using System.Linq;
using PrometheanAI.Modules.Utils;
using UnityEditor;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer
{
    /// <summary>
    /// Send Promethean paths of the materials of  the selected GameObject
    /// </summary>
    public class GetMaterialsFromSelectedMeshActor : ICommand
    {
        public string GetToken => "get_materials_from_selected_mesh_actor";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            var selectedObjects = Selection.gameObjects.ToList();
            selectedObjects = CommandUtility.ValidateObjects(selectedObjects);
            var allMaterials = Resources.FindObjectsOfTypeAll<Material>();
            var paths = new List<string>();
            foreach (var obj in selectedObjects) {
                var objectsMaterials = obj.GetComponent<Renderer>().sharedMaterials;
                foreach (var material in allMaterials) {
                    foreach (var objMaterial in objectsMaterials) {
                        if (JsonOutputFormatUtility.CutMaterialName(objMaterial) == material.name) {
                            paths.Add(AssetDatabase.GetAssetPath(material));
                        }
                    }
                }
            }

            var outputString = CommandUtility.StringArrayToString(paths);
            callback.Invoke(CommandHandleProcessState.Success, outputString);
        }
    }
}