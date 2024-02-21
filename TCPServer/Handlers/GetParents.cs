using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Used to get parents names of a given GameObjects
    /// </summary>
    public class GetParents : ICommand
    {
        public string GetToken => "get_parents";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var namedObjects = JsonOutputFormatUtility.ProduceParametersData(commandParametersString);
            namedObjects = JsonOutputFormatUtility.SplitStringOnNewLine(namedObjects);
            var parentsNames = new List<string>();
            var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (var obj in allObjects) {
                if (namedObjects.Contains(JsonOutputFormatUtility.GeneratePrometheanObjectName(obj))
                    && obj.transform.parent != null && obj.scene.IsValid()
                    || namedObjects.Contains(obj.name) && obj.transform.parent != null && obj.scene.IsValid()) {
                    parentsNames.Add(
                        JsonOutputFormatUtility.GeneratePrometheanObjectName(obj.transform.parent.gameObject));
                }
            }

            var combinedNames = string.Empty;
            foreach (var parentsName in parentsNames) {
                combinedNames += parentsName + ",";
            }

            combinedNames = combinedNames.Remove(combinedNames.Length - 1, 1);
            combinedNames = JsonOutputFormatUtility.ReplaceEmptySpaces(combinedNames);
            var output = JsonOutputFormatUtility.GenerateJsonString(combinedNames);
            callback.Invoke(CommandHandleProcessState.Success, output);
        }
    }
}