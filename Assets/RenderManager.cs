using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderManager : MonoBehaviour
{
    public RectTransform borderTop, borderBottom, borderLeft, borderRight;

    public RenderTexture rtTemp;
    public RawImage renderVisual;
    public Transform renderVisualParent;

    public Camera UICamera;

    private void Start()
    {
        BlackBoard.SetRenderTexture(rtTemp);
    }

    private void Update()
    {
        UpdateTheVisual();
    }
    public void UpdateTheVisual()
    {
        renderVisual.rectTransform.sizeDelta = new Vector2(BlackBoard.visualRT.width, BlackBoard.visualRT.height);
        AlignBorder();

        //I have to change the aspect ratio of the camera based on the changed render texture in runtime
        UICamera.aspect = BlackBoard.visualRT.texelSize.y / BlackBoard.visualRT.texelSize.x;

        float newScale = 1;
        //now to resize it so big and small sizes stay properly within the window
        if (BlackBoard.visualRT.height >= BlackBoard.visualRT.width)
        {
            newScale = 1 / ((float)BlackBoard.visualRT.height / 500);
        }

        else
        {
            newScale = 1 / ((float)BlackBoard.visualRT.width / 500);
        }

        renderVisualParent.localScale = new Vector3(newScale, newScale, newScale);

    }

    public void AlignBorder()
    {
        borderTop.localPosition = renderVisual.transform.localPosition + new Vector3(0, BlackBoard.visualRT.height/2, 0);
        borderTop.sizeDelta = new Vector2(BlackBoard.visualRT.width, 2 / renderVisualParent.localScale.x);

        borderBottom.localPosition = renderVisual.transform.localPosition - new Vector3(0, BlackBoard.visualRT.height/2, 0);
        borderBottom.sizeDelta = new Vector2(BlackBoard.visualRT.width, 2 / renderVisualParent.localScale.x);

        borderLeft.localPosition = renderVisual.transform.localPosition - new Vector3(BlackBoard.visualRT.width/2, 0, 0);
        borderLeft.sizeDelta = new Vector2(2 / renderVisualParent.localScale.x, BlackBoard.visualRT.height);

        borderRight.localPosition = renderVisual.transform.localPosition + new Vector3(BlackBoard.visualRT.width / 2, 0, 0);
        borderRight.sizeDelta = new Vector2(2 / renderVisualParent.localScale.x, BlackBoard.visualRT.height);
    }
    public void ExportImageVoid()
    {
        StartCoroutine(ExportImageToFile());
    }

    public IEnumerator ExportImageToFile()
    {
        yield return new WaitForEndOfFrame();

        RenderTexture oldRt = RenderTexture.active;

        RenderTexture.active = BlackBoard.visualRT;
        Texture2D screenShot = new Texture2D(BlackBoard.visualRT.width, BlackBoard.visualRT.height, TextureFormat.ARGB32, false);

        screenShot.ReadPixels(new Rect(0, 0, BlackBoard.visualRT.width, BlackBoard.visualRT.height), 0, 0);
        screenShot.Apply();

        byte[] bytes = ImageConversion.EncodeArrayToPNG(screenShot.GetRawTextureData(), screenShot.graphicsFormat, (uint)BlackBoard.visualRT.width, (uint)BlackBoard.visualRT.height);

        File.WriteAllBytes(Application.dataPath + "/screenshot.png", bytes);

        RenderTexture.active = oldRt;
    }
}
