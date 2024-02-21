using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    //Called by command "remove"
    //Raw incoming command example : remove box#-10666

    /// <summary>
    /// Used to delete specified GameObject from a Scene
    /// </summary>
    public class Remove : ICommand
    {
        public string GetToken => "remove";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var namedObjects = JsonOutputFormatUtility.ProduceParametersData(commandParametersString);
            var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (var obj in allObjects) {
                if (namedObjects.Contains(JsonOutputFormatUtility.GeneratePrometheanObjectName(obj)) &&
                    obj.scene.IsValid() || namedObjects.Contains(obj.name) && obj.scene.IsValid()) {
                    if (obj != null) {
                        foreach (Transform child in obj.transform) {
                            UndoUtility.RecordParenting(GetToken, child.gameObject, obj);
                        }

                        obj.transform.DetachChildren();
                        UndoUtility.RegisterObjectDestruction(GetToken, obj);
                    }
                }
            }

            EditorUtils.RefreshEditorWindows();
            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}