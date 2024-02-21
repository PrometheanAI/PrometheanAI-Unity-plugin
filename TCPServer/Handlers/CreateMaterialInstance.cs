using System;
using System.Collections.Generic;
using System.Globalization;
using PrometheanAI.Modules.Utils;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Used to create new material from given data
    /// </summary>
    public class CreateMaterialInstance : ICommand
    {
        static readonly int s_MainTex = Shader.PropertyToID("_MainTex");
        static readonly int s_EmissionColor = Shader.PropertyToID("_EmissionColor");
        public string GetToken => "create_material_instance";

        public void Handle(string rawCommandData, List<string> commandParametersString,
            Action<CommandHandleProcessState, string> callback) {
            var obj =
                JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(
                    commandParametersString[0]);
            if (obj != null) {
                foreach (var pair in obj) {
                    var parentPath = pair.Key;
                    var objectValues = pair.Value;
                    var targetPath = (string) objectValues["target"];
                    var info = Convert.ToString(objectValues["attributes"]);
                    var materialParameters = JsonConvert.DeserializeObject<Dictionary<string, object>>(info);
                    if (parentPath == string.Empty || targetPath == string.Empty) {
                        return;
                    }

                    var parentMaterial = AssetDatabase.LoadAssetAtPath<Material>(parentPath);
                    if (parentMaterial != null) {
                        var instanceMaterial = new Material(parentMaterial);
                        var texturePath = (string) materialParameters["Texture"];
                        var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
                        if (texture != null) {
                            instanceMaterial.SetTexture(s_MainTex, texture);
                        }

                        var color = Convert.ToString(materialParameters["Color"]).Replace("[", "").Replace("]", "")
                            .Replace("\n", "");
                        var colorValues = color.Split(',');
                        var colorVector = CommandUtility.StringArrayToVector(colorValues);
                        instanceMaterial.color = new Color(colorVector.x, colorVector.y, colorVector.z);
                        var emissionString = (string) materialParameters["Emissive"];
                        var emission = float.Parse(emissionString, CultureInfo.InvariantCulture.NumberFormat);

                        //TODO:emission checkbox doesnt toggle on
                        instanceMaterial.EnableKeyword("_EMISSION");
                        instanceMaterial.SetColor(s_EmissionColor,
                            new Color(emission, emission, emission, emission) / 4.7f);
                        instanceMaterial.name = parentMaterial.name + "(instance)";
                        targetPath += instanceMaterial.name + ".mat";
                        AssetDatabase.CreateAsset(instanceMaterial, targetPath);
                        var material = AssetDatabase.LoadAssetAtPath<Material>(targetPath);
                        UndoUtility.RecordUndo(GetToken, material, true);
                        Selection.activeObject = instanceMaterial;
                        EditorUtils.RefreshEditorWindows();
                    }
                }
            }

            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }
    }
}