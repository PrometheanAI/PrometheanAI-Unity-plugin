using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEngine.SceneManagement;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    //called by command "memorize"
    //Raw incoming command example : learn C:/Users/UserName/Documents/PrometheanAI/temp/learning_cache_memorize.json

    /// <summary>
    /// Used to provide data needed to execute PrometheanAI Learn functionality
    /// </summary>
    public class Learn : ICommand
    {
        public string GetToken => "learn";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var sceneName = SceneManager.GetActiveScene().name;
            var path = "";
            switch (commandParametersString[0]) {
                case "selection":
                    path = rawCommandData.Replace("learn selection ", "");
                    LearningUtility.LearnSelection(sceneName, path);
                    break;
                case "view":
                    path = rawCommandData.Replace("learn view ", "");
                    LearningUtility.LearnView(sceneName, path);
                    break;
                default:
                    path = rawCommandData.Replace("learn ", "");
                    LearningUtility.Learn(sceneName, path);
                    break;
            }

            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}