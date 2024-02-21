using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    //Called by command "unhide"
    //Raw incoming command example : set_visible ball#-13602

    /// <summary>
    /// Used to enable specified GameObjects in Scene
    /// </summary>
    public class SetVisible : ICommand
    {
        public string GetToken => "set_visible";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var namedObjects = JsonOutputFormatUtility.ProduceParametersData(commandParametersString);
            var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (var obj in allObjects) {
                if (namedObjects.Contains(JsonOutputFormatUtility.GeneratePrometheanObjectName(obj))
                    && !obj.activeInHierarchy ||
                    namedObjects.Contains(obj.name) && !obj.activeInHierarchy) {
                    UndoUtility.RecordUndoForHierarchy(GetToken, obj);
                    obj.SetActive(true);
                }
            }

            EditorUtils.RefreshEditorWindows();
            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}