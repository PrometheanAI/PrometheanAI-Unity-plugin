using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Object = UnityEngine.Object;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Loads asset at path and starts Unities DragAndDrop operation from a custom EditorWindow
    /// </summary>
    public class LoadAssets : ICommand
    {
        public string GetToken => "load_assets";
        string m_CurrentPath;
        List<GameObject> m_SceneObjects = new List<GameObject>();
        List<GameObject> m_ObjectsWithoutColliders = new List<GameObject>();
        GameObject m_TargetObject = null;

        public void Handle(string rawCommandData, List<string> commandParametersString, Action<CommandHandleProcessState, string> callback) {
            if (commandParametersString.Count < 1) {
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var paths = commandParametersString[0].Split(',');
            foreach (var path in paths) {
                m_CurrentPath = path;
                if (m_CurrentPath.StartsWith("StaticMesh")) {
                    m_CurrentPath = path.Replace("StaticMesh", "").Replace("\'", "");
                }

                if (m_CurrentPath.StartsWith("Material")) {
                    m_CurrentPath = path.Replace("Material", "");
                }

                if (m_CurrentPath.StartsWith("MaterialInstanceConstant")) {
                    m_CurrentPath = path.Replace("MaterialInstanceConstant", "").Replace("\'", "");
                }

                if (m_CurrentPath.StartsWith("Texture2D")) {
                    m_CurrentPath = path.Replace("Texture2D", "").Replace("\'", "");
                }

                SceneView.duringSceneGui += ManualDragOperation;
            }

            callback.Invoke(CommandHandleProcessState.Success, string.Empty);
        }

        /// <summary>
        /// Used to manually imitate drag and drop operation.If raycast from mouse position hits any object -  created
        /// object is placed at hit point and rotated according to hit normal.Otherwise created object will be placed in front of
        /// SceneView camera 
        /// </summary>
        /// <param name="view"></param>
        void ManualDragOperation(SceneView view) {
            var current = Event.current;
            switch (current.type) {
                case EventType.DragUpdated:
                    PerformObjectDrag(view);
                    break;
                case EventType.DragPerform:
                    PerformObjectDrop(view);
                    break;
                case EventType.DragExited:
                    Object.DestroyImmediate(m_TargetObject);
                    break;
              case EventType.MouseDown:
                // reset the DragAndDrop Data
                DragAndDrop.PrepareStartDrag ();
                Debug.Log ("MouseDown");
                break;
              case EventType.MouseUp:

                     // Clean up, in case MouseDrag never occurred:
                    DragAndDrop.PrepareStartDrag ();
                break;
                //Current event will be called only if mouse is inside the SceneView and there is no Drag being performed
             case EventType.MouseMove:
                    m_TargetObject = null;
                    foreach (var sceneObj in m_ObjectsWithoutColliders) {
                        Object.DestroyImmediate(sceneObj.GetComponent<MeshCollider>());
                    }

                    m_ObjectsWithoutColliders.Clear();
                    SceneView.duringSceneGui -= ManualDragOperation;
                    break;
            }
        }

        /// <summary>
        /// Creates target object and positions it same as if it was dragged by mouse
        /// </summary>
        /// <param name="view"></param>
        void PerformObjectDrag(SceneView view) {
            if (m_ObjectsWithoutColliders.Count == 0) {
                m_SceneObjects = CommandUtility.GetAllValidObjectsFromScene();
                m_ObjectsWithoutColliders = new List<GameObject>();
                foreach (var sceneObj in m_SceneObjects) {
                    if (!sceneObj.TryGetComponent(typeof(MeshCollider), out var collider)) {
                        sceneObj.AddComponent<MeshCollider>();
                        m_ObjectsWithoutColliders.Add(sceneObj);
                    }
                }
            }

            if (m_TargetObject == null) {
                var newObject = AssetDatabase.LoadAssetAtPath<GameObject>(m_CurrentPath);
                m_TargetObject = (GameObject) PrefabUtility.InstantiatePrefab(newObject);
                m_TargetObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            }

            var draggingMousePosition = Event.current.mousePosition;
            var temRay = HandleUtility.GUIPointToWorldRay(draggingMousePosition);
            m_TargetObject.transform.position = Physics.Raycast(temRay, out var tempHit)
                ? tempHit.point
                : temRay.origin + temRay.direction * 10;
        }

        /// <summary>
        /// Positions target object depending on raycast result and records Undo for Drag & Drop operation
        /// </summary>
        /// <param name="view"></param>
        void PerformObjectDrop(SceneView view) {
            DragAndDrop.AcceptDrag();
            EndDragOperation();
            SceneView.duringSceneGui -= ManualDragOperation;
            if (DragAndDrop.paths.Length == 0) {
                m_TargetObject = null;
                return;
            }

            if (m_TargetObject == null) {
                EditorUtils.RefreshWindowsAfterDrag();
                return;
            }

            EditorSceneManager.MarkSceneDirty(m_TargetObject.scene);
            UndoUtility.RecordUndo(GetToken, m_TargetObject, true);
            var mousePosition = Event.current.mousePosition;

            var ray = HandleUtility.GUIPointToWorldRay(mousePosition);
            if (Physics.Raycast(ray, out var hit)) {
                foreach (var sceneObj in m_ObjectsWithoutColliders) {
                    Object.DestroyImmediate(sceneObj.GetComponent<MeshCollider>());
                }

                m_ObjectsWithoutColliders.Clear();
                m_TargetObject.transform.position = hit.point;
                m_TargetObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                m_TargetObject.layer = LayerMask.NameToLayer("Default");
                Selection.objects = new Object[] {m_TargetObject};
                EditorUtils.RefreshWindowsAfterDrag();
                EditorUtils.RefreshEditorWindows();
                m_TargetObject = null;
                return;
            }

            EditorUtils.RefreshWindowsAfterDrag();
            var mouseWorldPos = ray.origin + ray.direction * 10;
            m_TargetObject.transform.position = mouseWorldPos;
            foreach (var sceneObj in m_ObjectsWithoutColliders) {
                Object.DestroyImmediate(sceneObj.GetComponent<MeshCollider>());
            }

            m_ObjectsWithoutColliders.Clear();
            Selection.objects = new Object[] {m_TargetObject};
            m_TargetObject.layer = LayerMask.NameToLayer("Default");
            m_TargetObject = null;
        }

        /// <summary>
        /// resets Unities' Drag & Drop operation
        /// </summary>

        //NOTE: currently cant reset  DragAndDrop.paths
        void EndDragOperation() {
            DragAndDrop.paths = null;
            DragAndDrop.objectReferences = null;
        }
    }
}