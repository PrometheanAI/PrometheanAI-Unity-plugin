using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PrometheanAI.Modules.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    //Called by command "Scatter"
    //Raw incoming command example : translate_and_snap 1695.1881018413235,135.319992,931.793507743835 800 0.65 ball#-17378 

    /// <summary>
    /// Used to change a world space position values of a GameObject to a specified ones
    /// and snap to a surface below if possible
    /// </summary>
    public class TranslateAndSnap : ICommand
    {
        public string GetToken => "translate_and_snap";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 5) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var targetNames = commandParametersString[3].Split(',').ToList();
            var ignoredNames = commandParametersString[4].Split(',').ToList();
            var raycastDistance = float.Parse(commandParametersString[1], CultureInfo.InvariantCulture.NumberFormat);
            var normalDeviation = float.Parse(commandParametersString[2], CultureInfo.InvariantCulture.NumberFormat);
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
                var newPosition = locationVector;
                if (Physics.Raycast(startPoint, endPoint, out hit)) {
                    newPosition = hit.point;
                    newPosition.y += Random.Range(0, 0.25f);
                    UndoUtility.RecordUndo(GetToken, obj.transform, false);
                    var dotProduct = Vector3.Dot(Vector3.up, hit.normal);
                    if (dotProduct > normalDeviation) {
                        obj.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
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