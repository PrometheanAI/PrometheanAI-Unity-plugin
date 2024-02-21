using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Transform = UnityEngine.Transform;

namespace PrometheanAI.Modules.Utils
{
    /// <summary>
    /// Class presents a set of methods used to simplify the workflow with Unity's data and PrometheanAI commands
    /// </summary>
    public static class CommandUtility
    {
        /// <summary>
        /// Used to parse commands parameters into string
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="commandToken"></param>
        /// <param name="commandParametersString"></param>
        public static void ParseCommandParameters(string commandData, out string commandToken,
            out string commandParametersString) {
            var selectionIndex = 0;
            if (!commandData.Contains(" ") && !commandData.Contains("\n")) {
                commandToken = commandData;
                commandParametersString = "";
                return;
            }

            for (var i = 0; i < commandData.Length; i++) {
                if (commandData[i].ToString().Equals(" ") || commandData[i].ToString().Equals("\n")) {
                    selectionIndex = i;
                    break;
                }
            }

            commandParametersString = commandData.Remove(0, selectionIndex + 1);
            commandParametersString = commandParametersString.Replace("\n", " ");
            commandToken = commandData.Remove(selectionIndex, commandData.Length - (selectionIndex));
        }

        /// <summary>
        /// Used to get GameObjects with overlapping
        /// </summary>
        /// <param name="selectedObjects"></param>
        /// <param name="allSceneObjects"></param>
        /// <returns>list of GameObjects which overlaps
        /// one of the GameObjects from selectedObjects param</returns>
        public static List<GameObject> GetOverlappedObjects(IEnumerable<GameObject> selectedObjects, IEnumerable<GameObject> allSceneObjects)
        {
            List<GameObject> overlapped = new List<GameObject>();
            List<Collider> selectedColliders = selectedObjects.Select(obj => obj.GetComponent<Collider>()).Where(coll => coll != null).ToList();

            foreach (var sceneObj in allSceneObjects)
            {
                var sceneCollider = sceneObj.GetComponent<Collider>();

                if (sceneCollider != null)
                {
                    foreach (var selectedCollider in selectedColliders)
                    {
                        if (selectedCollider.bounds.Intersects(sceneCollider.bounds) && !overlapped.Contains(sceneCollider.gameObject))
                        {
                            overlapped.Add(sceneCollider.gameObject);
                        }
                    }
                }
            }

            return overlapped;
        }

        /// <summary>
        /// Used to get GameObjects which are currently selected 
        /// </summary>
        /// <returns>list of selected GameObjects</returns>
        public static List<GameObject> GetSelectedObjects() {
            List<GameObject> objects = new List<GameObject>();
            foreach (var obj in Selection.gameObjects) {
                objects.Add(obj);
            }

            return objects;
        }

        /// <summary>
        /// Used to get GameObjects which are currently visible in Scene view
        /// </summary>
        /// <param name="sceneObj"></param>
        /// <param name="sceneCamera"></param>
        /// <param name="sceneRect"></param>
        /// <returns>list of visible GameObjects</returns>
        public static List<GameObject> GetVisibleObjects(List<GameObject> sceneObj, Camera sceneCamera, Rect sceneRect)
        {
            return sceneObj.Where(obj => sceneRect.Contains(sceneCamera.WorldToScreenPoint(obj.transform.position))).ToList();
        }

        /// <summary>
        /// Used to get names of GameObjects
        /// </summary>
        /// <param name="targetObjects"></param>
        /// <returns>objects names as List of strings</returns>
        public static List<string> GetNames(IEnumerable<GameObject> targetObjects) {
            List<string> names = new List<string>();
            foreach (var obj in targetObjects) {
                if (!obj.name.Contains($"#{obj.GetInstanceID()}")) {
                    var objName = JsonOutputFormatUtility.GeneratePrometheanObjectName(obj);
                    names.Add(objName);
                }
                else {
                    names.Add(obj.name);
                }
            }

            return names;
        }

