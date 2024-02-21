using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PrometheanAI.Modules.Utils;
using UnityEngine;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    //Called by command "copy"
    //Raw incoming command example: add_objects {"cube_scatter":{"name":"cube_scatter","asset_path":null,"group":true,
    //"location":[1036.0,141.82,760.0],"rotation":[0.0,0.0,0.0],"scale":[1.0,1.0,1.0],
    //"parent_dcc_name":"","raytrace_distance":0,"raytrace_alignment":0,"raytrace_alignment_mask":0}}

    /// <summary>
    /// Used to add new object to a scene based on incoming data
    /// </summary>
    public class AddObjects : ICommand
    {
        public string GetToken => "add_objects";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var obj =
                JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(
                    commandParametersString[0]);
            var outData = new Dictionary<string, string>();
            if (obj != null) {
                foreach (var pair in obj) {
                    var objectValues = pair.Value;
                    var name = (string) objectValues["name"];
                    var path = (string) objectValues["asset_path"];
                    path = path?.Replace("\\", "");
                    var group = (bool) objectValues["group"];
                    var location = Convert.ToString(objectValues["location"]).Replace("[", "").Replace("]", "")
                        .Replace("\n", "");
                    var locationValues = location.Split(',');
                    var locationVector = CommandUtility.StringArrayToVector(locationValues);
                    locationVector = CommandUtility.ConvertToMetersVector(locationVector);
                    var rotation = Convert.ToString(objectValues["rotation"]).Replace("[", "").Replace("]", "")
                        .Replace("\n", "");
                    var rotationValues = rotation.Split(',');
                    var rotationVector = CommandUtility.StringArrayToVector(rotationValues);
                    var scale = Convert.ToString(objectValues["scale"]).Replace("[", "").Replace("]", "")
                        .Replace("\n", "");
                    var scaleValues = scale.Split(',');
                    var scaleVector = CommandUtility.StringArrayToVector(scaleValues);
                    var parentName = (string) objectValues["parent_dcc_name"];
                    var raycastDistance = Convert.ToInt32(objectValues["raytrace_distance"]);
                    var raycastAlignment = Convert.ToSingle(objectValues["raytrace_alignment"], CultureInfo.InvariantCulture.NumberFormat);
                    var raycastMask = Convert.ToSingle(objectValues["raytrace_alignment_mask"], CultureInfo.InvariantCulture.NumberFormat);
                    var rotationFromNormal = new Quaternion();
                    if (raycastDistance > 0) {
                        var rayDirection = new Vector3(0, raycastDistance, 0);
                        var startPoint = locationVector+Vector3.up*2;
                        var endPoint = locationVector - rayDirection;
                        var newPosition = locationVector;
                        if (Physics.Raycast(startPoint, endPoint, out var hit)) {
                            newPosition = hit.point;
                            newPosition.y += Random.Range(0, 0.25f);

                            var dotProduct = Vector3.Dot(Vector3.up, hit.normal);
                            if (dotProduct < raycastMask) {
                                continue;
                            }

                            if (dotProduct > raycastAlignment) {
                                rotationFromNormal = Quaternion.FromToRotation(Vector3.up, hit.normal);
                            }
                        }

                        locationVector = newPosition;
                    }

                    GameObject newObject;
                    if (group) {
                        newObject = new GameObject();
                    }
                    else if (path != null && !path.Equals("Null")) {
                        newObject =
                            (GameObject) PrefabUtility.InstantiatePrefab(
                                AssetDatabase.LoadAssetAtPath<GameObject>(path));
                    }
                    else {
                        newObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    }

                    if (newObject != null) {
                        UndoUtility.RecordUndo(GetToken, newObject, true);
                        newObject.name = name;
                        var allObjects = SceneManager.GetActiveScene().GetRootGameObjects().ToList();
                        allObjects = CommandUtility.AddChildObjects(allObjects);
                        newObject.transform.position = locationVector;
                        newObject.transform.rotation = Quaternion.Euler(rotationVector) * rotationFromNormal;
                        newObject.transform.localScale = scaleVector;
                        if (parentName != string.Empty) {
                            foreach (var sceneObject in allObjects) {
                                if (sceneObject.name == parentName ||
                                    parentName == JsonOutputFormatUtility.GeneratePrometheanObjectName(sceneObject)) {
                                    newObject.transform.parent = sceneObject.transform;
                                    break;
                                }
                            }
                        }

                        outData.Add(pair.Key, JsonOutputFormatUtility.GeneratePrometheanObjectName(newObject));
                    }
                }

                UndoUtility.CollapseUndo();
                EditorUtils.RefreshEditorWindows();
                var output = JsonOutputFormatUtility.GenerateJsonString(outData);
                callback.Invoke(CommandHandleProcessState.Success, output);
            }
        }
    }
}