using System;
using System.Collections.Generic;
using System.Linq;
using PrometheanAI.Modules.Utils;
using UnityEditor;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Sets a mesh from a GameObject with specified path to a selected GameObjects 
    /// </summary>
    public class SetMeshOnSelection : ICommand
    {
        public string GetToken => "set_mesh_on_selection";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var assetPath = commandParametersString[0];
            var targetAsset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (targetAsset == null) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
            }

            var targetRender = targetAsset.GetComponent<MeshFilter>();
            if (targetRender == null) {
                targetRender = targetAsset.GetComponentInChildren<MeshFilter>();
            }

            var targetMesh = new Mesh();
            if (targetRender != null) {
                targetMesh = targetRender.sharedMesh;
            }
            else {
                var skinnedMesh = targetAsset.GetComponent<SkinnedMeshRenderer>();
                if (skinnedMesh == null) {
                    skinnedMesh = targetAsset.GetComponentInChildren<SkinnedMeshRenderer>();
                    targetMesh = skinnedMesh.sharedMesh;
                }
            }

            var selectedObjects = Selection.gameObjects.ToList();
            selectedObjects = CommandUtility.ValidateObjects(selectedObjects);
            foreach (var obj in selectedObjects) {
                var filter = obj.GetComponent<MeshFilter>();
                UndoUtility.RecordUndo(GetToken, filter, false);
                filter.sharedMesh = targetMesh;
            }

            EditorUtils.RefreshEditorWindows();
            callback(CommandHandleProcessState.Success, string.Empty);
        }
    }
}