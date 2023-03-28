using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CameraProperties : MonoBehaviour
{
    public bool movingInScene;
    bool editing;

    [Header("Object Position")]
    public TMP_InputField PositionX;
    public TMP_InputField PositionY;
    public TMP_InputField PositionZ;

    [Header("Object Rotation")]
    public TMP_InputField RotationX;
    public TMP_InputField RotationY;
    public TMP_InputField RotationZ;

    // Start is called before the first frame update
    void Start()
    {
        InitializeVariable(PositionX, "0");
        InitializeVariable(PositionY, "0");
        InitializeVariable(PositionZ, "-5.17");

        InitializeVariable(RotationX, "0");
        InitializeVariable(RotationY, "0");
        InitializeVariable(RotationZ, "0");
    }

    private void Update()
    {
        if (!movingInScene)
            UpdateVisuals();
    }

    void InitializeVariable(TMP_InputField field, string input)
    {
        field.text = input;
    }

    public void StartEdit()
    {
        editing = true;
    }

    public void UpdateVariables()
    {
        if (Camera.main == null)
            return;

        Debug.Log("Update Variables");

        TMP_InputField[] allInputCheck = new TMP_InputField[] { PositionX, PositionY, PositionZ, RotationX, RotationY, RotationZ};

        foreach (TMP_InputField input in allInputCheck)
        {
            float result;
            if (!float.TryParse(input.text, out result))
            {
                input.text = "0";
            }
        }

        Vector3 pos = new Vector3(float.Parse(PositionX.text), float.Parse(PositionY.text), float.Parse(PositionZ.text));
        Camera.main.transform.position = pos;

        Vector3 rot = new Vector3(float.Parse(RotationX.text), float.Parse(RotationY.text), float.Parse(RotationZ.text));
        Camera.main.transform.rotation = Quaternion.Euler(rot);
        movingInScene = false;
        editing = false;

    }

    public void UpdateVisuals()
    {
        if (Camera.main == null || editing)
        {
            return;
        }

        PositionX.text = Camera.main.transform.position.x.ToString("F2");
        PositionY.text = Camera.main.transform.position.y.ToString("F2");
        PositionZ.text = Camera.main.transform.position.z.ToString("F2");

        RotationX.text = Camera.main.transform.rotation.eulerAngles.x.ToString("F2");
        RotationY.text = Camera.main.transform.rotation.eulerAngles.y.ToString("F2");
        RotationZ.text = Camera.main.transform.rotation.eulerAngles.z.ToString("F2");
    }
}
