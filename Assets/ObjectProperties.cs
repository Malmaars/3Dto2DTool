using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectProperties : MonoBehaviour
{
    public GameObject editableObject;

    [Header("Object Position")]
    public TMP_InputField PositionX;
    public TMP_InputField PositionY; 
    public TMP_InputField PositionZ;
    
    [Header("Object Rotation")]
    public TMP_InputField RotationX;
    public TMP_InputField RotationY;
    public TMP_InputField RotationZ;

    [Header("Object Scale")]
    public TMP_InputField ScaleX;
    public TMP_InputField ScaleY;
    public TMP_InputField ScaleZ;
    // Start is called before the first frame update
    void Start()
    {
        InitializeVariable(PositionX, editableObject.transform.position.x.ToString());
        InitializeVariable(PositionY, editableObject.transform.position.y.ToString());
        InitializeVariable(PositionZ, editableObject.transform.position.z.ToString());

        InitializeVariable(RotationX, editableObject.transform.rotation.eulerAngles.x.ToString());
        InitializeVariable(RotationY, editableObject.transform.rotation.eulerAngles.y.ToString());
        InitializeVariable(RotationZ, editableObject.transform.rotation.eulerAngles.z.ToString());

        InitializeVariable(ScaleX, editableObject.transform.localScale.x.ToString());
        InitializeVariable(ScaleY, editableObject.transform.localScale.y.ToString());
        InitializeVariable(ScaleZ, editableObject.transform.localScale.z.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        UpdateVariables();
    }

    void InitializeVariable(TMP_InputField field, string input)
    {
        field.text = input;
    }

    void UpdateVariables()
    {
        TMP_InputField[] allInputCheck = new TMP_InputField[] { PositionX, PositionY, PositionZ, RotationX, RotationY, RotationZ, ScaleX, ScaleY, ScaleZ };

        foreach(TMP_InputField input in allInputCheck)
        {
            float result;
            if (!float.TryParse(input.text, out result))
            {
                input.text = "0";
            }
        }

        Vector3 pos = new Vector3(float.Parse(PositionX.text), float.Parse(PositionY.text), float.Parse(PositionZ.text));
        editableObject.transform.position = pos;

        Vector3 rot = new Vector3(float.Parse(RotationX.text), float.Parse(RotationY.text), float.Parse(RotationZ.text));
        editableObject.transform.rotation = Quaternion.Euler(rot);

        Vector3 scl = new Vector3(float.Parse(ScaleX.text), float.Parse(ScaleY.text), float.Parse(ScaleZ.text));
        editableObject.transform.localScale = scl;

    }
}
