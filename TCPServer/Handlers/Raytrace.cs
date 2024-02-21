using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Performs raycast from given point in given direction and sends back hit location
    /// </summary>
    public class Raytrace : ICommand
    {
        public string GetToken => "raytrace";

        public void Handle(string rawCommandData, List<string> commandParametersString, Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 2) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var offsetData = commandParametersString[0].Split(',');
            var offsetVector = CommandUtility.StringArrayToVector(offsetData);
            offsetVector = CommandUtility.ConvertToMetersVector(offsetVector);
            var names = commandParametersString[1].Split(',');
            var allSceneObjects = CommandUtility.GetAllValidObjectsFromScene();
            var outData = new Dictionary<string, float[]>();
            foreach (var obj in allSceneObjects) {
                foreach (var name in names) {
                    if (obj.name == name || JsonOutputFormatUtility.GeneratePrometheanObjectName(obj) == name) {
                        obj.layer = LayerMask.NameToLayer("Ignore Raycast");
                        var startPosition = obj.transform.position;
                        startPosition.z -= obj.GetComponent<Renderer>().bounds.size.z / 2;
                        var endPosition = startPosition + offsetVector;
                        if (Physics.Raycast(startPosition, endPosition, out var hit)) {
                            var convertedPoint = CommandUtility.ConvertToCentimetersVector(hit.point);
                            outData.Add(name, new[] {
                                CommandUtility.Round(convertedPoint.x, 4),
                                CommandUtility.Round(convertedPoint.y, 4),
                                CommandUtility.Round(convertedPoint.z, 4)
                            });
                        }

                        obj.layer = LayerMask.NameToLayer("Default");
                        break;
                    }
                }
            }

            var output = JsonOutputFormatUtility.GenerateJsonString(outData);
            callback.Invoke(CommandHandleProcessState.Success, output);
        }
    }
}