using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using SFB; //file browser, since the normal one only works in the editor

public class RenderManager : MonoBehaviour, IDataPersistence
{
    public RectTransform borderTop, borderBottom, borderLeft, borderRight;

    public RenderTexture rtTemp;
    public RawImage renderVisual;
    public Transform renderVisualParent;

    public Camera photoCam;
    public Camera UICam;

    public AnimatorApplier animApplier;

    string objectPath, animPath, texturePath;

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

    public void SaveData(GameData _gameData)
    {
        _gameData.objectPath = objectPath;
        _gameData.animPath = animPath;
        _gameData.texturePath = texturePath;

        _gameData.AnimationObjectName = BlackBoard.animObjectName;
    }

    public void LoadData(GameData _gameData)
    {
        StartCoroutine(Load(_gameData));
    }

    IEnumerator Load(GameData _gameData)
    {
        yield return new WaitForEndOfFrame();
        if (_gameData.objectPath != "")
        {
            objectPath = _gameData.objectPath;
            StartCoroutine(Import3DObject(objectPath));

            //wait for the renderObject to load in
            while (BlackBoard.renderedObject == null)
            {
                yield return null;
            }

            if (_gameData.animPath != "" && _gameData.AnimationObjectName != "")
            {
                animPath = _gameData.animPath;

                //now to find the gameobject in the hierarchy of the renderObject that the animation should be assigned to
                StartCoroutine(LoadAnim(animPath, FindObjectOfNameInChildren(BlackBoard.renderedObject.transform, _gameData.AnimationObjectName)));
            }

            if (_gameData.texturePath != "")
            {
                texturePath = _gameData.texturePath;
                StartCoroutine(ImportTexture(texturePath));
            }
        }
    }

    GameObject FindObjectOfNameInChildren(Transform _parent, string _name)
    {
        if(_parent.gameObject.name == _name)
        {
            return _parent.gameObject;
        }

        if(_parent.childCount > 0)
        {
            for(int i = 0; i < _parent.childCount; i++)
            {
                GameObject temp = FindObjectOfNameInChildren(_parent.GetChild(0), name);
                if (temp != null)
                {
                    return temp;
                }
            }
        }

        return null;
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
            newScale = 1 / ((float)BlackBoard.visualRT.height / 450);
        }

