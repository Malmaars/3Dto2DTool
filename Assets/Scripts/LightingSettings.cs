using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LightingSettings : Properties, IDataPersistence
{
    //don't forget to save and load these values as well now

    public Light directionLight;

    public TMP_InputField xRot, yRot, zRot;
    public TMP_InputField intensity;

    private void Start()
    {
        UpdateVisuals();
    }

    public void SaveData(GameData _gameData)
    {
        _gameData.lightRotation = directionLight.transform.rotation;
        _gameData.lightIntensity = directionLight.intensity;
    }

    public void LoadData(GameData _gameData)
    {
        directionLight.transform.rotation = _gameData.lightRotation;
        directionLight.intensity = _gameData.lightIntensity;
        UpdateVisuals();
    }

    public override void UpdateVariables()
    {
        directionLight.transform.rotation = Quaternion.Euler(new Vector3(float.Parse(xRot.text), float.Parse(yRot.text), float.Parse(zRot.text)));
        directionLight.intensity = float.Parse(intensity.text);
    }

    public override void UpdateVisuals()
    {
        xRot.text = directionLight.transform.rotation.eulerAngles.x.ToString();
        yRot.text = directionLight.transform.rotation.eulerAngles.y.ToString();
        zRot.text = directionLight.transform.rotation.eulerAngles.z.ToString();

        intensity.text = directionLight.intensity.ToString();
    }
}
