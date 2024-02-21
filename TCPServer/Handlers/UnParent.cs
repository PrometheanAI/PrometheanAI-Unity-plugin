using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEditor;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    //Called by command "unparent"
    //Raw incoming command example : unparent cube#-17994

    /// <summary>
    /// Used to remove a parent from a specified GameObject's transform 
    /// </summary>
    public class UnParent : ICommand
    {
        public string GetToken => "unparent";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var namedObjects = JsonOutputFormatUtility.ProduceParametersData(commandParametersString);
            var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            if (namedObjects.Count > 0 && namedObjects[0] != string.Empty) {
                foreach (var obj in allObjects) {
                    if (namedObjects.Contains(JsonOutputFormatUtility.GeneratePrometheanObjectName(obj))
                        && obj.transform.parent != null && obj.scene.IsValid()) {
                        UndoUtility.RecordParenting(GetToken, obj, null);
                        obj.transform.parent = null;
                    }
                }
            }
            else {
                foreach (var obj in Selection.gameObjects) {
                    if (obj.transform.parent != null && obj.scene.IsValid()) {
                        UndoUtility.RecordParenting(GetToken, obj, null);
                        obj.transform.parent = null;
                    }
                }
            }

            EditorUtils.RefreshEditorWindows();
            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}