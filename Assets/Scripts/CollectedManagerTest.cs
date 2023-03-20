using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
public class CollectedManagerTest : MonoBehaviour, IDataPersistence
{
    public Collectible[] possibleDrops;

    public Vector3 spawnPos;

    public List<CollectibleInstance> myStuff;

    ObjectPool<CollectibleInstance> collectiblePool;
    // Start is called before the first frame update
    void Start()
    {
        myStuff = new List<CollectibleInstance>();

        collectiblePool = new ObjectPool<CollectibleInstance>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveGame()
    {
        DataPersistenceManager.instance.SaveGame();
    }

    public void LoadGame()
    {
        while (myStuff.Count > 0)
        {
            collectiblePool.ReturnObjectToPool(myStuff[0]);
            myStuff.Remove(myStuff[0]);
        }
        DataPersistenceManager.instance.LoadGame();

        if (myStuff != null && myStuff.Count > 0)
        {
            Debug.Log("name: " + myStuff[0].name + ", description: " + myStuff[0].description + ", model: " + myStuff[0].model + ", set: " + myStuff[0].set + ", uses: " + myStuff[0].uses);
            Debug.Log(myStuff.Count);
        }
    }

    public void NewGame()
    {
        DataPersistenceManager.instance.NewGame();

        while (myStuff.Count > 0)
        {
            collectiblePool.ReturnObjectToPool(myStuff[0]);
            myStuff.Remove(myStuff[0]);
        }
        //SceneManager.LoadScene(0);
    }

    public void SaveData(GameData gameData)
    { 
        foreach(CollectibleInstance item in myStuff)
        {
            gameData.myStuff.Add(new SerializableCollectibleInstance(item));
        }
    }

    public void LoadData(GameData gameData)
    {
        foreach(SerializableCollectibleInstance col in gameData.myStuff)
        {
            CollectibleInstance colTemp = new CollectibleInstance(col);
            colTemp.CreateSceneRef(spawnPos, new Quaternion(0, 0, 0, 0));
            myStuff.Add(colTemp);
        }
    }

    public void CreateCollectible()
    {
        Collectible newCollectibleRef = possibleDrops[Random.Range(0, possibleDrops.Length)];

        CollectibleInstance newCollectibleSceneRef = new CollectibleInstance(newCollectibleRef);

        Debug.Log(newCollectibleSceneRef.name);

        newCollectibleSceneRef.CreateSceneRef(spawnPos, new Quaternion(0, 0, 0, 0));
        myStuff.Add(newCollectibleSceneRef);

    }
}*/
