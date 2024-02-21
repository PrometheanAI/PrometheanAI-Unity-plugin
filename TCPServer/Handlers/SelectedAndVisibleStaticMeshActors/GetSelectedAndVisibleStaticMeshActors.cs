using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEditor;
using UnityEngine;
using System.Diagnostics;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    //called by hovering over Promethean bar
    //Raw incoming command example : get_selected_and_visible_static_mesh_actors
    /// <summary>
    /// Send to Promethean data about currently visible and selected GameObjects in SceneView
    /// </summary>
    public class GetSelectedAndVisibleStaticMeshActors : ICommand
    {
        public string GetToken => "get_selected_and_visible_static_mesh_actors";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            Stopwatch stopwatch = new Stopwatch();
            void ProfilerResults(string s)
            {
             stopwatch.Stop();
             UnityEngine.Debug.Log($"GetSelectedAndVisibleStaticMeshActors.{s} took " + stopwatch.ElapsedMilliseconds + " ms");
             stopwatch.Reset();
             stopwatch.Start();
            }


            
            var sceneView = SceneView.lastActiveSceneView;
            var allSceneObjects = CommandUtility.GetAllValidObjectsFromScene();
            var sceneViewCamera = sceneView.camera;
            var rect = sceneViewCamera.pixelRect;
            var visibleObjects = CommandUtility.GetVisibleObjects(allSceneObjects, sceneViewCamera, rect);
            var selectedObjects = CommandUtility.GetSelectedObjects();
            selectedObjects = CommandUtility.ValidateObjects(selectedObjects);
            var childObjects = new List<GameObject>();
            foreach (var obj in selectedObjects) {
                if (obj.transform.childCount > 0) {
                    foreach (Transform child in obj.transform) {
                        childObjects.Add(child.gameObject);
                    }
                }
            }
            ProfilerResults("GetObjects ");
            foreach (var obj in childObjects) {
                if (!visibleObjects.Contains(obj)) {
                    visibleObjects.Add(obj);
                }
            }

            var overlappedObj = CommandUtility.GetOverlappedObjects(selectedObjects, allSceneObjects);
            foreach (var obj in overlappedObj) {
                if (!visibleObjects.Contains(obj)) {
                    visibleObjects.Add(obj);
                }
            }
            ProfilerResults("GetOverlappedObjects ");
            var selectedLocations = CommandUtility.GetPositions(selectedObjects);
            var visibleLocations = CommandUtility.GetPositions(visibleObjects);
            var selectedNames = CommandUtility.GetNames(selectedObjects);
            var visibleNames = CommandUtility.GetNames(visibleObjects);
            var selectedPaths = CommandUtility.GetPaths(selectedObjects);
            var visiblePaths = CommandUtility.GetPaths(visibleObjects);
            var sceneCameraTransform = sceneViewCamera.transform;
            var cameraPos = sceneCameraTransform.position;
            var cameraDir = sceneCameraTransform.forward;
            ProfilerResults("Get* ");
            var data = new InitialSceneData(selectedLocations, visibleLocations, selectedNames, visibleNames,
                selectedPaths, visiblePaths, cameraPos, cameraDir);
            var newOutput = JsonOutputFormatUtility.GenerateJsonString(data);
            ProfilerResults("GenerateJsonString ");


            callback.Invoke(CommandHandleProcessState.Success, newOutput);
            ProfilerResults("Invoke ");
        }
    }
}