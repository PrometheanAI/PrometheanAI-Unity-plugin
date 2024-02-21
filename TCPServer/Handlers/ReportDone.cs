using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    //called automatically by some commands
    //Raw incoming command example : report_done

    /// <summary>
    /// used to send basic answer to PrometheanAI
    /// </summary>
    public class ReportDone : ICommand
    {
        public string GetToken => "report_done";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            UndoUtility.CollapseUndo();
            callback.Invoke(CommandHandleProcessState.Success, "Done");
        }
    }
}