using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    //Called by command "simulate"
    //Raw incoming command example : get_simulation_on_actors_by_name cube#-3944

    /// <summary>
    /// Used to get a state of a specified objects regarding their ability to interact with Physics engine
    /// </summary>
    public class GetSimulationOnActorsByName : ICommand
    {
        public string GetToken => "get_simulation_on_actors_by_name";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var names = commandParametersString[0].Split(',');
            var allSceneObjects = CommandUtility.GetAllValidObjectsFromScene();
            var outputData = new Dictionary<string, bool>();
            foreach (var obj in allSceneObjects) {
                foreach (var name in names) {
                    if (JsonOutputFormatUtility.GeneratePrometheanObjectName(obj) == name
                        || obj.name == name) {
                        var rb = obj.GetComponent<Rigidbody>();
                        if (rb != null) {
                            outputData.Add(JsonOutputFormatUtility.GeneratePrometheanObjectName(obj), true);
                        }
                        else {
                            outputData.Add(JsonOutputFormatUtility.GeneratePrometheanObjectName(obj), false);
                        }
                    }
                }
            }

            var output = JsonOutputFormatUtility.GenerateJsonString(outputData);
            callback.Invoke(CommandHandleProcessState.Success, output);
        }
    }
}