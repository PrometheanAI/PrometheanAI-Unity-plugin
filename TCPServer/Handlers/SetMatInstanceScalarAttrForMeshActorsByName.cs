using System;
using System.Collections.Generic;
using System.Globalization;
using PrometheanAI.Modules.Utils;
using UnityEditor;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Set specified Float ShaderProperties of the materials of  the specified GameObject
    /// </summary>
    public class SetMatInstanceScalarAttrForMeshActorsByName : ICommand
    {
        public string GetToken => "set_mat_instance_scalar_attr_for_mesh_actors_by_name";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 3) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var attributeName = commandParametersString[0];
            var value = float.Parse(commandParametersString[1], CultureInfo.InvariantCulture.NumberFormat);
            var names = commandParametersString[2].Split(',');
            var allSceneObjects = CommandUtility.GetAllValidObjectsFromScene();
            foreach (var obj in allSceneObjects) {
                foreach (var name in names) {
                    if (JsonOutputFormatUtility.GeneratePrometheanObjectName(obj) == name
                        || obj.name == name) {
                        var materials = obj.GetComponent<Renderer>().sharedMaterials;
                        if (materials.Length > 0) {
                            foreach (var material in materials) {
                                var shader = material.shader;
                                for (int i = 0; i < ShaderUtil.GetPropertyCount(shader); i++) {
                                    var propertyName = ShaderUtil.GetPropertyName(shader, i);
                                    if (propertyName == attributeName) {
                                        UndoUtility.RecordUndo(GetToken, material, false);
                                        material.SetFloat(propertyName, value);
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