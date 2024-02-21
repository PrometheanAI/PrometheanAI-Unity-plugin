using System;
using System.Collections.Generic;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Makes a screenshot of a Game View and saves it to a given location
    /// </summary>
    public class Screenshot : ICommand
    {
        public string GetToken => "screenshot";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var folderPath = commandParametersString[0];
            if (!System.IO.Directory.Exists(folderPath)) {
                System.IO.Directory.CreateDirectory(folderPath);
            }

            var screenshotName =
                "Screenshot_" +
                DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") +
                ".png";
            ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(folderPath, screenshotName));
            callback.Invoke(CommandHandleProcessState.Success, "Done");
        }
    }
}