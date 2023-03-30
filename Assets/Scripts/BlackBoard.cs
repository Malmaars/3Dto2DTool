using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class BlackBoard
{
    public static GameObject renderedObject { get; private set; }

    public static RenderTexture visualRT;
    public static RawImage renderImage;

    public static Camera photoCamera;

    public static AnimationClip animClip;
    public static Animation anim;
    public static string animObjectName;

    public static bool movingObject;

    public static bool loading;

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

    public static void SetAnimationClip(AnimationClip _clip)
    {
        animClip = _clip;
    }

    public static void SetRenderObjectPosition(Vector3 _pos)
    {
        renderedObject.transform.position = _pos;
    }

    public static void MoveObject(Vector3 _toMove)
    {
        Debug.Log(_toMove);
        renderedObject.transform.position += _toMove;
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

    public static void IsLoading(bool _loadingState)
    {
        loading = _loadingState;
    }
}
