using System;
using System.Collections.Generic;
using System.Linq;
using PrometheanAI.Modules.Utils;
using UnityEditor;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Send Promethean ShaderProperties of the materials of  the selected GameObjects
    /// </summary>
    public class GetMatAttrsFromSelectedMeshActors : ICommand
    {
        public string GetToken => "get_mat_attrs_from_selected_mesh_actors";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 2) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var attributeType = commandParametersString[0];
            var selectedObjects = Selection.gameObjects.ToList();
            selectedObjects = CommandUtility.AddChildObjects(selectedObjects);
            selectedObjects = CommandUtility.ValidateObjects(selectedObjects);
            var propertyNames = new List<string>();
            foreach (var obj in selectedObjects) {
                var materials = obj.GetComponent<Renderer>().sharedMaterials;
                if (materials.Length > 0) {
                    foreach (var material in materials) {
                        var shader = material.shader;
                        switch (attributeType) {
                            case "vector":
                                for (var i = 0; i < ShaderUtil.GetPropertyCount(shader); i++) {
                                    var propertyName = ShaderUtil.GetPropertyName(shader, i);
                                    var propertyType = ShaderUtil.GetPropertyType(shader, i);
                                    switch (propertyType) {
                                        case ShaderUtil.ShaderPropertyType.Color:
                                            propertyNames.Add(propertyName);
                                            break;
                                        case ShaderUtil.ShaderPropertyType.Vector:
                                            propertyNames.Add(propertyName);
                                            break;
                                    }
                                }

                                break;
                            case "scalar":
                                for (var i = 0; i < ShaderUtil.GetPropertyCount(shader); i++) {
                                    var propertyName = ShaderUtil.GetPropertyName(shader, i);
                                    var propertyType = ShaderUtil.GetPropertyType(shader, i);
                                    switch (propertyType) {
                                        case ShaderUtil.ShaderPropertyType.Float:
                                            propertyNames.Add(propertyName);
                                            break;
                                        case ShaderUtil.ShaderPropertyType.Range:
                                            propertyNames.Add(propertyName);
                                            break;
                                    }
                                }

                                break;
                            case "texture":
                                for (var i = 0; i < ShaderUtil.GetPropertyCount(shader); i++) {
                                    var propertyName = ShaderUtil.GetPropertyName(shader, i);
                                    var propertyType = ShaderUtil.GetPropertyType(shader, i);
                                    switch (propertyType) {
                                        case ShaderUtil.ShaderPropertyType.TexEnv:
                                            propertyNames.Add(propertyName);
                                            break;
                                    }
                                }

                                break;

                            //cases "staticSwitch","componentMask"?
                        }
                    }
                }
            }

            var outputData = CommandUtility.StringArrayToString(propertyNames);
            callback.Invoke(CommandHandleProcessState.Success, outputData);
        }
    }
}