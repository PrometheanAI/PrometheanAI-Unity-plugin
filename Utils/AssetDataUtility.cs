using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PrometheanAI.Modules.Utils
{
    /// <summary>
    /// Class presents a set of tools for generating data  about Objects needed for PrometheanAI
    /// </summary>
    public static class AssetDataUtility
    {
        //
        const string k_SavePath = "C:/PrometheanAITemp/thumbnails/";

        /// <summary>
        /// used to generate data about GameObject at given path needed for PrometheanAI
        /// </summary>
        /// <param name="prefabPath"></param>
        /// <returns>Dictionary of GameObjects parameters as values,and parameters names as keys</returns>
        public static Dictionary<string, object> GetGameObjectData(string prefabPath)
        {
            var outData = new Dictionary<string, object>();
            var currentPath = prefabPath.Replace("StaticMesh", "");
            currentPath = currentPath.Replace("'", "");
            var obj = AssetDatabase.LoadAssetAtPath<GameObject>(currentPath);
            if (obj == null)
            {
                Debug.LogWarning("Trying to get an object data from none existing path");
                return outData;
            }

            var filter = obj.GetComponent<MeshFilter>();
            if (filter == null)
            {
                filter = obj.GetComponentInChildren<MeshFilter>();
            }

            var renderer = obj.GetComponent<Renderer>();
            if (renderer == null)
            {
                renderer = obj.GetComponentInChildren<Renderer>();
            }

            Mesh mesh;
            if (filter != null)
            {
                mesh = filter.sharedMesh;
            }
            else
            {
                var rend = renderer as SkinnedMeshRenderer;
                mesh = rend.sharedMesh;
            }

            var materialsCount = renderer.sharedMaterials.Length;
            var trianglesCount = mesh.triangles.Length;
            var vertexCount = mesh.vertices.Length;
            var uvsCount = 0;
            for (var i = 0; i < 8; i++)
            {
                var uvs = new List<Vector2>();
                mesh.GetUVs(i, uvs);
                if (uvs.Count > 0)
                {
                    uvsCount++;
                }
            }

            outData.Add("material_count", materialsCount);
            outData.Add("face_count", trianglesCount);
            outData.Add("vertex_count", vertexCount);
            outData.Add("uv_sets", uvsCount);
            var lodSys = obj.GetComponent<LODGroup>();
            var lodCount = 1;
            if (lodSys != null)
            {
                lodCount = lodSys.lodCount;
            }

            outData.Add("lod_count", lodCount);
            outData.Add("path", currentPath);
            var name = Path.GetFileName(currentPath);
            var indexToRemove = name.IndexOf(".");
            name = name.Remove(indexToRemove);
            outData.Add("name", name);
            outData.Add("type", "mesh");

            //TODO: same as unreal here?
            outData.Add("vertex_color_channels", 1);

            //TODO get source assets. May be only ue feature
            var materialPaths = new List<string>();
            foreach (var material in renderer.sharedMaterials)
            {
                materialPaths.Add(AssetDatabase.GetAssetPath(material));
            }

            outData.Add("material_paths", materialPaths.ToArray());
            var size = CommandUtility.GetObjectsSize(obj);
            var sizeData = new[] {
                Convert.ToInt32(CommandUtility.Round(size.x, 4)),
                Convert.ToInt32(CommandUtility.Round(size.y, 4)),
                Convert.ToInt32(CommandUtility.Round(size.z, 4))
            };
            outData.Add("bounding_box", sizeData);
            var pivotOffset = CommandUtility.GetPivotOffset(obj);
            var pivotData = new[] {
                Convert.ToInt32(CommandUtility.Round(pivotOffset.x, 4)),
                Convert.ToInt32(CommandUtility.Round(pivotOffset.y, 4)),
                Convert.ToInt32(CommandUtility.Round(pivotOffset.z, 4))
            };
            outData.Add("pivot_offset", pivotData);

            Directory.CreateDirectory(k_SavePath);

            var pathToSave = k_SavePath;
            var thumbnail = PrefabThumbnailGenerator.GetPrefabThumbnail(currentPath);
            //ScreenshotUtility.RenderPreview(currentPath, null);
            if (thumbnail != null)
            {
                var bytes = thumbnail.EncodeToPNG();

                    pathToSave += $"{obj.name}.bmp";
                    File.WriteAllBytes(pathToSave, bytes);


                outData.Add("thumbnail", pathToSave);
            }

            var vertsData = new Dictionary<string, float[]>();
            for (var i = 0; i < mesh.vertices.Length; i++)
            {
                var worldSpaceVert = obj.transform.TransformPoint(mesh.vertices[i]);
                var vertData = new[] {
                    CommandUtility.Round(worldSpaceVert.x, 4),
                    CommandUtility.Round(worldSpaceVert.y, 4),
                    CommandUtility.Round(worldSpaceVert.z, 4)
                };
                vertsData.Add(i.ToString(), vertData);
            }

            var vertOutput = JsonOutputFormatUtility.GenerateJsonString(vertsData);
            var vertPath = k_SavePath + $"verts_{obj.name}";

            if (File.Exists(vertPath))
                File.Delete(vertPath);
            var writer = new StreamWriter(vertPath, true);
            writer.WriteLine(vertOutput);
            writer.Close();


            outData.Add("verts", vertPath);
            return outData;
        }

        /// <summary>
        /// used to generate data about Material at given path needed for PrometheanAI
        /// </summary>
        /// <param name="materialPath"></param>
        /// <returns>Dictionary of Materials parameters as values,and parameters names as keys</returns>
        public static Dictionary<string, object> GetMaterialData(string materialPath)
        {
            var outData = new Dictionary<string, object>();
            var currentPath = "";
            if (materialPath.StartsWith("Material"))
            {
                currentPath = materialPath.Remove(0, 8);
            }
            else if (materialPath.StartsWith("MaterialInstanceConstant"))
            {
                currentPath = materialPath.Replace("MaterialInstanceConstant", "");
            }
            else
            {
                currentPath = materialPath;
            }

            currentPath = currentPath.Replace("'", "");
            var obj = AssetDatabase.LoadAssetAtPath<Material>(currentPath);
            if (obj == null)
            {
                Debug.LogWarning("Trying to get an object data from none existing path");
                return outData;
            }

            outData.Add("path", currentPath);
            var name = Path.GetFileName(currentPath);
            var indexToRemove = name.IndexOf(".");
            name = name.Remove(indexToRemove);
            outData.Add("name", name);
            outData.Add("type", "material");
            outData.Add("parent_path", currentPath);
            outData.Add("is_instance", 0);
            outData.Add("instructions_static", 0);
            outData.Add("instructions_dynamic", 0);
            outData.Add("instructions_vertex", 0);
            var materialTextures = new List<Texture>();
            var shader = obj.shader;

            for (var i = 0; i < ShaderUtil.GetPropertyCount(shader); i++)
            {
                if (ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                {
                    var texture = obj.GetTexture(ShaderUtil.GetPropertyName(shader, i));
                    if (texture != null)
                    {
                        materialTextures.Add(texture);
                    }
                }
            }

            outData.Add("texture_count", materialTextures.Count);
            var texturePaths = new List<string>();
            foreach (var texture in materialTextures)
            {
                var path = AssetDatabase.GetAssetPath(texture);
                if (path != string.Empty)
                {
                    texturePaths.Add(path);
                }
            }

            outData.Add("texture_paths", texturePaths.ToArray());

            //TODO: unreal specific feature?
            outData.Add("is_two_sided", 0);

            //TODO: unreal specific feature?
            outData.Add("is_masked", 0);
            var savePath = k_SavePath;
            var thumbnail = AssetPreview.GetAssetPreview(obj);
            int counter = 0;
            while (thumbnail == null && counter < 100)
            {
                thumbnail = AssetPreview.GetAssetPreview(obj);
                counter++;
                System.Threading.Thread.Sleep(15);
            }

            if (thumbnail == null)
            {
                thumbnail = AssetPreview.GetMiniThumbnail(obj);
            }

            if (thumbnail != null)
            {
                var tmp = RenderTexture.GetTemporary(
                    thumbnail.width,
                    thumbnail.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);
                Graphics.Blit(thumbnail, tmp);
                var previous = RenderTexture.active;
                RenderTexture.active = tmp;
                var myTexture2D = new Texture2D(thumbnail.width, thumbnail.height);
                myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
                myTexture2D.Apply();
                RenderTexture.active = previous;
                if (RenderTexture.active != tmp)
                {
                    RenderTexture.ReleaseTemporary(tmp);
                }

                var bytes = myTexture2D.EncodeToPNG();
                if (!Directory.Exists(k_SavePath))
                    Directory.CreateDirectory(k_SavePath);
                savePath += $"{obj.name}.bmp";
                File.WriteAllBytes(savePath, bytes);


                outData.Add("thumbnail", savePath);
            }

            outData.Add("material_functions", Array.Empty<string>());
            return outData;
        }

        /// <summary>
        /// used to generate data about Texture2d at given path needed for PrometheanAI
        /// </summary>
        /// <param name="texturePath"></param>
        /// <returns>Dictionary of Texture2Ds parameters as values,and parameters names as keys</returns>
        public static Dictionary<string, object> GetTexture2dData(string texturePath)
        {
            var outData = new Dictionary<string, object>();
            var currentPath = texturePath.Replace("Texture2D", "");
            currentPath = currentPath.Replace("'", "");
            var obj = AssetDatabase.LoadAssetAtPath<Texture2D>(currentPath);
            if (obj == null)
            {
                Debug.LogWarning("Trying to get an object data from none existing path");
                return outData;
            }

            var width = obj.width;
            var height = obj.height;
            outData.Add("width", width);
            outData.Add("height", height);
            var importer = (TextureImporter)AssetImporter.GetAtPath(currentPath);
            var hasAlpha = importer.DoesSourceTextureHaveAlpha();
            outData.Add("has_alpha", hasAlpha ? 1 : 0);
            outData.Add("path", currentPath);
            var name = Path.GetFileName(currentPath);
            var indexToRemove = name.IndexOf(".");
            name = name.Remove(indexToRemove);
            outData.Add("name", name);
            outData.Add("type", "texture");
            var savePath = k_SavePath;
            var thumbnail = AssetPreview.GetAssetPreview(obj);
            var counter = 0;
            while (thumbnail == null && counter < 100)
            {
                thumbnail = AssetPreview.GetAssetPreview(obj);
                counter++;
                System.Threading.Thread.Sleep(15);
            }

            if (thumbnail == null)
            {
                thumbnail = AssetPreview.GetMiniThumbnail(obj);
            }

            if (thumbnail != null)
            {
                var tmp = RenderTexture.GetTemporary(
                    thumbnail.width,
                    thumbnail.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);
                Graphics.Blit(thumbnail, tmp);
                var previous = RenderTexture.active;
                RenderTexture.active = tmp;
                var myTexture2D = new Texture2D(thumbnail.width, thumbnail.height);
                myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
                myTexture2D.Apply();
                RenderTexture.active = previous;
                if (RenderTexture.active != tmp)
                {
                    RenderTexture.ReleaseTemporary(tmp);
                }

                var bytes = myTexture2D.EncodeToPNG();
                //    if (Directory.Exists(savePath)) {
                savePath += $"{obj.name}.bmp";
                File.WriteAllBytes(savePath, bytes);
                //    }

                outData.Add("thumbnail", savePath);
            }

            outData.Add("source_path", currentPath);
            return outData;
        }

        /// <summary>
        /// used to generate data about Scene at given path needed for PrometheanAI
        /// </summary>
        /// <param name="scenePath"></param>
        /// <returns>Dictionary of Scenes parameters as values,and parameters names as keys</returns>
        public static Dictionary<string, object> GetSceneData(string scenePath)
        {
            var outData = new Dictionary<string, object>();
            var currentPath = scenePath.Replace("World", "");
            currentPath = currentPath.Replace("'", "");
            var obj = AssetDatabase.LoadAssetAtPath<SceneAsset>(currentPath);
            if (obj == null || currentPath.Length == 0)
            {
                Debug.LogWarning("Trying to get an object data from none existing path");
                return outData;
            }

            var savePath = k_SavePath;
            var screenshotPath = Path.Combine(savePath, obj.name);
            screenshotPath += ".bmp";
            ScreenCapture.CaptureScreenshot(screenshotPath);
            outData.Add("thumbnail", screenshotPath);
            var name = Path.GetFileName(currentPath);
            var indexToRemove = name.IndexOf(".");
            name = name.Remove(indexToRemove);
            outData.Add("path", currentPath);
            outData.Add("name", name);
            outData.Add("type", "level");
            var paths = new List<string>();
            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(currentPath);
            if (sceneAsset != null)
            {
                EditorSceneManager.OpenScene(currentPath, OpenSceneMode.Additive);
                var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
                foreach (var gameObject in allObjects)
                {
                    if (gameObject.scene.IsValid() &&
                        gameObject.scene.name == obj.name)
                    {
                        var path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject);
                        if (path != string.Empty)
                        {
                            paths.Add(path);
                        }
                    }
                }

                var scene = SceneManager.GetSceneByPath(currentPath);
                if (scene.IsValid() && SceneManager.sceneCount > 1)
                {
                    SceneManager.UnloadSceneAsync(scene);
                }
            }

            outData.Add("asset_paths", paths.ToArray());
            return outData;
        }

        /// <summary>
        /// used to generate data about Object at given path needed for PrometheanAI,if its type is unknown
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns>Dictionary of Objects parameters as values,and parameters names as keys</returns>
        public static Dictionary<string, object> GetAssetDataWithoutType(string assetPath)
        {
            var outData = new Dictionary<string, object>();
            var type = AssetDatabase.GetMainAssetTypeAtPath(assetPath).ToString();
            if (type.EndsWith("GameObject"))
            {
                outData = GetGameObjectData(assetPath);
            }
            else if (type.EndsWith("Material"))
            {
                outData = GetMaterialData(assetPath);
            }
            else if (type.EndsWith("SceneAsset"))
            {
                outData = GetSceneData(assetPath);
            }
            else if (type.EndsWith("Texture2D"))
            {
                outData = GetTexture2dData(assetPath);
            }

            return outData;
        }
    }
}