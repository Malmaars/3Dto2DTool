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
        DataPersistenceManager.instance.NewGame();
        var extensions = new[] {
        new ExtensionFilter("save Files", "save")
            };

        string path = StandaloneFileBrowser.OpenFilePanel("load a save file", "", extensions, false)[0];

        DataPersistenceManager.instance.LoadGame(path);
    }

    public void NewGame()
    {
        DataPersistenceManager.instance.NewGame();
    }

    public void SaveData(GameData _gameData)
    {
        _gameData.objectPosition = BlackBoard.renderedObject.transform.position;
        _gameData.objectScale = BlackBoard.renderedObject.transform.localScale;
        _gameData.objectRotation = BlackBoard.renderedObject.transform.rotation;

        _gameData.cameraPosition = Camera.main.transform.position;
        _gameData.cameraRotation = Camera.main.transform.rotation;
    }

    public void LoadData(GameData _gameData)
    {
        StartCoroutine(Load(_gameData));
    }

    IEnumerator Load(GameData _gameData)
    {
        yield return new WaitForEndOfFrame();

        if(_gameData.objectPath != "")
        {
            while (BlackBoard.renderedObject == null)
            {
                yield return null;
            }
        }

        if (BlackBoard.renderedObject != null)
        {
            Debug.Log("Set position to " + _gameData.objectPosition);
            BlackBoard.SetRenderObjectPosition(_gameData.objectPosition);
            BlackBoard.renderedObject.transform.localScale = _gameData.objectScale;
            BlackBoard.renderedObject.transform.rotation = _gameData.objectRotation;
        }
        Camera.main.transform.position = _gameData.cameraPosition;
        Camera.main.transform.rotation = _gameData.cameraRotation;
    }
}
