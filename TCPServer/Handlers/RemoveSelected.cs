using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEditor;
using Object = UnityEngine.Object;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Deletes selected objects
    /// </summary>
    public class RemoveSelected : ICommand
    {
        public string GetToken => "remove_selected";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            UndoUtility.RecordUndo(GetToken, Selection.objects, true);
            foreach (var obj in Selection.objects) {
                Object.DestroyImmediate(obj);
            }

            EditorUtils.RefreshEditorWindows();
            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}