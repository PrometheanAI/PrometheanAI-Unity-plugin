using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEditor;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    //Called by command "rename"
    //Raw incoming command example : rename cube#-13602 ball
    //where *ball* is a new name

    /// <summary>
    /// Used to change a name of a GameObject to a specified one
    /// </summary>
    public class Rename : ICommand
    {
        public string GetToken => "rename";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 2) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var targetName = commandParametersString[1];
            foreach (var obj in Selection.gameObjects) {
                UndoUtility.RecordUndo(GetToken, obj, false);
                obj.name = targetName;
            }

            EditorUtils.RefreshEditorWindows();
            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}