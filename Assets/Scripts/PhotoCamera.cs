using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PhotoCamera : MonoBehaviour
{
    Camera thisCamera;

    public RawImage preview;
    RenderTexture rt;

    public RectTransform borderTop, borderBottom, borderLeft, borderRight;
    public TMP_InputField xSize, ySize;
    public void Initialize(Camera _thisCam, RenderTexture _rt)
    {
        thisCamera = _thisCam;
        rt = _rt;

        preview.texture = rt;
        xSize.text = rt.width.ToString();
        ySize.text = rt.height.ToString();
    }

    private void Update()
    {
        SyncPreview();
    }

    public void SyncPreview()
    {
        //preview is an image based on the view of the camera.

        preview.rectTransform.sizeDelta = new Vector2(rt.width, rt.height);
        AlignBorder();

        //I have to change the aspect ratio of the camera based on the changed render texture in runtime
        //BlackBoard.photoCamera.aspect = BlackBoard.visualRT.texelSize.y / BlackBoard.visualRT.texelSize.x;

        float newScale = 1;
        //now to resize it so big and small sizes stay properly within the window
        if (BlackBoard.visualRT.height >= BlackBoard.visualRT.width)
        {
            newScale = 1 / ((float)rt.height / 80);
        }

        else
        {
            newScale = 1 / ((float)rt.width / 80);
        }

        preview.transform.localScale = new Vector3(newScale, newScale, newScale);
    }

    public void AlignBorder()
    {
        borderTop.localPosition = preview.transform.localPosition + new Vector3(0, rt.height * preview.transform.localScale.y / 2, 0);
        borderTop.sizeDelta = new Vector2(rt.width * preview.transform.localScale.x, 2 / transform.localScale.x);

        borderBottom.localPosition = preview.transform.localPosition - new Vector3(0, rt.height * preview.transform.localScale.y / 2, 0);
        borderBottom.sizeDelta = new Vector2(rt.width * preview.transform.localScale.x, 2 / transform.localScale.x);

        borderLeft.localPosition = preview.transform.localPosition - new Vector3(rt.width * preview.transform.localScale.x / 2, 0, 0);
        borderLeft.sizeDelta = new Vector2(2 / transform.localScale.x, rt.height * preview.transform.localScale.y);

        borderRight.localPosition = preview.transform.localPosition + new Vector3(rt.width * preview.transform.localScale.x / 2, 0, 0);
        borderRight.sizeDelta = new Vector2(2 / transform.localScale.x, rt.height * preview.transform.localScale.y);
    }

    public void ApplyNewSize()
    {
        if (xSize.text == null || ySize.text == null)
        {
            return;
        }
        int newXSize = int.Parse(xSize.text);
        int newYSize = int.Parse(ySize.text);

        if (newXSize != 0 && newYSize != 0 && (newXSize != rt.width || newYSize != rt.height))
        {
            //BlackBoard.SetRenderTexture(new RenderTexture(newXSize, newYSize, 16, RenderTextureFormat.ARGB32));
            rt = new RenderTexture(newXSize, newYSize, 16, RenderTextureFormat.ARGB32);
            thisCamera.aspect = rt.texelSize.y / rt.texelSize.x;
            thisCamera.targetTexture = rt;
            preview.texture = rt;
        }
    }

    public void SyncCamera(Camera _syncWith)
    {
        thisCamera.CopyFrom(_syncWith);

        SyncPreview();
        AlignBorder();

        xSize.text = rt.width.ToString();
        ySize.text = rt.height.ToString();
    }
    public void SetCameraPos(Vector3 _newPos)
    {
        thisCamera.transform.position = _newPos;
    }

    public void SetCameraRos(Quaternion _newRot)
    {
        thisCamera.transform.rotation = _newRot;
    }

    public void SetToViewPort()
    {
        thisCamera.CopyFrom(BlackBoard.photoCamera);
        
        rt = new RenderTexture(BlackBoard.visualRT);
        thisCamera.aspect = rt.texelSize.y / rt.texelSize.x;
        thisCamera.targetTexture = rt;
        preview.texture = rt;

        SyncPreview();
        AlignBorder();

        xSize.text = rt.width.ToString();
        ySize.text = rt.height.ToString();
    }

    public void DeleteCamera()
    {
        Destroy(thisCamera.gameObject);
        Destroy(this.gameObject);
    }
}
