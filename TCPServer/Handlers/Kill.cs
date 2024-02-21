using System;
using System.Collections.Generic;
using System.Linq;
using PrometheanAI.Modules.Utils;
using UnityEditor;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Adds Promethean ignore token to objects name
    /// </summary>
    public class Kill : ICommand
    {
        public string GetToken => "_kill_";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            var ignored = GetToken;
            var selectedValidObjects = CommandUtility.ValidateObjects(Selection.gameObjects.ToList());
            foreach (var obj in selectedValidObjects) {
                string newName;
                if (obj.name.Contains(ignored)) {
                    newName = obj.name.Replace(ignored, "");
                }
                else {
                    newName = obj.name + $" {ignored}";
                }

                UndoUtility.RecordUndo(GetToken, obj, false);
                obj.name = newName;
            }

            EditorUtils.RefreshEditorWindows();
            callback(CommandHandleProcessState.Success, string.Empty);
        }
    }
}