        else
        {
            newScale = 1 / ((float)BlackBoard.visualRT.width / 450);
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

    string GetFileTypeFromPath(string _path)
    {
        char[] pathAsCharArray = _path.ToCharArray();

        List<char> reverseFileTypeChars = new List<char>();

        //we'll force the filetype out of the path. YOU WILL GIVE ME THE FILETYPE
        for (int i = pathAsCharArray.Length - 1; i >= 0; i--)
        {
            if (pathAsCharArray[i] == '.')
            {
                break;
            }

            reverseFileTypeChars.Add(pathAsCharArray[i]);
        }

        string fileType = "";

        for (int i = reverseFileTypeChars.Count - 1; i >= 0; i--)
        {
            fileType += reverseFileTypeChars[i];
        }

        return fileType;
    }

    public void ImportTextureVoid()
    {
        //if there isn't an object already present, throw an error
        if(BlackBoard.renderedObject == null)
        {
            return;
        }

        var extensions = new[] {
        new ExtensionFilter("Image Files", "png", "jpg", "jpeg")
            };

        string path = StandaloneFileBrowser.OpenFilePanel("Select a texture", "", extensions, false)[0];

        StartCoroutine(ImportTexture(path));
    }

    IEnumerator ImportTexture(string path)
    {
        yield return new WaitForEndOfFrame();



        //save texture
        if(path.Length != 0)
        {
            byte[] fileContent = File.ReadAllBytes(path);
            string fileType = GetFileTypeFromPath(path);

            if (Resources.Load("LoadedTexture" + fileType) != null)
            {
                File.Delete(Application.dataPath + "/Resources/LoadedTexture" + fileType + "." + fileType);
            }

            File.WriteAllBytes(Application.dataPath + "/Resources/LoadedTexture" + fileType + "." + fileType, fileContent);
            
            AssetDatabase.Refresh();

            //apply texture to object

            BlackBoard.ApplyTexture(Resources.Load("LoadedTexture" + fileType) as Texture2D);
            texturePath = path;
        }
    }

    public void ImportAnimVoid()
    {
        //if there isn't an object already present, throw an error
        if (BlackBoard.renderedObject == null)
        {
            return;
        }

        animApplier.gameObject.SetActive(true);

        //clear animApplier visuals
        animApplier.ClearHierarchyVisuals();

        string[] paths = StandaloneFileBrowser.OpenFilePanel("Import an animation", "", "anim", false);

        //I hate to make nestled if statements, but I can't return a coroutine, so it'll have to do
        if (paths.Length != 0)
        {
            StartCoroutine(ImportAnim(paths[0]));
        }

        else
        {
            animApplier.Close();
        }
    }

    IEnumerator ImportAnim(string path)
    {
        yield return new WaitForEndOfFrame();

        if (path.Length != 0)
        {
            byte[] fileContent = File.ReadAllBytes(path);
            string fileType = GetFileTypeFromPath(path);
            string fileName = Path.GetFileNameWithoutExtension(path);

            File.WriteAllBytes(Application.dataPath + "/Resources/" + fileName + "." + fileType, fileContent);

            AssetDatabase.Refresh();

            while (Resources.Load(fileName) == null) { yield return null; }

            Debug.Log(Resources.Load(fileName).GetType());
            BlackBoard.SetAnimationClip(Resources.Load(fileName) as AnimationClip);
            animApplier.SetClip(BlackBoard.animClip);
            animApplier.SetHierarchyVisuals(BlackBoard.renderedObject, null);
            animPath = path;
        }
    }

    IEnumerator LoadAnim(string path, GameObject _applyTo)
    {
        yield return new WaitForEndOfFrame();

        if (path.Length != 0)
        {
            byte[] fileContent = File.ReadAllBytes(path);
            string fileType = GetFileTypeFromPath(path);
            string fileName = Path.GetFileNameWithoutExtension(path);

            File.WriteAllBytes(Application.dataPath + "/Resources/" + fileName + "." + fileType, fileContent);

            AssetDatabase.Refresh();

            while (Resources.Load(fileName) == null) { yield return null; }

            Debug.Log(Resources.Load(fileName).GetType());
            BlackBoard.SetAnimationClip(Resources.Load(fileName) as AnimationClip);
            animApplier.SetClip(BlackBoard.animClip);

            //now we set the anim to a gameobject we saved before
            animApplier.ApplyAnimation(_applyTo);
            animApplier.ApplyAnim();
        }
    }


    IEnumerator FindAnimationInFbx()
    {
        //I'd like to check if there's already an animation in the imported fbx, as that is a possibility


        yield return new WaitForEndOfFrame();
    }

    public void ImportObjectVoid()
    {
        var extensions = new[] {
        new ExtensionFilter("3D Files", "fbx", "obj")
            };

        string path = StandaloneFileBrowser.OpenFilePanel("load a 3D object", "", extensions, false)[0];

        StartCoroutine(Import3DObject(path));
    }

    IEnumerator Import3DObject(string path)
    {
        yield return new WaitForEndOfFrame();

        //string path = EditorUtility.OpenFilePanel("load a 3D object", "", "fbx,obj");

        if (path.Length != 0)
        {
            byte[] fileContent = File.ReadAllBytes(path);

            //check what filetype it is, and rewrite it as that filetype

            string fileType = GetFileTypeFromPath(path);

            File.WriteAllBytes(Application.dataPath + "/Resources/LoadedObject"+ fileType+ "." + fileType, fileContent);

            AssetDatabase.Refresh();

            while (Resources.Load("LoadedObject" + fileType) == null) { yield return null; }

            BlackBoard.SetRenderedObject(Instantiate(Resources.Load("LoadedObject" + fileType)) as GameObject);

            objectPath = path;
            //LoadObjectVoid("LoadedObject");
        }
    }

    public void LoadObjectVoid(string fileName)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(Application.streamingAssetsPath);
        print("Streaming Assets Path: " + Application.streamingAssetsPath);
        FileInfo[] allFiles = directoryInfo.GetFiles("*.*");

        foreach (FileInfo file in allFiles)
        {
            Debug.Log(file.Name + ", " + fileName);
            if (file.Name.Contains(fileName))
            {
                StartCoroutine(Load3DObject(file, fileName));
            }
        }
    }
    IEnumerator Load3DObject(FileInfo playerFile, string fileName)
    {
        //1
        if (playerFile.Name.Contains("meta"))
        {
            yield break;
        }
        //2
        else
        {
            string playerFileWithoutExtension = Path.GetFileNameWithoutExtension(playerFile.ToString());
            string[] playerNameData = playerFileWithoutExtension.Split(" "[0]);
            //3
            string tempPlayerName = "";
            int i = 0;
            foreach (string stringFromFileName in playerNameData)
            {
                if (i != 0)
                {
                    tempPlayerName = tempPlayerName + stringFromFileName + " ";
                }
                i++;
            }
            //4
            string wwwPlayerFilePath = "file://" + playerFile.FullName.ToString();

            UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(wwwPlayerFilePath);


            //yield return www;
            yield return www.SendWebRequest();

            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
            GameObject temp = bundle.LoadAsset(fileName) as GameObject;

            //5
            Debug.Log("trying to instantiate");
            BlackBoard.renderedObject = Instantiate(temp);
        }
    }



