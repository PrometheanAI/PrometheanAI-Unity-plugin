using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    //Raw incoming command example : disable_simulation_on_actors_by_name cube#-3944

    /// <summary>
    /// Used to remove Rigidbodies form specified GameObjects
    /// </summary>
    public class DisableSimulationOnActorsByName : ICommand
    {
        public string GetToken => "disable_simulation_on_actors_by_name";

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
                        if (obj.GetComponent<Rigidbody>() != null) {
                            Object.DestroyImmediate(obj.GetComponent<Rigidbody>(), true);
                        }
                    }
                }
            }

            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}