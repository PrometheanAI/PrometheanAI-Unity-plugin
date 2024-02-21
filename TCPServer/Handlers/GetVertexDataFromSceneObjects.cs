using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    //was seen after command "memorize"
    //Raw incoming command example : get_vertex_data_from_scene_objects floor#13490

    /// <summary>
    /// Used to gather vertex data from specified GameObject and send it to PrometheanAI
    /// </summary>
    public class GetVertexDataFromSceneObjects : ICommand
    {
        public string GetToken => "get_vertex_data_from_scene_objects";

        public void Handle(string rawCommandData, List<string> commandParametersString, Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var names = commandParametersString[0].Split(',');
            var allSceneObjects = CommandUtility.GetAllValidObjectsFromScene();
            var outputData = new Dictionary<string, Dictionary<string, float[]>[]>();
            foreach (var obj in allSceneObjects) {
                foreach (var name in names) {
                    if (obj.name == name || JsonOutputFormatUtility.GeneratePrometheanObjectName(obj) == name) {
                        var mesh = obj.GetComponent<MeshFilter>().sharedMesh;
                        var vertices = mesh.vertices;
                        var vertData = new Dictionary<string, float[]>();
                        var index = 0;
                        foreach (var ind in mesh.GetIndices(0)) {
                            index++;
                            var vertex = vertices[ind];

                            var localToWorld = obj.transform.localToWorldMatrix;
                            var worldRelativeVertex = localToWorld.MultiplyPoint3x4(vertex);

                            var value = new[] {
                                CommandUtility.ConvertToCentimeters(worldRelativeVertex.x),
                                CommandUtility.ConvertToCentimeters(worldRelativeVertex.y),
                                CommandUtility.ConvertToCentimeters(worldRelativeVertex.z)
                            };
                            vertData.Add(index.ToString(), value);
                        }

                        var vertArray = new[] {vertData};
                        outputData.Add(name, vertArray);
                        break;
                    }
                }
            }

            var output = JsonOutputFormatUtility.GenerateJsonString(outputData);
            callback.Invoke(CommandHandleProcessState.Success, output);
        }
    }
}