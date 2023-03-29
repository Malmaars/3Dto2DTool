using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    //stuff we need to save are:
    //The object (perhaps as a path), The object's position, rotation, scale, the camera postition and rotation, the set resolution, the animation, the texture, extra cameras (maybe)

    public Vector3 objectPosition, objectScale, cameraPosition;

    public Quaternion objectRotation, cameraRotation;

    //we save them as separate ints, and not as Vector2, because Vector2 values are floats
    public int xResolution, yResolution;

    //paths to file locations on your computer, this way I don't have to save an entire byte array of an fbx, save files would be very big if I did
    public string objectPath, animPath, texturePath;

    //we need a way to know (and save) which object actually has the animation on it
    public string AnimationObjectName;

    // the values defined in this constructor will be the default values
    // the game starts with when there's no data to load
    public GameData()
    {
        objectPosition = Vector3.zero;
        objectScale = Vector3.zero;
        cameraPosition = new Vector3(0,0, 5.17f);

        objectRotation = new Quaternion(0, 0, 0, 0);
        cameraRotation = Quaternion.Euler(new Vector3(0,180,0));

        xResolution = 1024;
        yResolution = 1024;

        objectPath = "";
        animPath = "";
        objectPath = "";

        AnimationObjectName = "";

    }
}