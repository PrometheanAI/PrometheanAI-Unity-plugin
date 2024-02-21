using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    //Called by command "hide"
    //Raw incoming command example : set_hidden ball#-13602

    /// <summary>
    /// Used to disable specified GameObjects in Scene
    /// </summary>
    public class SetHidden : ICommand
    {
        public string GetToken => "set_hidden";

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
                    && obj.activeInHierarchy ||
                    namedObjects.Contains(obj.name) && obj.activeInHierarchy) {
                    UndoUtility.RecordUndoForHierarchy(GetToken, obj);
                    obj.SetActive(false);
                }
            }

            EditorUtils.RefreshEditorWindows();
            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}