using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEditor;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Send Promethean Float ShaderProperties of the materials of  the specified GameObject
    /// </summary>
    public class GetMatInstanceScalarAttrValForMeshActorsByName : ICommand
    {
        public string GetToken => "get_mat_instance_scalar_attr_val_for_mesh_actors_by_name";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 2) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var attributeName = commandParametersString[0];
            var names = commandParametersString[1].Split(',');
            var allSceneObjects = CommandUtility.GetAllValidObjectsFromScene();
            var propertyValues = new List<float>();
            foreach (var obj in allSceneObjects) {
                foreach (var name in names) {
                    if (JsonOutputFormatUtility.GeneratePrometheanObjectName(obj) == name
                        || obj.name == name) {
                        var materials = obj.GetComponent<Renderer>().sharedMaterials;
                        if (materials.Length > 0) {
                            foreach (var material in materials) {
                                var shader = material.shader;
                                for (var i = 0; i < ShaderUtil.GetPropertyCount(shader); i++) {
                                    var propertyName = ShaderUtil.GetPropertyName(shader, i);
                                    var propertyType = ShaderUtil.GetPropertyType(shader, i);
                                    switch (propertyType) {
                                        case ShaderUtil.ShaderPropertyType.Float:
                                            if (propertyName == attributeName) {
                                                propertyValues.Add(material.GetFloat(propertyName));
                                            }

                                            break;
                                        case ShaderUtil.ShaderPropertyType.Range:
                                            if (propertyName == attributeName) {
                                                propertyValues.Add(material.GetFloat(propertyName));
                                            }

                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (propertyValues.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var outputData = CommandUtility.FloatArrayToString(propertyValues);
            callback.Invoke(CommandHandleProcessState.Success, outputData);
        }
    }
}