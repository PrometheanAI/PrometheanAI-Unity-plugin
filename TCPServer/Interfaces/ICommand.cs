using System;
using System.Collections.Generic;

namespace PrometheanAI.Modules.TCPServer
{
    public interface ICommand
    {
        /// <summary>
        /// Used to differ command handlers
        /// </summary>
        string GetToken { get; }

        /// <summary>
        /// Used to handle command
        /// </summary>
        /// <param name="rawCommandData"></param>
        /// <param name="commandParametersString"></param>
        /// <param name="callback"></param>
        void Handle(string rawCommandData, List<string> commandParametersString, Action<CommandHandleProcessState, string> callback);
    }
}