using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PrometheanAI.Modules.Utils
{
    /// <summary>
    /// Class used to generate custom preview for GameObject and save them for PrometheanAI
    /// </summary>
    public static class ScreenshotUtility
    {
        /// <summary>
        /// Creates custom preview for a GameObject using Unity PreviewScene
        /// </summary>
        /// <param name="path"></param>
        /// <param name="renderTexture"></param>
        /// <returns></returns>
        public static Texture2D RenderPreview(string path, RenderTexture renderTexture = null)
        {
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (!asset || !EditorUtility.IsPersistent(asset))
            {
                Debug.Log("Unable to create preview for non-existing GameObject");
                return null;
            }

            RenderTexture rt = renderTexture ?? RenderTexture.GetTemporary(512, 512, 24, RenderTextureFormat.ARGB32);
            var preview = EditorSceneManager.NewPreviewScene();
            var container = new GameObject("Container");
            SceneManager.MoveGameObjectToScene(container, preview);
            var cameraObject = new GameObject("Camera");
            cameraObject.transform.SetParent(container.transform);
            var camera = cameraObject.AddComponent<Camera>();
            camera.scene = preview;
            var go = Object.Instantiate(asset, Vector3.zero, Quaternion.identity, container.transform);
            var bounds = go.GetComponent<Renderer>()?.bounds ?? go.GetComponentInChildren<Renderer>().bounds;
            var size = bounds.size;
            var center = bounds.center;
            var maxDimension = Mathf.Max(size.x, size.y, size.z);
            camera.transform.position = center - new Vector3(0, 0, maxDimension * 1.5f);
            camera.transform.LookAt(center);
            camera.orthographic = true;
            camera.orthographicSize = maxDimension / 2;

            var lightObject = new GameObject("Light");
            lightObject.transform.SetParent(container.transform);
            lightObject.transform.position = center + Vector3.up * maxDimension;
            lightObject.transform.LookAt(center);
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = Color.white;
            light.shadows = LightShadows.Soft;
            light.lightmapBakeType = LightmapBakeType.Realtime;

            camera.targetTexture = rt;
            camera.Render();

            Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
            var prevActiveRT = RenderTexture.active;
            RenderTexture.active = rt;
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            tex.Apply();
            RenderTexture.active = prevActiveRT;

            if (!renderTexture)
                RenderTexture.ReleaseTemporary(rt);

            EditorSceneManager.ClosePreviewScene(preview);
            return tex;
        }

        /// <summary>
        /// Setups Camera for generating objects preview
        /// </summary>
        /// <param name="sceneCamera"></param>
        /// <param name="obj"></param>
        /// <param name="sizeX"></param>
        /// <param name="sizeY"></param>
        /// <param name="zOffset"></param>
        /// <param name="center"></param>
        /// <param name="renderTarget"></param>
        /// <returns> camera initialized for rendering target object </returns>
        static void SetupCamera(Camera sceneCamera, GameObject obj, float sizeX, float sizeY, float zOffset, Vector3 center, RenderTexture renderTarget) {
            sceneCamera.transform.position = obj.transform.position + new Vector3(sizeX, sizeY * 1.5f, zOffset);
            sceneCamera.transform.LookAt(center);
            sceneCamera.cameraType = CameraType.Preview;
            sceneCamera.clearFlags = CameraClearFlags.SolidColor;
            sceneCamera.backgroundColor = Color.gray;
            sceneCamera.orthographic = false;
            sceneCamera.farClipPlane = 78.3f;
            sceneCamera.nearClipPlane = 0.01f;
            sceneCamera.aspect = (float) renderTarget.width / renderTarget.height;
            sceneCamera.fieldOfView = 60.0f;
            sceneCamera.targetTexture = renderTarget;
            sceneCamera.forceIntoRenderTexture = true;
            sceneCamera.renderingPath = RenderingPath.Forward;
        }
    }
}