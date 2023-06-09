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

    public AnimatorApplier animApplier;

    string objectPath, animPath, texturePath;

    int frameRate;

    public GameObject ExportWindow;

    public GameObject LoadingVisual;

    private void Start()
    {
        BlackBoard.SetRenderImage(renderVisual);
        BlackBoard.SetPhotoCamera(photoCam);
        BlackBoard.SetRenderTexture(rtTemp);

        frameRate = 24;
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

                Debug.Log("Loading animation");
                Debug.Log(FindObjectOfNameInChildren(BlackBoard.renderedObject.transform, _gameData.AnimationObjectName));
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

    public void SetFrameRate(int _newFrameRate)
    {
        frameRate = _newFrameRate;
    }

    public void OpenExportWindow()
    {
        ExportWindow.SetActive(true);
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
        StartLoading();
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
        StopLoading();
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
        StartLoading();
        yield return new WaitForEndOfFrame();

        if (path.Length != 0)
        {
            byte[] fileContent = File.ReadAllBytes(path);
            string fileType = GetFileTypeFromPath(path);
            string fileName = Path.GetFileNameWithoutExtension(path);

            File.WriteAllBytes(Application.dataPath + "/Resources/" + fileName + "." + fileType, fileContent);

            AssetDatabase.Refresh();

            while (Resources.Load(fileName) == null) { yield return null; }

            BlackBoard.SetAnimationClip(Resources.Load(fileName) as AnimationClip);
            animApplier.SetClip(BlackBoard.animClip);
            animApplier.SetHierarchyVisuals(BlackBoard.renderedObject, null);
            animPath = path;
        }
        StopLoading();
    }

    IEnumerator LoadAnim(string path, GameObject _applyTo)
    {
        StartLoading();
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
        StopLoading();
    }

    public void ImportObjectVoid()
    {
        var extensions = new[] {
        new ExtensionFilter("3D Files", "fbx")
            };

        string path = StandaloneFileBrowser.OpenFilePanel("load a 3D object", "", extensions, false)[0];

        StartCoroutine(Import3DObject(path));
    }

    IEnumerator Import3DObject(string path)
    {
        StartLoading();
        yield return new WaitForEndOfFrame();

        if (path.Length != 0)
        {
            byte[] fileContent = File.ReadAllBytes(path);

            //check what filetype it is
            string fileType = GetFileTypeFromPath(path);

            //copy the data from the fbx file to the resources folder
            File.WriteAllBytes(Application.dataPath + "/Resources/LoadedObject"+ fileType+ "." + fileType, fileContent);

            AssetDatabase.Refresh();

            while (Resources.Load("LoadedObject" + fileType) == null) { yield return null; }

            //load the object from the resources folder using Resources.Load
            BlackBoard.SetRenderedObject(Instantiate(Resources.Load("LoadedObject" + fileType)) as GameObject);

            objectPath = path;
        }
        StopLoading();
    }

    public void ExportImageVoid()
    {
        StartCoroutine(ExportImageToFile());
    }

    IEnumerator ExportImageToFile()
    {
        StartLoading();
        yield return new WaitForEndOfFrame();

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

        StopLoading();
    }

    public void ExportSpriteSheetVoid()
    {
        string path = StandaloneFileBrowser.SaveFilePanel("Save PNG Image", "", "image.png", "png");
        StartCoroutine(ExportSpriteSheetToFile(frameRate, path));
    }

    IEnumerator ExportSpriteSheetToFile(int desiredFramerate, string path)
    {
        StartLoading();
        yield return new WaitForEndOfFrame();

        if(path.Length == 0)
        {
            StopLoading();
            yield break;
        }

        foreach (AnimationState state in BlackBoard.anim)
        {
            state.time = 0;
            state.speed = 0;
        }

        yield return new WaitForEndOfFrame();
        BlackBoard.anim.Play();
        yield return new WaitForEndOfFrame();

        //run the animation, and take a screenshot each frame

        //find out the length of the animation first
        float animLength = BlackBoard.animClip.length;
        Debug.Log(animLength);

        float currentAnimLocation = 0;

        int frameNumber = 1;

        string pathFolder = Path.GetDirectoryName(path);
        string SaveFileName = Path.GetFileNameWithoutExtension(path);
        string fileType = Path.GetExtension(path);

        //for now we'll the spritesheets linear, so it'll be one long strip
        //to do this, we'll need to calculate the amount of frames beforehand
        int amountOfFrames = Mathf.FloorToInt(desiredFramerate * animLength);
        Debug.Log(amountOfFrames);

        int textureSheetXSize = BlackBoard.visualRT.width * amountOfFrames;
        int textureSheetYSize = BlackBoard.visualRT.height;

        if(textureSheetXSize * textureSheetYSize * 4 > 2130702268)
        {
            Debug.LogError("Texture sheet is too big");
            yield break;
        }


        int currentFrame = 0;

        //*4 because each pixel has 4 values, and will be stored on 4 bytes
        byte[] sheetBytes = new byte[textureSheetXSize * textureSheetYSize * 4];

        while (currentAnimLocation <= animLength)
        {
            Debug.Log(animLength - currentAnimLocation + ", " + currentFrame);
            //so to keep it clear: if the desired framerate is 30, 1 second should be split up in 30 images
            //we can go to a specific place in an animation by passing a value between 0 and 1, so we have to do some calculations
            // 1 / animLength / desiredFrameRate = 1 frame

            //then, we can skip to desired points in the animation, depending on the frame rate, and the length of the animation
            int stateAmount = 0;
            foreach (AnimationState state in BlackBoard.anim)
            {
                stateAmount++;
                state.speed = 0;
                state.time = currentAnimLocation;
                BlackBoard.anim.Play();

                RenderTexture.active = BlackBoard.visualRT;
                Texture2D screenShot = new Texture2D(BlackBoard.visualRT.width, BlackBoard.visualRT.height, TextureFormat.ARGB32, false);

                screenShot.ReadPixels(new Rect(0, 0, BlackBoard.visualRT.width, BlackBoard.visualRT.height), 0, 0);
                screenShot.Apply();

                //the byte array is the size of the texture * 4, we need to keep this in mind when transferring the bytes to a texture sheet
                byte[] addToSheet = screenShot.GetRawTextureData();

                int textureSheetIndex = currentFrame * BlackBoard.visualRT.width * 4 + 4;
                int nextLine = 0;


                for(int i = 0; i < addToSheet.Length; i++)
                { 
                    if(nextLine > (BlackBoard.visualRT.width + 1) * 4)
                    {
                        nextLine = 0;
                        //skip to the next line
                        textureSheetIndex += BlackBoard.visualRT.width * 4 * (amountOfFrames - 1);
                    }

                    sheetBytes[textureSheetIndex - 1] = addToSheet[i];

                    textureSheetIndex++;
                    nextLine++;
                }
            }
            //now that we've got all the seperate images of the animation, the trick will be to merge them into a single spritesheet
            //Ideally I want to do this solely through code. The cheeky way would be to render each image in the unity scene, and then take a new screenshot of the images next to each other

            currentAnimLocation += 1 / (float)desiredFramerate;
            currentFrame++;
            frameNumber++;

            yield return 0;
        }

        byte[] bytes = ImageConversion.EncodeArrayToPNG(sheetBytes, BlackBoard.visualRT.graphicsFormat, (uint)textureSheetXSize, (uint)textureSheetYSize);

        if (path.Length != 0)
        {
            File.WriteAllBytes(Path.Combine(pathFolder, SaveFileName) + fileType, bytes);
        }

        foreach (AnimationState state in BlackBoard.anim)
        {
            state.time = 0;
            state.speed = 1;
        }
        StopLoading();
    }

    public void StartLoading()
    {
        BlackBoard.IsLoading(true);
        LoadingVisual.SetActive(true);
    }
    public void StopLoading()
    {
        BlackBoard.IsLoading(false);
        LoadingVisual.SetActive(false);
    }
}
