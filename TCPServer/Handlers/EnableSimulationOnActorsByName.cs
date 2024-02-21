using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    //Called by command "simulate"
    //Raw incoming command example : enable_simulation_on_actors_by_name cube#-3944

    /// <summary>
    /// Used to add Rigidbodies to specified GameObjects 
    /// </summary>
    public class EnableSimulationOnActorsByName : ICommand
    {
        public string GetToken => "enable_simulation_on_actors_by_name";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var names = commandParametersString[0].Split(',');
            var allSceneObjects = CommandUtility.GetAllValidObjectsFromScene();
            foreach (var obj in allSceneObjects) {
                foreach (var name in names) {
                    if (JsonOutputFormatUtility.GeneratePrometheanObjectName(obj) == name
                        || obj.name == name) {
                        if (obj.name.ToLower().Contains("floor") || obj.name.ToLower().Contains("ground")
                            || obj.name.ToLower().Contains("terrain")) {
                            break;
                        }

                        obj.AddComponent<Rigidbody>();
                    }
                }
            }

            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}