using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer
{
    //Called by command "parent" "scale"
    //Raw incoming command example : command parent floor#13490,cube#-17994
    // where floor#13490 is a name of a desired parent

    /// <summary>
    /// Used to parent one GameObject to another
    /// </summary>
    public class Parent : ICommand
    {
        public string GetToken => "parent";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var namedObjects = JsonOutputFormatUtility.ProduceParametersData(commandParametersString);
            var parentName = namedObjects[0];
            namedObjects.RemoveAt(0);
            var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            GameObject parent = null;
            foreach (var obj in allObjects) {
                if (JsonOutputFormatUtility.GeneratePrometheanObjectName(obj) == parentName && obj.scene.IsValid()
                    || obj.name == parentName && obj.scene.IsValid()) {
                    parent = obj;
                }
            }

            if (parent != null) {
                foreach (GameObject obj in allObjects) {
                    if (namedObjects.Contains(JsonOutputFormatUtility.GeneratePrometheanObjectName(obj))
                        && obj.scene.IsValid()) {
                        UndoUtility.RecordParenting(GetToken, obj, parent);
                        obj.transform.parent = parent.transform;
                    }
                }
            }

            EditorUtils.RefreshEditorWindows();
            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}