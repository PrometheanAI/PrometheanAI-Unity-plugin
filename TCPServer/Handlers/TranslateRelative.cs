using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PrometheanAI.Modules.Utils;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer
{
    /// <summary>
    /// Used to change a local space position values of a GameObject to a specified ones
    /// </summary>
    public class TranslateRelative : ICommand
    {
        public string GetToken => "translate_relative";

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
                float.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat));
            vector = CommandUtility.ConvertToMetersVector(vector);
            var allSceneObjects = CommandUtility.GetAllValidObjectsFromScene();
            foreach (var obj in allSceneObjects) {
                foreach (var name in targetNames) {
                    if (JsonOutputFormatUtility.GeneratePrometheanObjectName(obj) == name
                        || obj.name == name) {
                        UndoUtility.RecordUndo(GetToken, obj.transform, false);
                        obj.transform.localPosition = vector;
                    }
                }
            }

            EditorUtils.RefreshEditorWindows();
            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}