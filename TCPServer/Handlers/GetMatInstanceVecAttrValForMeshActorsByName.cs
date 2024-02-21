using System;
using System.Collections.Generic;
using System.Globalization;
using PrometheanAI.Modules.Utils;
using UnityEditor;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Send Promethean Vector ShaderProperties of the materials of  the specified GameObject
    /// </summary>
    public class GetMatInstanceVecAttrValForMeshActorsByName : ICommand
    {
        public string GetToken => "get_mat_instance_vec_attr_val_for_mesh_actors_by_name";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback)
        {
            if (commandParametersString.Count < 2)
            {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var attributeName = commandParametersString[0];
            var names = commandParametersString[1].Split(',');
            var index = 0;
            if (commandParametersString.Count > 2)
            {
                index = Int32.Parse(commandParametersString[2], CultureInfo.InvariantCulture.NumberFormat);
            }

            var allSceneObjects = CommandUtility.GetAllValidObjectsFromScene();
            var propertyValues = new List<Vector4>();
            foreach (var obj in allSceneObjects)
            {
                foreach (var name in names)
                {
                    if (JsonOutputFormatUtility.GeneratePrometheanObjectName(obj) != name
                        && obj.name != name)
                    {
                        continue;
                    }
                    var materials = obj.GetComponent<Renderer>()?.sharedMaterials;
                    if (materials == null || materials.Length == 0)
                    {
                        continue;
                    }
                    if (index >= materials.Length) continue;

                    var j = index;
                    var material = materials[j];
                    var shader = material.shader;
                    for (var i = 0; i < ShaderUtil.GetPropertyCount(shader); i++)
                    {
                        var propertyName = ShaderUtil.GetPropertyName(shader, i);

                        switch (ShaderUtil.GetPropertyType(shader, i))
                        {
                            case ShaderUtil.ShaderPropertyType.Color:
                                if (propertyName == attributeName)
                                {
                                    propertyValues.Add(material.GetVector(propertyName));
                                }

                                break;
                            case ShaderUtil.ShaderPropertyType.Vector:
                                if (propertyName == attributeName)
                                {
                                    propertyValues.Add(material.GetVector(propertyName));
                                }

                                break;
                        }
                    }

                }
            }

            if (propertyValues.Count < 1)
            {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var outputData = CommandUtility.Vector4ArrayToString(propertyValues);
            callback.Invoke(CommandHandleProcessState.Success, outputData);
        }
    }
}