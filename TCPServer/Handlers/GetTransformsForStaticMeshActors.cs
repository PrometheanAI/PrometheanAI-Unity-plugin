using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    //Called by hovering over Promethean bar
    //Raw incoming command example : get_transforms_for_static_mesh_actors test#13474,box#-1304

    /// <summary>
    /// Used to gather Transform data for specified GameObjects and send it back to PrometheanAI
    /// </summary>
    public class GetTransformsForStaticMeshActors : ICommand
    {
        public string GetToken => "get_transforms_for_static_mesh_actors";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var allSceneObjects = CommandUtility.GetAllValidObjectsFromScene();
            var names = JsonOutputFormatUtility.ProduceParametersData(commandParametersString);
            var sortedGameObjects = new List<GameObject>();
            foreach (var obj in allSceneObjects) {
                foreach (var name in names) {
                    if (JsonOutputFormatUtility.GeneratePrometheanObjectName(obj) == name
                        || obj.name == name) {
                        sortedGameObjects.Add(obj);
                        break;
                    }
                }
            }

            var gatheredData = new Dictionary<string, object[]>();
            for (var index = 0; index < sortedGameObjects.Count; index++) {
                var obj = sortedGameObjects[index];
                var objTransform = obj.transform;
                var position = objTransform.position;
                var positionRounded = new Vector3(CommandUtility.Round(position.x, 4),
                    CommandUtility.Round(position.y, 4),
                    CommandUtility.Round(position.z, 4));
                positionRounded = CommandUtility.ConvertToCentimetersVector(positionRounded);
                var rotation = objTransform.eulerAngles;
                var rotationRounded = new Vector3(CommandUtility.Round(rotation.x, 4),
                    CommandUtility.Round(rotation.y, 4),
                    CommandUtility.Round(rotation.z, 4));
                var parent = objTransform.parent;
                var scale = objTransform.lossyScale;
                var roundedScale = new Vector3(CommandUtility.Round(scale.x, 4),
                    CommandUtility.Round(scale.y, 4),
                    CommandUtility.Round(scale.z, 4));
                var size = CommandUtility.GetObjectsSize(obj);
                var sizeRounded = new Vector3(CommandUtility.Round(size.x, 4),
                    CommandUtility.Round(size.y, 4),
                    CommandUtility.Round(size.z, 4));
                var pivotOffset = CommandUtility.GetPivotOffset(obj);
                var pivotOffsetRounded = new Vector3(CommandUtility.Round(pivotOffset.x, 4),
                    CommandUtility.Round(pivotOffset.y, 4),
                    CommandUtility.Round(pivotOffset.z, 4));
                var parentsName = objTransform.parent != null
                    ? JsonOutputFormatUtility.GeneratePrometheanObjectName(parent.gameObject)
                    : "no_parent";

                object[] newData = {
                    positionRounded.x, positionRounded.y, positionRounded.z,
                    rotationRounded.x, rotationRounded.y, rotationRounded.z,
                    roundedScale.x, roundedScale.y, roundedScale.z,
                    sizeRounded.x, sizeRounded.y, sizeRounded.z,
                    pivotOffsetRounded.x, pivotOffsetRounded.y, pivotOffsetRounded.z,
                    parentsName
                };
                var outputName = obj.name;
                if (outputName != names[index]) {
                    outputName = JsonOutputFormatUtility.GeneratePrometheanObjectName(obj);
                }

                gatheredData.Add(outputName, newData);
            }

            var output = JsonOutputFormatUtility.GenerateJsonString(gatheredData);
            callback.Invoke(CommandHandleProcessState.Success, output);
        }
    }
}