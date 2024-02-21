using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    //called by pressing either "Cancel" or "Accept simulation results" on Simulation bar in Promethean
    //Raw incoming command example : end_simulation

    /// <summary>
    /// Used to exit the Play Mode
    /// </summary>
    public class EndSimulation : ICommand
    {
        public string GetToken => "end_simulation";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            EditorApplication.ExitPlaymode();
            EditorApplication.playModeStateChanged += ApplySimulationResults;
            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }

        /// <summary>
        /// As we cant quit make changes from physical simulation consistent between Play and Edit mode,we apply them after
        /// Application has already exited Play Mode
        /// </summary>
        /// <param name="state"></param>
        void ApplySimulationResults(PlayModeStateChange state) {
            if (state == PlayModeStateChange.EnteredEditMode && SimulatedObjectsData.SimulatedData.Count > 0) {
                var allSceneObjects = CommandUtility.GetAllValidObjectsFromScene();
                foreach (var obj in allSceneObjects) {
                    foreach (var pair in SimulatedObjectsData.SimulatedData) {
                        if (pair.Key == obj.name) {
                            Object.DestroyImmediate(obj.GetComponent<Rigidbody>());
                            UndoUtility.RecordUndo(GetToken, obj.transform, false);
                            obj.transform.position = pair.Value[0];
                            obj.transform.eulerAngles = pair.Value[1];
                        }
                    }
                }

                SimulatedObjectsData.SimulatedData.Clear();
            }
        }
    }
}