using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RenderManager : MonoBehaviour
{
    public RectTransform borderTop, borderBottom, borderLeft, borderRight;

    public RenderTexture rtTemp;
    public RawImage renderVisual;
    public Transform renderVisualParent;

    public Camera photoCam;

    private void Start()
    {
        BlackBoard.SetRenderImage(renderVisual);
        BlackBoard.SetPhotoCamera(photoCam);
        BlackBoard.SetRenderTexture(rtTemp);
    }

    private void Update()
    {
        UpdateTheVisual();
    }
    public void UpdateTheVisual()
    {
        BlackBoard.renderImage.rectTransform.sizeDelta = new Vector2(BlackBoard.visualRT.width, BlackBoard.visualRT.height);
        AlignBorder();

        //I have to change the aspect ratio of the camera based on the changed render texture in runtime
        //BlackBoard.photoCamera.aspect = BlackBoard.visualRT.texelSize.y / BlackBoard.visualRT.texelSize.x;

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
        borderTop.localPosition = BlackBoard.renderImage.transform.localPosition + new Vector3(0, BlackBoard.visualRT.height/2, 0);
        borderTop.sizeDelta = new Vector2(BlackBoard.visualRT.width, 2 / renderVisualParent.localScale.x);

        borderBottom.localPosition = BlackBoard.renderImage.transform.localPosition - new Vector3(0, BlackBoard.visualRT.height/2, 0);
        borderBottom.sizeDelta = new Vector2(BlackBoard.visualRT.width, 2 / renderVisualParent.localScale.x);

        borderLeft.localPosition = BlackBoard.renderImage.transform.localPosition - new Vector3(BlackBoard.visualRT.width/2, 0, 0);
        borderLeft.sizeDelta = new Vector2(2 / renderVisualParent.localScale.x, BlackBoard.visualRT.height);

        borderRight.localPosition = BlackBoard.renderImage.transform.localPosition + new Vector3(BlackBoard.visualRT.width / 2, 0, 0);
        borderRight.sizeDelta = new Vector2(2 / renderVisualParent.localScale.x, BlackBoard.visualRT.height);
    }

    public void ImportObjectVoid()
    {
        StartCoroutine(Import3DObject());

    }

    public IEnumerator Import3DObject()
    {
        yield return new WaitForEndOfFrame();

        string path = EditorUtility.OpenFilePanel("load a 3D object", "", "fbx,obj");

        if (path.Length != 0)
        {
            byte[] fileContent = File.ReadAllBytes(path);

            //check what filetype it is, and rewrite it as that filetype

            char[] pathAsCharArray = path.ToCharArray();

            List<char> reverseFileTypeChars = new List<char>();

            //we'll force the filetype out of the path. YOU WILL GIVE ME THE FILETYPE
            for (int i = pathAsCharArray.Length - 1; i >= 0; i--)
            {
                if(pathAsCharArray[i] == '.')
                {
                    break;
                }

                reverseFileTypeChars.Add(pathAsCharArray[i]);
            }

            string fileType = "";

            for(int i = reverseFileTypeChars.Count - 1; i >= 0; i--)
            {
                fileType += reverseFileTypeChars[i];
            }

            Debug.Log(fileType);


            File.WriteAllBytes(Application.dataPath + "/Resources/LoadedObject." + fileType, fileContent);
        }
    }


    public void ExportImageVoid()
    {
        StartCoroutine(ExportImageToFile());
    }

    public IEnumerator ExportImageToFile()
    {
        yield return new WaitForEndOfFrame();

        RenderTexture.active = BlackBoard.visualRT;
        Texture2D screenShot = new Texture2D(BlackBoard.visualRT.width, BlackBoard.visualRT.height, TextureFormat.ARGB32, false);

        screenShot.ReadPixels(new Rect(0, 0, BlackBoard.visualRT.width, BlackBoard.visualRT.height), 0, 0);
        screenShot.Apply();

        byte[] bytes = ImageConversion.EncodeArrayToPNG(screenShot.GetRawTextureData(), screenShot.graphicsFormat, (uint)BlackBoard.visualRT.width, (uint)BlackBoard.visualRT.height);

        string path = EditorUtility.SaveFilePanel("Save PNG Image", "", "image.png", "png");

        if (path.Length != 0)
        {
            File.WriteAllBytes(path, bytes);
        }
    }
}
