using System;
using System.Collections.Generic;
using System.Linq;
using PrometheanAI.Modules.Utils;
using UnityEditor;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Sets a mesh from a GameObject with specified path to a GameObjects with specified names
    /// </summary>
    public class SetMesh : ICommand
    {
        public string GetToken => "set_mesh";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 2) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var assetPath = commandParametersString[0];
            var names = commandParametersString[1].Split(',').ToList();
            GameObject targetAsset;
            targetAsset = assetPath == "None" ? GameObject.CreatePrimitive(PrimitiveType.Cube) : AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            if (targetAsset == null) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var targetRender = targetAsset.GetComponent<MeshFilter>();
            if (targetRender == null) {
                targetRender = targetAsset.GetComponentInChildren<MeshFilter>();
            }

            var targetMesh = targetRender.sharedMesh;
            var allObjects = CommandUtility.GetAllValidObjectsFromScene();
            foreach (var obj in allObjects) {
                foreach (var name in names) {
                    if (JsonOutputFormatUtility.GeneratePrometheanObjectName(obj) == name) {
                        if (obj.GetComponent<MeshFilter>() != null) {
                            var filter = obj.GetComponent<MeshFilter>();
                            UndoUtility.RecordUndo(GetToken, filter, false);
                            filter.sharedMesh = targetMesh;
                            break;
                        }

                        obj.AddComponent<MeshFilter>().sharedMesh = targetMesh;
                        break;
                    }
                }
            }

            EditorUtils.RefreshEditorWindows();
            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}