    public void ExportImageVoid()
    {
        StartCoroutine(ExportImageToFile());
    }

    IEnumerator ExportImageToFile()
    {
        yield return new WaitForEndOfFrame();

        //BlackBoard.photoCamera.cullingMask

        RenderTexture.active = BlackBoard.visualRT;
        Texture2D screenShot = new Texture2D(BlackBoard.visualRT.width, BlackBoard.visualRT.height, TextureFormat.ARGB32, false);

        screenShot.ReadPixels(new Rect(0, 0, BlackBoard.visualRT.width, BlackBoard.visualRT.height), 0, 0);
        screenShot.Apply();

        byte[] bytes = ImageConversion.EncodeArrayToPNG(screenShot.GetRawTextureData(), screenShot.graphicsFormat, (uint)BlackBoard.visualRT.width, (uint)BlackBoard.visualRT.height);

        string path = StandaloneFileBrowser.SaveFilePanel("Save PNG Image", "", "image.png", "png");

        if (path.Length != 0)
        {
            File.WriteAllBytes(path, bytes);
        }
    }

    public void ExportSpriteSheetVoid()
    {
        StartCoroutine(ExportSpriteSheetToFile(30));
    }

    IEnumerator ExportSpriteSheetToFile(int desiredFramerate)
    {
        yield return new WaitForEndOfFrame();

        string path = StandaloneFileBrowser.SaveFilePanel("Save PNG Image", "", "image.png", "png");
        //run the animation, and take a screenshot each frame

        //find out the length of the animation first
        float animLength = BlackBoard.animClip.length;
        Debug.Log(animLength);

        float currentAnimLocation = 0;

        int frameNumber = 1;

        string pathFolder = Path.GetDirectoryName(path);
        string SaveFileName = Path.GetFileNameWithoutExtension(path);
        string fileType = Path.GetExtension(path);

        while (currentAnimLocation <= animLength)
        {

            //so to keep it clear: if the desired framerate is 30, 1 second should be split up in 30 images
            //we can go to a specific place in an animation by passing a value between 0 and 1, so we have to do some calculations
            // 1 / animLength / desiredFrameRate = 1 frame

            //then, we can skip to desired points in the animation, depending on the frame rate, and the length of the animation
            foreach (AnimationState state in BlackBoard.anim)
            {
                state.speed = 0;
                state.time = currentAnimLocation;
                BlackBoard.anim.Play();

                RenderTexture.active = BlackBoard.visualRT;
                Texture2D screenShot = new Texture2D(BlackBoard.visualRT.width, BlackBoard.visualRT.height, TextureFormat.ARGB32, false);

                screenShot.ReadPixels(new Rect(0, 0, BlackBoard.visualRT.width, BlackBoard.visualRT.height), 0, 0);
                screenShot.Apply();

                byte[] bytes = ImageConversion.EncodeArrayToPNG(screenShot.GetRawTextureData(), screenShot.graphicsFormat, (uint)BlackBoard.visualRT.width, (uint)BlackBoard.visualRT.height);

                if (path.Length != 0)
                {
                    File.WriteAllBytes(Path.Combine(pathFolder, SaveFileName) + frameNumber + fileType, bytes);
                }

            }

            //now that we've got all the seperate images of the animation, the trick will be to merge them into a single spritesheet
            //Ideally I want to do this solely through code. The cheeky way would be to render each image in the unity scene, and then take a new screenshot of the images next to each other

            currentAnimLocation += 1 / animLength / desiredFramerate;
            frameNumber++;

            yield return 0;
        }
    }
}
