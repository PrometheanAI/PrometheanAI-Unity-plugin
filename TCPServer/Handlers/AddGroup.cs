using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Adds empty object to the Scene based on incoming data
    /// </summary>
    public class AddGroup : ICommand
    {
        public string GetToken => "add_group";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var name = commandParametersString[0];
            var position = Vector3.zero;
            var rotation = Vector3.zero;
            var scale = Vector3.one;
            if (commandParametersString.Count > 1) {
                var positionsData = commandParametersString[1].Split(',');
                var positionVector = CommandUtility.StringArrayToVector(positionsData);
                position = CommandUtility.ConvertToMetersVector(positionVector);
            }

            if (commandParametersString.Count > 2) {
                var rotationData = commandParametersString[2].Split(',');
                rotation = CommandUtility.StringArrayToVector(rotationData);
            }

            if (commandParametersString.Count > 3) {
                var scaleData = commandParametersString[3].Split(',');
                scale = CommandUtility.StringArrayToVector(scaleData);
            }

            var newObject = new GameObject();
            if (newObject != null) {
                UndoUtility.RecordUndo(GetToken, newObject, true);
                newObject.name = name;
                newObject.transform.position = position;
                newObject.transform.rotation = Quaternion.Euler(rotation);
                newObject.transform.localScale = scale;
            }

            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}