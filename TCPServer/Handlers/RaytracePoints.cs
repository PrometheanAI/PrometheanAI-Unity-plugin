using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Performs raycast from given points in given direction and sends back hit location
    /// </summary>
    public class RaytracePoints : ICommand
    {
        public string GetToken => "raytrace_points";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 2) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var directionData = commandParametersString[0].Split(',');
            var directionVector = CommandUtility.StringArrayToVector(directionData);
            directionVector = CommandUtility.ConvertToMetersVector(directionVector);
            var ignoredNames = commandParametersString[1].Split(',');
            var ignoredObjects = new List<GameObject>();
            var allSceneObjects = CommandUtility.GetAllValidObjectsFromScene();
            foreach (var obj in allSceneObjects) {
                foreach (var name in ignoredNames) {
                    if (obj.name == name || JsonOutputFormatUtility.GeneratePrometheanObjectName(obj) == name) {
                        ignoredObjects.Add(obj);
                        break;
                    }
                }
            }

            commandParametersString.RemoveAt(0);
            commandParametersString.RemoveAt(0);
            var startPoints = new List<Vector3>();
            foreach (var data in commandParametersString) {
                var value = data.Replace("[", "").Replace("]", "").Replace(" ", "");
                var valueArray = value.Split(',');
                var vectorValue = CommandUtility.StringArrayToVector(valueArray);
                vectorValue = CommandUtility.ConvertToMetersVector(vectorValue);
                startPoints.Add(vectorValue);
            }

            foreach (var obj in ignoredObjects) {
                obj.layer = LayerMask.NameToLayer("Ignore Raycast");
            }

            var outData = new Dictionary<string, float[]>();
            var index = 0;
            foreach (var point in startPoints) {
                var endPoint = point + directionVector;
                RaycastHit hit;
                if (Physics.Raycast(point, endPoint, out hit)) {
                    index++;
                    var convertedPoint = CommandUtility.ConvertToCentimetersVector(hit.point);
                    outData.Add(index.ToString(), new[] {
                        CommandUtility.Round(convertedPoint.x, 4),
                        CommandUtility.Round(convertedPoint.y, 4),
                        CommandUtility.Round(convertedPoint.z, 4)
                    });
                }
            }

            foreach (var obj in ignoredObjects) {
                obj.layer = LayerMask.NameToLayer("Default");
            }

            var output = JsonOutputFormatUtility.GenerateJsonString(outData);
            callback.Invoke(CommandHandleProcessState.Success, output);
        }
    }
}