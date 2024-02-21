using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEditor;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    //Called by command "deselect"
    //Raw incoming command example : clear_selection

    /// <summary>
    /// Used to clear the Selection.objects
    /// </summary>
    public class ClearSelection : ICommand
    {
        public string GetToken => "clear_selection";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            UndoUtility.RecordUndo(GetToken, Selection.objects, false);
            Selection.objects = null;
            EditorUtils.RefreshEditorWindows();
            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}