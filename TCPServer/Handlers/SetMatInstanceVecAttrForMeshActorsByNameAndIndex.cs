using System;
using System.Collections.Generic;
using System.Globalization;
using PrometheanAI.Modules.Utils;
using UnityEditor;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Set specified Float ShaderProperties of the material of  the specified GameObject at given index
    /// </summary>
    public class SetMatInstanceVecAttrForMeshActorsByNameAndIndex : ICommand
    {
        public string GetToken => "set_mat_instance_vec_attr_for_mesh_actors_by_name_and_index";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 4) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var attributeName = commandParametersString[0];
            var valueData = commandParametersString[1].Split(',');
            var value = CommandUtility.StringArrayToVector4(valueData);
            var names = commandParametersString[2].Split(',');
            var index = int.Parse(commandParametersString[3], CultureInfo.InvariantCulture.NumberFormat);
            var allSceneObjects = CommandUtility.GetAllValidObjectsFromScene();
            foreach (var obj in allSceneObjects) {
                foreach (var name in names) {
                    if (JsonOutputFormatUtility.GeneratePrometheanObjectName(obj) == name
                        || obj.name == name) {
                        var materials = obj.GetComponent<Renderer>().sharedMaterials;
                        if (materials.Length > 0) {
                            for (var j = 0; j < materials.Length; j++) {
                                if (j == index) {
                                    var material = materials[j];
                                    var shader = material.shader;
                                    for (var i = 0; i < ShaderUtil.GetPropertyCount(shader); i++) {
                                        var propertyName = ShaderUtil.GetPropertyName(shader, i);
                                        if (propertyName == attributeName) {
                                            UndoUtility.RecordUndo(GetToken, material, false);
                                            material.SetVector(propertyName, value);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}