        /// <summary>
        /// Used to get world positions of objects
        /// </summary>
        /// <param name="objects"></param>
        /// <returns> returns a Dictionary
        /// where key is objects index in passing List of objects
        /// and value is float[] of objects coordinates</returns>
        public static Dictionary<int, object[]> GetPositions(List<GameObject> objects) {
            var locations = new Dictionary<int, object[]>();
            for (var index = 0; index < objects.Count; index++) {
                var obj = objects[index];
                var positions = obj.transform.position;
                positions.x = Round(ConvertToCentimeters(positions.x), 4);
                positions.y = Round(ConvertToCentimeters(positions.y), 4);
                positions.z = Round(ConvertToCentimeters(positions.z), 4);
                var position = new[] {positions.x, positions.y, (object) positions.z};
                locations.Add(index + 1, position);
            }

            return locations;
        }

        /// <summary>
        /// Used to get a path to objects location in Assets folder
        /// </summary>
        /// <param name="objects"></param>
        /// <returns>Dictionary where key is a path to objects location
        /// and values is array of indexes of objects with such path </returns>
        public static Dictionary<string, int[]> GetPaths(List<GameObject> objects) {
            var paths = new Dictionary<string, int[]>();
            for (var index = 0; index < objects.Count; index++) {
                var obj = objects[index];
                var path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
                if (path.Equals(string.Empty)) {
                    path = "None";
                }

                var value = new[] {index};

                //adds new entry if theres no such path in dict
                if (!paths.Keys.Contains(path)) {
                    paths.Add(path, value);
                }

                //modifies existing entry with new values
                else {
                    var oldValues = paths[path];
                    var newValues = new int[paths[path].Length + 1];
                    for (var i = 0; i < paths[path].Length; i++) {
                        newValues[i] = oldValues[i];
                    }

                    newValues[paths[path].Length] = index;
                    paths[path] = newValues;
                }
            }

            if (paths.Count == 0 || paths.ElementAt(0).Key.Equals(string.Empty)) {
                paths.Clear();
            }

            return paths;
        }

        /// <summary>
        /// Used to get all objects from scene including child objects
        /// </summary>
        /// <param name="sceneObjects"></param>
        /// <returns>List of GameObjects which are instances of prefabs </returns>
        public static List<GameObject> AddChildObjects(IEnumerable<GameObject> sceneObjects) {
            var sortedObjects = new List<GameObject>();
            foreach (var obj in sceneObjects) {
                if (!sortedObjects.Contains(obj)) {
                    sortedObjects.Add(obj);
                }

                foreach (Transform child in obj.transform.GetComponentsInChildren<Transform>()) {
                    var childObj = child.gameObject;
                    if (!sortedObjects.Contains(childObj)) {
                        sortedObjects.Add(childObj);
                    }
                }
            }

            return sortedObjects;
        }

        /// <summary>
        /// Used to get a size of the object bounding box in scene
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>size of the object as Vector3 if it has MeshRenderer component
        /// or Vector3.zer if it doesnt</returns>
        public static Vector3 GetObjectsSize(GameObject obj) {
            var scale = obj.transform.lossyScale;
            var filter = obj.GetComponent<MeshFilter>();
            Vector3 bounds;
            if (filter != null) {
                bounds = filter.sharedMesh.bounds.size;
                return ConvertToCentimetersVector(Vector3.Scale(bounds, scale));
            }

            filter = obj.GetComponentInChildren<MeshFilter>();
            if (filter != null) {
                bounds = filter.sharedMesh.bounds.size;
                return ConvertToCentimetersVector(Vector3.Scale(bounds, scale));
            }

            var skinnedMesh = obj.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMesh == null) {
                skinnedMesh = obj.GetComponentInChildren<SkinnedMeshRenderer>();
            }

            if (skinnedMesh != null) {
                var meshBounds = skinnedMesh.sharedMesh.bounds.size;
                return ConvertToCentimetersVector(Vector3.Scale(meshBounds, scale));
            }

