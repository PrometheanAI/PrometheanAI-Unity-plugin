using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PrometheanAI.Modules.Utils;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Performs raycasts from specified GameObjects and assigning them new positions depending on hit information 
    /// </summary>
    public class TranslateAndRaytrace : ICommand
    {
        public string GetToken => "translate_and_raytrace";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 4) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var targetNames = commandParametersString[2].Split(',').ToList();
            var ignoredNames = commandParametersString[3].Split(',').ToList();
            var raycastDistance = float.Parse(commandParametersString[1], CultureInfo.InvariantCulture.NumberFormat);
            float normalDeviation = 0;
            var newLocation = commandParametersString[0].Split(',');
            var locationVector = CommandUtility.StringArrayToVector(newLocation);
            locationVector = CommandUtility.ConvertToMetersVector(locationVector);
            var allSceneObjects = CommandUtility.GetAllValidObjectsFromScene();
            var ignoredObjects = new List<GameObject>();
            foreach (var obj in allSceneObjects) {
                foreach (var name in ignoredNames) {
                    if (JsonOutputFormatUtility.GeneratePrometheanObjectName(obj) == name) {
                        ignoredObjects.Add(obj);
                        break;
                    }
                }
            }

            foreach (var obj in ignoredObjects) {
                obj.layer = LayerMask.NameToLayer("Ignore Raycast");
            }

            var targetObjects = new List<GameObject>();
            foreach (var obj in allSceneObjects) {
                foreach (var name in targetNames) {
                    if (JsonOutputFormatUtility.GeneratePrometheanObjectName(obj) == name
                        || obj.name == name) {
                        targetObjects.Add(obj);
                        break;
                    }
                }
            }

            foreach (var obj in targetObjects) {
                var rayDirection = new Vector3(0, raycastDistance, 0);
                var startPoint = locationVector;
                var endPoint = locationVector - rayDirection;
                RaycastHit hit;
                UndoUtility.RecordUndo(GetToken, obj.transform, false);
                var newPosition = locationVector;
                if (Physics.Raycast(startPoint, endPoint, out hit)) {
                    newPosition = hit.point;
                    newPosition.y += UnityEngine.Random.Range(0, 0.25f);

                    var dotProduct = Vector3.Dot(Vector3.up, hit.normal);
                    if (dotProduct > normalDeviation) {
                        var rotationFromNormal = Quaternion.FromToRotation(obj.transform.up, hit.normal);
                        obj.transform.rotation = rotationFromNormal * obj.transform.rotation;
                    }
                }

                obj.transform.position = newPosition;
            }

            foreach (var obj in ignoredObjects) {
                obj.layer = LayerMask.NameToLayer("Default");
            }

            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}