using System.Collections.Generic;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// data container for GetsAllValidSceneActorsAndPaths Handler
    /// </summary>
    public class AllValidSceneActorsAndPaths
    {
        public string[] scene_names;
        public Dictionary<string, int[]> scene_paths;

        public AllValidSceneActorsAndPaths(List<string> sceneNames, Dictionary<string, int[]> scenePaths) {
            scene_names = sceneNames.ToArray();
            scene_paths = scenePaths;
        }
    }
}