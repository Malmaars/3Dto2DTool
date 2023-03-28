using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB;

public class SaveManager : MonoBehaviour, IDataPersistence
{
    public RenderManager rendManager;
    public void SaveGame()
    {
        string path = StandaloneFileBrowser.SaveFilePanel("Save Scene", "", "scene.save", "save");

        DataPersistenceManager.instance.SaveGame(path);
    }

    public void LoadGame()
    {
        var extensions = new[] {
        new ExtensionFilter("Save Files", "save")
            };

        string path = StandaloneFileBrowser.OpenFilePanel("load a save file", "", extensions, false)[0];

        DataPersistenceManager.instance.LoadGame(path);
    }

    public void NewGame()
    {
        DataPersistenceManager.instance.NewGame();

        //SceneManager.LoadScene(0);
    }

    public void SaveData(GameData _gameData)
    {
        _gameData.objectPosition = BlackBoard.renderedObject.transform.position;
        _gameData.objectScale = BlackBoard.renderedObject.transform.localScale;
        _gameData.objectRotation = BlackBoard.renderedObject.transform.rotation;

        _gameData.cameraPosition = Camera.main.transform.position;
        _gameData.cameraRotation = Camera.main.transform.rotation;

        _gameData.xResolution = BlackBoard.visualRT.width;
        _gameData.yResolution = BlackBoard.visualRT.height;
    }

    public void LoadData(GameData _gameData)
    {
        //foreach (SerializableCollectibleInstance col in gameData.myStuff)
        //{
        //    CollectibleInstance colTemp = new CollectibleInstance(col);
        //    colTemp.CreateSceneRef(spawnPos, new Quaternion(0, 0, 0, 0));
        //    myStuff.Add(colTemp);
        //}
        if(BlackBoard.renderedObject == null)
        {
            rendManager.LoadData(_gameData);
        }

        if (BlackBoard.renderedObject != null)
        {
            BlackBoard.SetRenderObjectPosition(_gameData.objectPosition);
            BlackBoard.renderedObject.transform.localScale = _gameData.objectScale;
            BlackBoard.renderedObject.transform.rotation = _gameData.objectRotation;
        }
        Camera.main.transform.position = _gameData.cameraPosition;
        Camera.main.transform.rotation = _gameData.cameraRotation;
    }
}
