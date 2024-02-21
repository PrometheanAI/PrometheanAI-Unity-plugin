using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Used to gather Vertex data for specified GameObject and send it back to PrometheanAI
    /// </summary>
    public class GetVertexDataFromSceneObject : ICommand
    {
        public string GetToken => "get_vertex_data_from_scene_object";

        public void Handle(string rawCommandData, List<string> commandParametersString, Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var name = commandParametersString[0];
            var allSceneObjects = CommandUtility.GetAllValidObjectsFromScene();
            var outputData = new Dictionary<string, Dictionary<string, float[]>>();
            foreach (var obj in allSceneObjects) {
                if (obj.name == name || JsonOutputFormatUtility.GeneratePrometheanObjectName(obj) == name) {
                    var mesh = obj.GetComponent<MeshFilter>().sharedMesh;
                    var vertices = mesh.vertices;

                    var vertData = new Dictionary<string, float[]>();
                    foreach (var ind in mesh.GetIndices(0)) {
                        var vertex = vertices[ind];
                        var value = new[] {
                            CommandUtility.Round(vertex.x, 4),
                            CommandUtility.Round(vertex.y, 4),
                            CommandUtility.Round(vertex.z, 4)
                        };
                        vertData.Add(ind.ToString(), value);
                    }

                    outputData.Add("vertex_positions", vertData);
                    break;
                }
            }

            var output = JsonOutputFormatUtility.GenerateJsonString(outputData);
            callback.Invoke(CommandHandleProcessState.Success, output);
        }
    }
}