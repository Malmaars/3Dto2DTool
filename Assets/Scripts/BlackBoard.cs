using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class BlackBoard
{
    public static GameObject renderedObject;

    public static RenderTexture visualRT;
    public static RawImage renderImage;

    public static Camera photoCamera;

    public static void ApplyTexture(Texture2D _tex)
    {
        MeshRenderer[] meshes =  renderedObject.GetComponentsInChildren<MeshRenderer>();

        Debug.Log(meshes.Length);
        foreach(MeshRenderer mesh in meshes)
        {
            //apply the texture
            mesh.material.SetTexture("_MainTex", _tex);
        }
    }
    public static void SetRenderedObject(GameObject _newObject)
    {
        Object.Destroy(renderedObject);
        renderedObject = _newObject;
    }

    public static void SetRenderImage(RawImage _newRI)
    {
        renderImage = _newRI;
    }

    public static void SetPhotoCamera(Camera _newCam)
    {
        photoCamera = _newCam;
    }

    public static void SetRenderTexture(RenderTexture _newRt)
    {
        visualRT = _newRt;
        photoCamera.aspect = visualRT.texelSize.y / visualRT.texelSize.x;
        photoCamera.targetTexture = visualRT;
        renderImage.texture = visualRT;
    }
}
