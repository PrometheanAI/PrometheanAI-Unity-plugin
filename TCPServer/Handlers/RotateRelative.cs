using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PrometheanAI.Modules.Utils;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer
{
    //Called by command "rotate" 
    //Raw incoming command example : rotate_relative 0.0,-29.999999999999993,-0.0 ball#-13602

    /// <summary>
    /// Used to change a local space rotation values of a GameObject to a specified ones
    /// </summary>
    public class RotateRelative : ICommand
    {
        public string GetToken => "rotate_relative";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 2) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var values = commandParametersString[0].Split(',').ToList();
            var targetNames = commandParametersString[1].Split(',').ToList();
            var vector = new Vector3(
                float.Parse(values[0], CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(values[1], CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat)
            );
            var allSceneObjects = CommandUtility.GetAllValidObjectsFromScene();
            foreach (var obj in allSceneObjects) {
                foreach (var name in targetNames) {
                    if (JsonOutputFormatUtility.GeneratePrometheanObjectName(obj) == name
                        || obj.name == name) {
                        UndoUtility.RecordUndo(GetToken, obj.transform, false);
                        obj.transform.Rotate(vector);
                    }
                }
            }

            EditorUtils.RefreshEditorWindows();
            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}