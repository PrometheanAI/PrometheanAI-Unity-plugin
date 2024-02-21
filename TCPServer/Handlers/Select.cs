using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    //Called by command "select" 
    //Raw incoming command example : select box#-1304
    /// <summary>
    /// Used to select specified Objects in a Scene
    /// </summary>
    public class Select : ICommand
    {
        public string GetToken => "select";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var namedObjects = JsonOutputFormatUtility.ProduceParametersData(commandParametersString);
            var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            var objectToSelect = new List<Object>();
            foreach (var obj in allObjects) {
                if (namedObjects.Contains(JsonOutputFormatUtility.GeneratePrometheanObjectName(obj))
                    && obj.scene.IsValid() ||
                    namedObjects.Contains(obj.name) && obj.scene.IsValid()) {
                    objectToSelect.Add(obj);
                }
            }

            UndoUtility.RecordUndo(GetToken, Selection.objects, false);
            Selection.objects = objectToSelect.ToArray();
            EditorUtils.RefreshEditorWindows();
            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}