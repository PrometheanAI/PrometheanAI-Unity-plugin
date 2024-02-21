using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PrometheanAI.Modules.Utils;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer
{
    /// <summary>
    /// Assign given transform data for GameObjects with given names in Scene
    /// </summary>
    public class TransformHandler : ICommand
    {
        public string GetToken => "transform";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 2) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var values = commandParametersString[0].Split(',').ToList();
            var targetNames = commandParametersString[1].Split(',').ToList();
            var dummy = new GameObject();
            if (values.Count > 8) {
                dummy.transform.position = new Vector3(
                    float.Parse(values[0], CultureInfo.InvariantCulture.NumberFormat),
                    float.Parse(values[1], CultureInfo.InvariantCulture.NumberFormat),
                    float.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat));
                dummy.transform.rotation = Quaternion.Euler(new Vector3(
                    float.Parse(values[3], CultureInfo.InvariantCulture.NumberFormat),
                    float.Parse(values[4], CultureInfo.InvariantCulture.NumberFormat),
                    float.Parse(values[5], CultureInfo.InvariantCulture.NumberFormat)));
                dummy.transform.localScale = new Vector3(
                    float.Parse(values[6], CultureInfo.InvariantCulture.NumberFormat),
                    float.Parse(values[7], CultureInfo.InvariantCulture.NumberFormat),
                    float.Parse(values[8], CultureInfo.InvariantCulture.NumberFormat));
                var allSceneObjects = CommandUtility.GetAllValidObjectsFromScene();
                dummy.transform.position = CommandUtility.ConvertToMetersVector(dummy.transform.position);
                foreach (var obj in allSceneObjects) {
                    foreach (var name in targetNames) {
                        if (JsonOutputFormatUtility.GeneratePrometheanObjectName(obj) == name) {
                            UndoUtility.RecordUndo(GetToken, obj.transform, false);
                            obj.transform.position = dummy.transform.position;
                            obj.transform.rotation = dummy.transform.rotation;
                            obj.transform.localScale = dummy.transform.localScale;
                        }
                    }
                }

                EditorUtils.RefreshEditorWindows();
                callback.Invoke(CommandHandleProcessState.Success, string.Empty);
            }
            else {
                Debug.Log("Not enough numbers in transform");
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
            }
        }
    }
}