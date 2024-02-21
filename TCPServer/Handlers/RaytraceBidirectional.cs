using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Performs raycast from given point in given directions and sends back hit location
    /// </summary>
    public class RaytraceBidirectional : ICommand
    {
        public string GetToken => "raytrace_bidirectional";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 2) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var offsetData = commandParametersString[0].Split(',');
            var offsetVector = CommandUtility.StringArrayToVector(offsetData) / 100;
            var names = commandParametersString[1].Split(',');
            var allSceneObjects = CommandUtility.GetAllValidObjectsFromScene();
            var outData = new Dictionary<string, float[]>();
            foreach (var obj in allSceneObjects) {
                foreach (var name in names) {
                    if (obj.name == name || JsonOutputFormatUtility.GeneratePrometheanObjectName(obj) == name) {
                        var objectsCenter = obj.transform.position;
                        var render = obj.GetComponent<Renderer>();
                        if (render == null) {
                            render = obj.GetComponentInChildren<Renderer>();
                        }

                        if (render != null) {
                            objectsCenter.y -= obj.GetComponent<Renderer>().bounds.size.y / 2;
                        }

                        var currentBestLocation = obj.transform.position;
                        obj.layer = LayerMask.NameToLayer("Ignore Raycast");
                        if (Physics.Raycast(objectsCenter, offsetVector, out var hit)) {
                            if (hit.collider.gameObject != obj) {
                                currentBestLocation = hit.point;
                            }

                            currentBestLocation = CommandUtility.ConvertToCentimetersVector(currentBestLocation);
                            outData.Add(name, new[] {
                                CommandUtility.Round(currentBestLocation.x, 4),
                                CommandUtility.Round(currentBestLocation.y, 4),
                                CommandUtility.Round(currentBestLocation.z, 4)
                            });

                            var output = JsonOutputFormatUtility.GenerateJsonString(outData);
                            callback.Invoke(CommandHandleProcessState.Success, output);
                            obj.layer = LayerMask.NameToLayer("Default");
                            return;
                        }

                        obj.layer = LayerMask.NameToLayer("Default");
                        callback.Invoke(CommandHandleProcessState.Success, string.Empty);
                    }
                }
            }
        }
    }
}