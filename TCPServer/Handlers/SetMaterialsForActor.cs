using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Sets materials to a specified GameObject in Scene
    /// </summary>
    public class SetMaterialsForActor : ICommand
    {
        public string GetToken => "set_materials_for_actor";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var objectName = commandParametersString[0];
            var data = rawCommandData.Replace(GetToken, "");
            data = data.Remove(0, 1);
            var json = JObject.Parse(data);
            var allSceneObjects = CommandUtility.GetAllValidObjectsFromScene();
            Renderer targetRender = null;
            foreach (var obj in allSceneObjects) {
                if (obj.name == objectName
                    || JsonOutputFormatUtility.GeneratePrometheanObjectName(obj) == objectName) {
                    targetRender = obj.GetComponent<Renderer>();
                    break;
                }
            }

            if (targetRender != null) {
                var objectsMaterials = targetRender.sharedMaterials;
                foreach (var pair in json) {
                    var index = Int32.Parse(pair.Key);
                    var path = pair.Value.ToString();
                    var material = AssetDatabase.LoadAssetAtPath<Material>(path);
                    if (objectsMaterials.Length > index && material != null) {
                        objectsMaterials[index] = material;
                    }
                }

                UndoUtility.RecordUndo(GetToken, targetRender, false);
                targetRender.sharedMaterials = objectsMaterials;
            }

            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}