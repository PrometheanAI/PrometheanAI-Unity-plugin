using System;
using System.Collections.Generic;
using System.Linq;
using PrometheanAI.Modules.Utils;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    //

    /// <summary>
    /// Adds instance of a prefab as a child to each of selected objects in Scene 
    /// </summary>
    public class AddMeshOnSelection : ICommand
    {
        public string GetToken => "add_mesh_on_selection";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var paths = commandParametersString[0].Split(',').ToList();
            var selectedObjects = Selection.gameObjects.ToList();
            var newObjectsToSelect = new List<Object>();
            selectedObjects = CommandUtility.ValidateObjects(selectedObjects);
            if (selectedObjects.Count > 0) {
                foreach (var obj in selectedObjects) {
                    foreach (var path in paths) {
                        var targetAsset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                        var prefab = (GameObject) PrefabUtility.InstantiatePrefab(targetAsset);
                        newObjectsToSelect.Add(prefab);
                        UndoUtility.RecordUndo(GetToken, prefab, true);
                        prefab.transform.position = obj.transform.position;
                        try {
                            prefab.transform.parent = obj.transform;
                        }
                        catch (Exception e) {
                            Debug.Log(e);
                        }
                    }
                }

                Selection.objects = newObjectsToSelect.ToArray();
            }
            else {
                foreach (var path in paths) {
                    var targetAsset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    var prefab = (GameObject) PrefabUtility.InstantiatePrefab(targetAsset);
                    newObjectsToSelect.Add(prefab);

                    UndoUtility.RecordUndo(GetToken, prefab, true);
                    prefab.transform.position = Vector3.zero;
                }

                Selection.objects = newObjectsToSelect.ToArray();
            }

            UndoUtility.CollapseUndo();
            EditorUtils.RefreshEditorWindows();
            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}