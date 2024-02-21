using System;
using System.Collections.Generic;
using PrometheanAI.Modules.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Data container for GetSelectedAndVisibleStaticMeshActors handler
    /// </summary>
    [Serializable]
    public class InitialSceneData
    {
        public string[] selected_names;
        public string[] rendered_names;
        public Dictionary<int, object[]>[] selected_locations;
        public Dictionary<int, object[]>[] rendered_locations;
        public Dictionary<string, int[]> selected_paths;
        public Dictionary<string, int[]> rendered_paths;
        public string scene_name;
        public float[] camera_location;
        public float[] camera_direction;

        public InitialSceneData(Dictionary<int, object[]> selectedLocations,
            Dictionary<int, object[]> renderedLocations,
            List<string> selectedNames,
            List<string> renderedNames, Dictionary<string, int[]> selectedPaths, Dictionary<string, int[]> renderedPath,
            Vector3 cameraPos, Vector3 cameraDir) {
            selected_names = selectedNames.ToArray();
            rendered_names = renderedNames.ToArray();
            selected_locations = new[] {selectedLocations};
            rendered_locations = new[] {renderedLocations};
            selected_paths = selectedPaths;
            rendered_paths = renderedPath;
            scene_name = SceneManager.GetActiveScene().name;
            camera_location = new[] {
                CommandUtility.Round(CommandUtility.ConvertToCentimeters(cameraPos.x), 4),
                CommandUtility.Round(CommandUtility.ConvertToCentimeters(cameraPos.y), 4),
                CommandUtility.Round(CommandUtility.ConvertToCentimeters(cameraPos.z), 4)
            };
            camera_direction = new[] {
                CommandUtility.Round(cameraDir.x, 4),
                CommandUtility.Round(cameraDir.y, 4),
                CommandUtility.Round(cameraDir.z, 4)
            };
        }
    }
}