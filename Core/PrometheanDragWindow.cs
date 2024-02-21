using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PrometheanAI.Modules.Editor
{
    public class PrometheanDragWindow : EditorWindow
    {
        bool m_PosSet;
        string m_ObjectPath = "";
        GameObject m_Prefab;

        /// <summary>
        /// Creates custom EditorWindow for DragAndDrop operations and opens SceneView windows if its closed or not visible
        /// </summary>
        /// <param name="path"></param>
        public void ShowWindow(string path) {
            EditorApplication.ExecuteMenuItem("Window/Windows/Scene");
            m_ObjectPath = path;
            Init();
            GetWindow<PrometheanDragWindow>("DragAndDrop");
            maxSize = new Vector2(100, 100);
            minSize = maxSize;
            var sceneRect = SceneView.lastActiveSceneView.position;
            var middlePoint = SceneView.lastActiveSceneView.position.center;
            var posX = middlePoint.x + sceneRect.width / 2 - position.width;
            var posY = middlePoint.y + sceneRect.height / 2 - position.height - 10f;
            position = new Rect(posX, posY, maxSize.x, maxSize.y);
        }

        /// <summary>
        /// Initialize visuals of EditorWindow and DragAndDrop operation
        /// </summary>
        void Init() {
            m_Prefab = AssetDatabase.LoadAssetAtPath<GameObject>(m_ObjectPath);

            var container = new VisualElement();
            var picture = new Image {
                image = AssetPreview.GetAssetPreview(m_Prefab)
            };
            container.Add(picture);
            container.RegisterCallback<MouseMoveEvent>(evt => {
                container.RegisterCallback<MouseDownEvent>(ev => {
                    DragAndDrop.PrepareStartDrag();
                    DragAndDrop.objectReferences = new Object[] {m_Prefab};
                    DragAndDrop.paths = new[] {m_ObjectPath};
                    DragAndDrop.StartDrag("Dragging");
                });
            });

            container.RegisterCallback<DragUpdatedEvent>(evt => {
                DragAndDrop.visualMode = DragAndDropVisualMode.Move;
            });

            rootVisualElement.Add(container);
        }
    }
}