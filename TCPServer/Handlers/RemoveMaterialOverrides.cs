using System;
using System.Collections.Generic;
using System.Linq;
using PrometheanAI.Modules.Utils;
using UnityEditor;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Removes material overrides from a given prefab instance in Scene
    /// </summary>
    public class RemoveMaterialOverrides : ICommand
    {
        public string GetToken => "remove_material_overrides";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var names = commandParametersString[0].Split(',').ToList();
            var allObjects = CommandUtility.GetAllValidObjectsFromScene();
            foreach (var obj in allObjects) {
                foreach (var name in names) {
                    if (JsonOutputFormatUtility.GeneratePrometheanObjectName(obj) == name
                        || obj.name == name) {
                        var prefab = PrefabUtility.GetCorrespondingObjectFromSource(obj);
                        if (prefab == null) {
                            break;
                        }

                        var prefabRenderer = prefab.GetComponent<Renderer>();
                        if (prefabRenderer == null) {
                            prefabRenderer = prefab.GetComponentInChildren<Renderer>();
                        }

                        var prefabMaterials = prefabRenderer.sharedMaterials;

                        var objRenderer = obj.GetComponent<Renderer>();
                        if (objRenderer == null) {
                            objRenderer = obj.GetComponentInChildren<Renderer>();
                        }
                        
                        UndoUtility.RecordUndo(GetToken,objRenderer,false);
                        objRenderer.materials = prefabMaterials;
                        break;
                    }
                }
            }

            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}