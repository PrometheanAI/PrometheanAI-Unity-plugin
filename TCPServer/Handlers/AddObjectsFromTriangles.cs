using System;
using System.Collections.Generic;
using System.Globalization;
using PrometheanAI.Modules.Utils;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    //Raw incoming command example: 
    //   Format is as follows - higher level key is the original unique name of the asset in standalone memory, the value is the dictionary of object data to generate
    //  {"ceiling_4847230804619326912":{"name":"ceiling","tri_ids":[[0,1,2],[0,2,3]],"verts":[[-25.38,274.54,-9.38],[-368.04,274.54,-9.38],[-368.04,274.54,-166.49],[-25.38,274.54,-166.49]],"normals":[[0.0,-1.0,0.0],[0.0,-1.0,0.0]]},
    //   "floor_3593213985134695789":{"name":"__floor","tri_ids":[[0,1,2],[0,2,3]],"verts":[[-25.38,0.0,-9.38],[-368.04,0.0,-9.38],[-368.04,0.0,-166.49],[-25.38,0.0,-166.49]],"normals":[[0.0,1.0,0.0],[0.0,1.0,0.0]]},
    //   "wall_3044943331162771623":{"name":"__wall","tri_ids":[[0,1,2],[0,3,2],[1,4,5],[1,2,5],[4,6,7],[4,5,7],[6,8,7],[8,9,7],[9,10,3],[9,7,3],[0,11,3],[11,10,3],[8,9,12],[9,13,12],[9,10,13],[10,14,13],[11,10,15],[10,14,15]],"ver
    //   ts":[[-368.04,0.0,-9.38],[-368.04,0.0,-166.49],[-368.04,274.66,-166.49],[-368.04,27...

    public class AddObjectsFromTriangles : ICommand
    {
        public string GetToken => "add_objects_from_triangles";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var data =
                JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(
                    commandParametersString[0]);
            var outputData = new Dictionary<string, string>();
            foreach (var pair in data) {
                var originalName = pair.Key;
                var objectValues = pair.Value;
                var trisData = new List<int>();
                var vertsData = new List<Vector3>();
                var normalsData = new List<Vector3>();
                var uvsData = new List<Vector2>();
                var tangentsData = new List<Vector4>();
                var colorsData = new List<Color>();
                var materialPath = "";
                var targetMaterial = new Material(Shader.Find("Diffuse"));
                var name = Convert.ToString(objectValues["name"]);
                if (name == string.Empty) {
                    name = "PrometheanGameObject";
                }

                if (objectValues.ContainsKey("material")) {
                    materialPath = Convert.ToString(objectValues["material"]);
                    targetMaterial = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                    if (targetMaterial == null) {
                        targetMaterial = new Material(Shader.Find("Diffuse"));
                    }
                }

                var trianglesValues = new int[][] { };
                if (objectValues.ContainsKey("tri_ids")) {
                    var triangleIDs = objectValues["tri_ids"].ToString();
                    trianglesValues = JsonConvert.DeserializeObject<int[][]>(triangleIDs);
                    var index = 0;
                    foreach (var value in trianglesValues) {
                        foreach (var triangle in value) {
                            trisData.Add(triangle);
                        }
                    }
                }

                if (objectValues.ContainsKey("verts")) {
                    var verts = objectValues["verts"].ToString();
                    var vertsValues = JsonConvert.DeserializeObject<string[][]>(verts);
                    foreach (var vert in vertsValues) {
                        var newVert = CommandUtility.StringArrayToVector(vert);

                        vertsData.Add(new Vector3(CommandUtility.ConvertToMeters(newVert.x), CommandUtility.ConvertToMeters(newVert.y),
                            CommandUtility.ConvertToMeters(newVert.z)));
                    }
                }

                if (objectValues.ContainsKey("normals")) {
                    var normals = objectValues["normals"].ToString();
                    var normalsValues = JsonConvert.DeserializeObject<string[][]>(normals);
                    foreach (var vert in vertsData) {
                        normalsData.Add(CommandUtility.StringArrayToVector(normalsValues[0]));
                    }
                }

                if (objectValues.ContainsKey("uvs")) {
                    var uvs = objectValues["uvs"].ToString();
                    var uvsValues = JsonConvert.DeserializeObject<string[][]>(uvs);
                    foreach (var uv in uvsValues) {
                        var uveValue = new Vector2(
                            float.Parse(uv[0], CultureInfo.InvariantCulture.NumberFormat),
                            float.Parse(uv[1], CultureInfo.InvariantCulture.NumberFormat));
                        if (uveValue == Vector2.negativeInfinity) {
                            uveValue = Vector2.zero;
                        }

                        uvsData.Add(CommandUtility.StringArrayToVector(uv));
                    }
                }

                if (objectValues.ContainsKey("tangents")) {
                    var tangents = objectValues["tangents"].ToString();
                    var tangentsValues = JsonConvert.DeserializeObject<string[][]>(tangents);
                    foreach (var value in tangentsValues) {
                        var tangent = new Vector4(float.Parse(value[0], CultureInfo.InvariantCulture.NumberFormat),
                            float.Parse(value[1], CultureInfo.InvariantCulture.NumberFormat),
                            float.Parse(value[2], CultureInfo.InvariantCulture.NumberFormat),
                            float.Parse(value[3], CultureInfo.InvariantCulture.NumberFormat)
                        );
                        tangentsData.Add(tangent);
                    }
                }

                if (objectValues.ContainsKey("vcolors")) {
                    var colors = objectValues["vcolors"].ToString();
                    var colorValues = JsonConvert.DeserializeObject<string[][]>(colors);
                    foreach (var color in vertsData) {
                        var newColor = new Vector4(float.Parse(colorValues[0][0], CultureInfo.InvariantCulture.NumberFormat),
                            float.Parse(colorValues[0][1], CultureInfo.InvariantCulture.NumberFormat),
                            float.Parse(colorValues[0][2], CultureInfo.InvariantCulture.NumberFormat),
                            float.Parse(colorValues[0][3], CultureInfo.InvariantCulture.NumberFormat));

                        colorsData.Add(newColor);
                    }
                }

                TransformNormals(trianglesValues, trisData, vertsData, normalsData);
                TransformFaces(trisData, vertsData, normalsData);

                var mesh = new Mesh();
                mesh.Clear();
                mesh.SetVertices(vertsData);
                mesh.SetTriangles(trisData, 0);
                mesh.SetNormals(normalsData);

                if (uvsData.Count > 0) {
                    mesh.uv = uvsData.ToArray();
                }
                else {
                    Unwrapping.GenerateSecondaryUVSet(mesh);
                }

                if (tangentsData.Count > 0) {
                    mesh.tangents = tangentsData.ToArray();
                }
                else {
                    mesh.RecalculateTangents();
                }

                if (colorsData.Count > 0) {
                    mesh.colors = colorsData.ToArray();
                }

                var newGameObject = new GameObject(name);
                UndoUtility.RecordUndo(GetToken, newGameObject, true);
                newGameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
                newGameObject.AddComponent<MeshRenderer>().sharedMaterial = targetMaterial;
                outputData.Add(originalName, JsonOutputFormatUtility.GeneratePrometheanObjectName(newGameObject));
            }

            var output = JsonConvert.SerializeObject(outputData);
            callback.Invoke(CommandHandleProcessState.Success, output);
        }

        public static Vector3 CalculatePerpendicularNormal(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3) {
            return Vector3.Cross(vertex3 - vertex1, vertex2 - vertex1);
        }

        public static bool IsTriangleInverted(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3, Vector3 normal) {
            return Vector3.Dot(normal, CalculatePerpendicularNormal(vertex1, vertex2, vertex3)) < 0.0f;
        }

        public static void TransformFaces(List<int> triIds, List<Vector3> verts, List<Vector3> normals) {
            for (var i = 0; i + 2 < triIds.Count; i += 3) {
                if (IsTriangleInverted(verts[triIds[i]], verts[triIds[i + 1]], verts[triIds[i + 2]], normals[triIds[i]])) {
                    Debug.Log($"Triangle {i} is inverted");
                    var tri1 = triIds[i];
                    var tri2 = triIds[i + 2];
                    triIds[i] = tri2;
                    triIds[i + 2] = tri1;
                }
            }
        }

        public static void TransformNormals(int[][] trisRawData, List<int> triIds, List<Vector3> verts, List<Vector3> normals) {
            var newVertex = new List<Vector3>();
            var newTriIds = new List<int>();
            for (var triCount = 0; triCount < trisRawData.Length; triCount++) {
                var triangles = trisRawData[triCount];
                foreach (var triangle in triangles) {
                    var vertex = verts[triangle];
                    newVertex.Add(vertex);
                    newTriIds.Add(verts.Count - 1);
                    verts.Add(vertex);
                    triIds.Add(verts.Count - 1);

                    if (normals.Count > 0) {
                        var normal = normals[triCount];
                        normals.Add(normal);
                    }
                }
            }

            triIds = newTriIds;
            verts = newVertex;
        }
    }
}