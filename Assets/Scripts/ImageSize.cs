using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ImageSize : MonoBehaviour, IDataPersistence
{

    public TMP_InputField xSize, ySize;

    // Start is called before the first frame update
    void Start()
    {
        CheckSize();
    }

    public void SaveData(GameData _gameData)
    {
        _gameData.xResolution = BlackBoard.visualRT.width;
        _gameData.yResolution = BlackBoard.visualRT.height;
    }

    public void LoadData(GameData _gameData)
    {
        BlackBoard.SetRenderTexture(new RenderTexture(_gameData.xResolution, _gameData.yResolution, 16, RenderTextureFormat.ARGB32));
        xSize.text = _gameData.xResolution.ToString();
        ySize.text = _gameData.yResolution.ToString();
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

    public void CheckSize()
    {
        xSize.text = BlackBoard.visualRT.width.ToString();
        ySize.text = BlackBoard.visualRT.height.ToString();
    }
}
