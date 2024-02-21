using UnityEditor;

namespace PrometheanAI.Modules.Utils
{
    /// <summary>
    /// Class presents a methods for simplifying workflow with UnityEditor
    /// </summary>
    public static class EditorUtils
    {
        /// <summary>
        /// Used to refresh  Editor windows after command executed 
        /// </summary>
        public static void RefreshEditorWindows() {
            SceneView.RepaintAll();
            EditorApplication.RepaintHierarchyWindow();
            EditorApplication.DirtyHierarchyWindowSorting();
            var windowType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow");
            EditorWindow.GetWindow(windowType).Repaint();
            EditorApplication.RepaintProjectWindow();
        }
        /// <summary>
        /// Used to refresh Editor windows after Drag & Drop operation
        /// </summary>
        public static void RefreshWindowsAfterDrag() {
            SceneView.RepaintAll();
            EditorApplication.RepaintHierarchyWindow();
            EditorApplication.DirtyHierarchyWindowSorting();
            var windowType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow");
            EditorWindow.GetWindow(windowType).Repaint();
        }
    }
}