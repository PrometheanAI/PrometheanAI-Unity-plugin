using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEngine.SceneManagement;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    //Called by command "build *object_name*"
    //Raw incoming command example : scene_name

    /// <summary>
    ///Used to get a current active Scene's name 
    /// </summary>
    public class SceneName : ICommand
    {
        public string GetToken => "scene_name";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            callback.Invoke(CommandHandleProcessState.Success,
                JsonOutputFormatUtility.GenerateJsonString(SceneManager.GetActiveScene().name));
        }
    }
}