            return Vector3.zero;
        }

        /// <summary>
        /// Used to get offset of the object center point similar to Unreal plugin
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>offset of objects pivot as Vector3 if it has MeshFilter component
        /// or Vector3.zero if it doesnt</returns>
        public static Vector3 GetPivotOffset(GameObject obj) {
            var filter = obj.GetComponent<MeshFilter>();
            if (filter == null) {
                filter = obj.GetComponentInChildren<MeshFilter>();
            }

            if (filter != null) {
                var bounds = filter.sharedMesh.bounds;
                var offset = bounds.center;
                offset.y = bounds.center.y - bounds.extents.y;

                return ConvertToCentimetersVector(-offset);
            }

            var skinnedMesh = obj.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMesh == null) {
                skinnedMesh = obj.GetComponentInChildren<SkinnedMeshRenderer>();
            }

            if (skinnedMesh != null) {
                var bounds = skinnedMesh.sharedMesh.bounds;
                var offset = bounds.center;
                offset.y = bounds.center.y - bounds.extents.y;

                return ConvertToCentimetersVector(-offset);
            }

            return Vector3.zero;
        }

        /// <summary>
        /// Used to check if GameObject is supported by plugin
        /// </summary>
        /// <param name="objectsToValidate"></param>
        /// <returns>list of GameObjects with MeshRenderer component</returns>
        public static List<GameObject> ValidateObjects(IEnumerable<GameObject> objectsToValidate) {
            var validatedObjects = new List<GameObject>();
            foreach (var obj in objectsToValidate) {
                var renderer = obj.GetComponent<Renderer>();
                if (renderer == null) {
                    renderer = obj.GetComponentInChildren<Renderer>();
                }

                //PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj) != string.Empty
                if (renderer != null) {
                    validatedObjects.Add(obj);
                }
            }

            return validatedObjects;
        }

        /// <summary>
        /// Used to generate single string from an array of strings
        /// </summary>
        /// <param name="namesList"></param>
        /// <returns>generated string</returns>
        public static string StringArrayToString(IEnumerable<string> namesList) {
            var output = "";
            foreach (var name in namesList) {
                output += name + ",";
            }

            output = output.Remove(output.Length - 1, 1);
            return output;
        }

        /// <summary>
        /// converts string[] to vector4 if array is enough Lenght
        /// </summary>
        /// <param name="value"></param>
        /// <returns>created Vector4</returns>
        public static Vector4 StringArrayToVector4(string[] value) {
            if (value.Length < 4) {
                return Vector4.zero;
            }

            return new Vector4(
                float.Parse(value[0], CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(value[1], CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(value[2], CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(value[3], CultureInfo.InvariantCulture.NumberFormat));
        }

        /// <summary>
        /// Used to generate single string from an array of floats
        /// </summary>
        /// <param name="values"></param>
        /// <returns>generated string</returns>
        public static string FloatArrayToString(IEnumerable<float> values) {
            var output = "";
            foreach (var value in values) {
                output += Round(value, 4) + ",";
            }

            output = output.Remove(output.Length - 1, 1);
            return output;
        }

        /// <summary>
        /// converts array of Vector4 into string
        /// </summary>
        /// <param name="values"></param>
        /// <returns>generated string</returns>
        public static string Vector4ArrayToString(IEnumerable<Vector4> values) {
            var output = "";
            foreach (var value in values) {
                output += Round(value.x, 4).ToString().Replace(',', '.') + ",";
                output += Round(value.y, 4).ToString().Replace(',', '.') + ",";
                output += Round(value.z, 4).ToString().Replace(',', '.') + ",";
                output += Round(value.w, 4).ToString().Replace(',', '.') + " ";
            }

            output = output.Remove(output.Length - 1, 1);
            return output;
        }

        /// <summary>
        /// Used to round a float
        /// </summary>
        /// <param name="value"></param>
        /// <param name="digits"></param>
        /// <returns>returns float rounded to set amount of digits</returns>
        public static float Round(float value, int digits) {
            // return (float) Math.Round(value, digits);
            return value;
        }

        /// <summary>
        /// Used to sort out GameObjects with specific path to Asset from a list
        /// </summary>
        /// <param name="path"></param>
        /// <param name="targetObjects"></param>
        /// <returns>list of GameObjects with targeted Asset path</returns>
        public static List<GameObject> SortGameObjectsByPath(string path, IEnumerable<GameObject> targetObjects) {
            var sortedList = new List<GameObject>();
            foreach (var obj in targetObjects) {
                if (PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj) == path) {
                    sortedList.Add(obj);
                }
            }

            return sortedList;
        }

        /// <summary>
        /// Used to sort object by their material assuming they were already sorted for Promethean
        /// </summary>
        /// <param name="targetObjects"></param>
        /// <param name="materialPath"></param>
        /// <returns>list of GameObjects with specific material</returns>
        public static List<GameObject> SortObjectByMaterial(IEnumerable<GameObject> targetObjects, string materialPath) {
            var mat = AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material));
            var materialName = mat.name;
            var sortedObjects = new List<GameObject>();
            foreach (var obj in targetObjects) {
                var checkName = obj.GetComponent<Renderer>().sharedMaterial.name;
                checkName = checkName.Replace(" (Instance)", "");
                if (checkName == materialName) {
                    sortedObjects.Add(obj);
                }
            }

            return sortedObjects;
        }

        /// <summary>
        /// Used to convert string[] into Vector3
        /// </summary>
        /// <param name="data"></param>
        /// <returns>generated Vector3</returns>
        public static Vector3 StringArrayToVector(string[] data) {
            var locationVector = new Vector3(
                float.Parse(data[0], CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(data[1], CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(data[2], CultureInfo.InvariantCulture.NumberFormat));
            if (locationVector == Vector3.negativeInfinity) {
                locationVector = Vector3.zero;
            }

            return locationVector;
        }

        /// <summary>
        /// used to convert float values from meters to centimeters
        /// </summary>
        /// <param name="meterValue"></param>
        /// <returns>float value in centimeters</returns>
        public static float ConvertToCentimeters(float meterValue) {
            return meterValue * 100f;
        }

        /// <summary>
        /// used to convert float values from centimeters to meters
        /// </summary>
        /// <param name="centimetersValue"></param>
        /// <returns>float value in meters</returns>
        public static float ConvertToMeters(float centimetersValue) {
            return centimetersValue / 100f;
        }

        /// <summary>
        /// used to convert Vector3 from meters to centimeters
        /// </summary>
        /// <param name="metersVector"></param>
        /// <returns>Vector3 in centimeters</returns>
        public static Vector3 ConvertToCentimetersVector(Vector3 metersVector) {
            return new Vector3(ConvertToCentimeters(metersVector.x),
                ConvertToCentimeters(metersVector.y),
                ConvertToCentimeters(metersVector.z));
        }

        /// <summary>
        /// used to convert Vector3 from centimeters to meters
        /// </summary>
        /// <param name="centimetersVector"></param>
        /// <returns>Vector3 in meters</returns>
        public static Vector3 ConvertToMetersVector(Vector3 centimetersVector) {
            return new Vector3(ConvertToMeters(centimetersVector.x),
                ConvertToMeters(centimetersVector.y),
                ConvertToMeters(centimetersVector.z));
        }

        /// <summary>
        /// used to get hold of all GameObjects valid for PrometheanAI from the current scene
        /// </summary>
        /// <returns>List of valid GameObjects</returns>
        public static List<GameObject> GetAllValidObjectsFromScene() {
            var currentScene = SceneManager.GetActiveScene();
            var allSceneObjects = currentScene.GetRootGameObjects().ToList();
            allSceneObjects = CommandUtility.AddChildObjects(allSceneObjects);
            allSceneObjects = CommandUtility.ValidateObjects(allSceneObjects);
            return allSceneObjects;
        }
    }
}