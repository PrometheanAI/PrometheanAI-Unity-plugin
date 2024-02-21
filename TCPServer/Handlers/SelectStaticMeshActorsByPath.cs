using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PrometheanAI.Modules.TCPServer
{
    /// <summary>
    /// Used to select GameObjects in a Scene with a given  path to root prefab
    /// </summary>
    public class SelectStaticMeshActorsByPath : ICommand
    {
        public string GetToken => "select_static_mesh_actors_by_path";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var path = commandParametersString[0];
            var allSceneObjects = CommandUtility.GetAllValidObjectsFromScene();
            allSceneObjects = CommandUtility.SortGameObjectsByPath(path, allSceneObjects);
            var objectsList = new List<Object>();
            foreach (var obj in allSceneObjects) {
                objectsList.Add(obj);
            }

            if (allSceneObjects.Count > 0) {
                UndoUtility.RecordUndo(GetToken, Selection.objects, false);
                Selection.objects = objectsList.ToArray();
            }
            else {
                Selection.objects = null;
                Debug.Log("Theres no GameObjects with such path in scene");
            }

            EditorUtils.RefreshEditorWindows();
            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}