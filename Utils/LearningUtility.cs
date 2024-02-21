using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PrometheanAI.Modules.Utils
{
    /// <summary>
    /// Provides a set of methods to work with PrometheanAI *Learn* functionality
    /// </summary>
    public static class LearningUtility
    {
        /// <summary>
        /// used to generate learn data for PrometheanAI if the target of the *learn* command is Selection
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="path"></param>
        public static void LearnSelection(string sceneName, string path) {
            var selectedObjects = Selection.gameObjects.ToList();
            selectedObjects = CommandUtility.ValidateObjects(selectedObjects);
            GenerateLearnData(selectedObjects, sceneName, path);
        }

        /// <summary>
        /// used to generate learn data for PrometheanAI if the target of the *learn* command is SceneView
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="path"></param>
        public static void LearnView(string sceneName, string path) {
            var sceneView = SceneView.lastActiveSceneView;
            var currentScene = SceneManager.GetActiveScene();
            var allSceneObjects = currentScene.GetRootGameObjects().ToList();
            allSceneObjects = CommandUtility.AddChildObjects(allSceneObjects);
            allSceneObjects = CommandUtility.ValidateObjects(allSceneObjects);
            var sceneViewCamera = sceneView.camera;
            var rect = sceneViewCamera.pixelRect;
            var visibleObjects = CommandUtility.GetVisibleObjects(allSceneObjects, sceneViewCamera, rect);
            GenerateLearnData(visibleObjects, sceneName, path);
        }

        /// <summary>
        ///  used to generate learn data for PrometheanAI if the target of the *learn* command is not specified
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="path"></param>
        public static void Learn(string sceneName, string path) {
            var currentScene = SceneManager.GetActiveScene();
            var allSceneObjects = currentScene.GetRootGameObjects().ToList();
            allSceneObjects = CommandUtility.AddChildObjects(allSceneObjects);
            allSceneObjects = CommandUtility.ValidateObjects(allSceneObjects);
            GenerateLearnData(allSceneObjects, sceneName, path);
        }

        /// <summary>
        /// used to generate needed Data about Object for PromethenAI
        /// </summary>
        /// <param name="targets"></param>
        /// <param name="sceneName"></param>
        /// <returns> generated data as Json string</returns>
        static string GenerateLearnFileString(IEnumerable<GameObject> targets, string sceneName) {
            var outputData = new Dictionary<string, object>();
            var objectsData = new List<object>();
            foreach (var obj in targets) {
                var objData = new Dictionary<string, object>();
                var objPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
                if (objPath == "") {
                    objData.Add("is_group", true);
                }
                else {
                    objData.Add("art_asset_path", objPath);
                }

                objData.Add("raw_name", JsonOutputFormatUtility.GeneratePrometheanObjectName(obj));
                var parent = obj.transform.parent;
                objData.Add("parent_name", parent != null
                    ? JsonOutputFormatUtility.GeneratePrometheanObjectName(parent.gameObject)
                    : "No_Parent");
                var rotation = obj.transform.eulerAngles;
                var rotationValues = new[] {
                    CommandUtility.Round(rotation.x, 4),
                    CommandUtility.Round(rotation.y, 4),
                    CommandUtility.Round(rotation.z, 4)
                };
                objData.Add("rotation", rotationValues);
                var scale = obj.transform.lossyScale;
                var scaleValues = new[] {
                    CommandUtility.Round(scale.x, 4),
                    CommandUtility.Round(scale.y, 4),
                    CommandUtility.Round(scale.z, 4)
                };
                objData.Add("scale", scaleValues);
                var pivot = CommandUtility.GetPivotOffset(obj);
                var worldSpacePivot = obj.transform.TransformPoint(pivot);
                var pivotData = new[] {
                    CommandUtility.Round(worldSpacePivot.x, 4),
                    CommandUtility.Round(worldSpacePivot.y, 4),
                    CommandUtility.Round(worldSpacePivot.z, 4)
                };
                objData.Add("pivot", pivotData);
                var size = CommandUtility.GetObjectsSize(obj);
                var sizeValues = new[] {
                    CommandUtility.Round(size.x, 4),
                    CommandUtility.Round(size.y, 4),
                    CommandUtility.Round(size.z, 4)
                };
                objData.Add("size", sizeValues);
                var objPosition = obj.transform.position;
                objPosition = CommandUtility.ConvertToCentimetersVector(objPosition);
                var positionValues = new[] {
                    CommandUtility.Round(objPosition.x, 4),
                    CommandUtility.Round(objPosition.y, 4),
                    CommandUtility.Round(objPosition.z, 4)
                };
                var transformDataArray = new[] {
                    positionValues[0], positionValues[1], positionValues[2],
                    rotationValues[0], rotationValues[1], rotationValues[2],
                    scaleValues[0], scaleValues[1], scaleValues[2]
                };

                objData.Add("transform", transformDataArray);

                objectsData.Add(objData);
            }

            var arrayObjectData = objectsData.ToArray();
            outputData.Add("scene_id", sceneName);
            outputData.Add("raw_data", arrayObjectData);

            return JsonOutputFormatUtility.GenerateJsonString(outputData);
        }

        /// <summary>
        /// used to save generated Learn data for PrometheanAI to pick up
        /// </summary>
        /// <param name="targets"></param>
        /// <param name="sceneName"></param>
        /// <param name="path"></param>
        static void GenerateLearnData(List<GameObject> targets, string sceneName, string path) {
            var outputData = GenerateLearnFileString(targets, sceneName);
            File.Delete(path);
            var writer = new StreamWriter(path, true);
            writer.WriteLine(outputData);
            writer.Close();
        }
    }
}