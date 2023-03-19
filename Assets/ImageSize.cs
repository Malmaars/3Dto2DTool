using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ImageSize : MonoBehaviour
{

    public TMP_InputField xSize, ySize;

    // Start is called before the first frame update
    void Start()
    {
        xSize.text = BlackBoard.visualRT.width.ToString();
        ySize.text = BlackBoard.visualRT.height.ToString();
    }

    public void ApplyNewSize()
    {
        if (xSize.text == null || ySize.text == null)
        {
            return;
        }
        int newXSize = int.Parse(xSize.text);
        int newYSize = int.Parse(ySize.text);

        if (newXSize != 0 && newYSize != 0 && (newXSize != BlackBoard.visualRT.width || newYSize != BlackBoard.visualRT.height))
        {
            BlackBoard.SetRenderTexture(new RenderTexture(newXSize, newYSize, 16, RenderTextureFormat.ARGB32));
        }
    }
}
