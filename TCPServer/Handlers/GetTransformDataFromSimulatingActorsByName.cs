using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    //Called by command "simulate" and then pressing "Accept simulation results"
    //Raw incoming command example : get_transform_data_from_simulating_actors_by_name cube#-3944

    /// <summary>
    /// Used to gather Transform data for specified objects in the end of Physics simulation
    /// </summary>
    public class GetTransformDataFromSimulatingActorsByName : ICommand
    {
        public string GetToken => "get_transform_data_from_simulating_actors_by_name";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var names = commandParametersString[0].Split(',');
            var outputData = new Dictionary<string, float[]>();
            var allSceneObjects = CommandUtility.GetAllValidObjectsFromScene();
            SimulatedObjectsData.SimulatedData.Clear();
            foreach (var obj in allSceneObjects) {
                foreach (var name in names) {
                    if (JsonOutputFormatUtility.GeneratePrometheanObjectName(obj) == name
                        || obj.name == name) {
                        var vectorArray = new[] {obj.transform.position, obj.transform.eulerAngles};
                        SimulatedObjectsData.SimulatedData.Add(obj.name, vectorArray);
                        var position = vectorArray[0];
                        position = CommandUtility.ConvertToCentimetersVector(position);
                        var rotation = vectorArray[1];
                        var scale = obj.transform.lossyScale;
                        var transformData = new[] {
                            CommandUtility.Round(position.x, 4),
                            CommandUtility.Round(position.y, 4),
                            CommandUtility.Round(position.z, 4),
                            CommandUtility.Round(rotation.x, 4),
                            CommandUtility.Round(rotation.y, 4),
                            CommandUtility.Round(rotation.z, 4),
                            CommandUtility.Round(scale.x, 4),
                            CommandUtility.Round(scale.y, 4),
                            CommandUtility.Round(scale.z, 4)
                        };
                        outputData.Add(name, transformData);
                        break;
                    }
                }
            }

            var output = JsonOutputFormatUtility.GenerateJsonString(outputData);
            callback.Invoke(CommandHandleProcessState.Success, output);
        }
    }
}