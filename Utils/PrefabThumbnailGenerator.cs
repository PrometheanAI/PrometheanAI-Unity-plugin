using UnityEngine;
using UnityEditor;
using System.IO;
using PrometheanAI.Modules.Utils;

public static class PrefabThumbnailGenerator 
{

    public static Texture2D GetPrefabThumbnail(string path)
    {
        var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (!asset || !EditorUtility.IsPersistent(asset)) {
                Debug.Log("Unable to create preview for none existing GameObject");
                return null;
            }
        Texture2D thumbnail = AssetPreview.GetAssetPreview(asset);

        if (thumbnail == null)
        {
            thumbnail = GenerateThumbnail(path);
        }

        return thumbnail;
    }

    private static Texture2D GenerateThumbnail(string path)
    {
        // Code to generate a thumbnail goes here
        // For example, you could take a screenshot of the prefab in the scene
        // and resize it to the appropriate size for the thumbnail

        return ScreenshotUtility.RenderPreview(path);
    }


    public static Texture2D CreateThumbnail(GameObject targetObject)
    {
          int thumbnailWidth = 128;
          int thumbnailHeight = 128;

 
        GameObject cameraObject = new GameObject("Thumbnail Camera");
        var _thumbnailCamera = cameraObject.AddComponent<Camera>();
        _thumbnailCamera.enabled = false; // Disable the camera so it doesn't render in the main scene

        Rect originalCameraRect = _thumbnailCamera.rect;

        // Set the camera to render only the target object
        _thumbnailCamera.transform.position = targetObject.transform.position;
        _thumbnailCamera.transform.rotation = targetObject.transform.rotation;
        _thumbnailCamera.transform.Translate(-Vector3.forward * 5); // Adjust the distance from the target object
        _thumbnailCamera.rect = new Rect(0, 0, 1, 1);
        _thumbnailCamera.clearFlags = CameraClearFlags.Color;
        _thumbnailCamera.backgroundColor = Color.clear;
        _thumbnailCamera.cullingMask = 1 << targetObject.layer;

        // Render the target object to a RenderTexture
        RenderTexture renderTexture = new RenderTexture(thumbnailWidth, thumbnailHeight, 24);
        _thumbnailCamera.targetTexture = renderTexture;
        _thumbnailCamera.Render();

        // Read the pixels from the RenderTexture into a Texture2D
        Texture2D thumbnailTexture = new Texture2D(thumbnailWidth, thumbnailHeight, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        thumbnailTexture.ReadPixels(new Rect(0, 0, thumbnailWidth, thumbnailHeight), 0, 0);
        thumbnailTexture.Apply();

        // Clean up
        //RenderTexture.active = null;
        _thumbnailCamera.targetTexture = null;
        _thumbnailCamera.rect = originalCameraRect;
        //Destroy(renderTexture);

        return thumbnailTexture;
    }
}