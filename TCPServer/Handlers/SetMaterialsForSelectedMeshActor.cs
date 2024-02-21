using System;
using System.Collections.Generic;
using System.Linq;
using PrometheanAI.Modules.Utils;
using UnityEditor;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Sets materials for selected GameObjects in Scene
    /// </summary>
    public class SetMaterialsForSelectedMeshActor : ICommand
    {
        public string GetToken => "set_materials_for_selected_mesh_actor";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var materialPath = commandParametersString[0];
            var index = 0;
            if (commandParametersString.Count > 2) {
                index = Convert.ToInt32(commandParametersString[1]);
            }

            var selectedObject = Selection.gameObjects.ToList();
            selectedObject = CommandUtility.ValidateObjects(selectedObject);
            var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            foreach (var obj in selectedObject) {
                var renderer = obj.GetComponent<Renderer>();
                var materials = renderer.sharedMaterials;
                if (renderer.sharedMaterials.Length > index) {
                    materials[index] = material;
                    UndoUtility.RecordUndo(GetToken, renderer, false);
                    renderer.sharedMaterials = materials;
                }
            }

            EditorUtils.RefreshEditorWindows();
            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}