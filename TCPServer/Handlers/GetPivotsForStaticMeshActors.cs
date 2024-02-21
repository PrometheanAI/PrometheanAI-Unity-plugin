using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer
{
    /// <summary>
    /// Sends to Promethean Pivot offset data for specified GameObjects with their corresponding names as keys for it
    /// </summary>
    public class GetPivotsForStaticMeshActors : ICommand
    {
        public string GetToken => "get_pivots_for_static_mesh_actors";

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
                    }
                }
            }

            var gatheredData = new Dictionary<string, object[]>();
            foreach (var obj in sortedGameObjects) {
                var pivotOffset = CommandUtility.GetPivotOffset(obj);
                object[] newData = {pivotOffset.x, pivotOffset.y, pivotOffset.z};
                gatheredData.Add(JsonOutputFormatUtility.GeneratePrometheanObjectName(obj), newData);
            }

            var output = gatheredData.Count == 0
                ? JsonOutputFormatUtility.GenerateJsonString("None")
                : JsonOutputFormatUtility.GenerateJsonString(gatheredData);
            callback.Invoke(CommandHandleProcessState.Success, output);
        }
    }
}