using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    //Called by command "look at"
    //Raw incoming command example : focus box#-1304

    /// <summary>
    /// Used to focus camera on a specific GameObject in SceneView
    /// </summary>
    public class Focus : ICommand
    {
        public string GetToken => "focus";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var namedObjects = JsonOutputFormatUtility.ProduceParametersData(commandParametersString);
            namedObjects = JsonOutputFormatUtility.SplitStringOnNewLine(namedObjects);
            var objectsInScene = Resources.FindObjectsOfTypeAll<GameObject>();
            var objectToSelect = new List<Object>();
            foreach (var obj in objectsInScene) {
                if (namedObjects.Contains(JsonOutputFormatUtility.GeneratePrometheanObjectName(obj))
                    && obj.scene.IsValid() || namedObjects.Contains(obj.name) && obj.scene.IsValid()) {
                    objectToSelect.Add(obj);
                }
            }

            Selection.objects = objectToSelect.ToArray();
            UndoUtility.RecordUndo(GetToken, SceneView.lastActiveSceneView, false);
            SceneView.lastActiveSceneView.FrameSelected();
            EditorUtils.RefreshEditorWindows();
            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}