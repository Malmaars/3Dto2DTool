using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectProperties : MonoBehaviour
{
    public bool movingInScene;

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
        InitializeVariable(PositionX, "0");
        InitializeVariable(PositionY, "0");
        InitializeVariable(PositionZ, "0");

        InitializeVariable(RotationX, "0");
        InitializeVariable(RotationY, "0");
        InitializeVariable(RotationZ, "0");

        InitializeVariable(ScaleX, "1");
        InitializeVariable(ScaleY, "1");
        InitializeVariable(ScaleZ, "1");
    }

    private void Update()
    {
        if (!BlackBoard.movingObject)
            UpdateVisuals();
    }

    void InitializeVariable(TMP_InputField field, string input)
    {
        field.text = input;
    }

    public void UpdateVariables()
    {
        if (BlackBoard.renderedObject == null)
            return;


        TMP_InputField[] allInputCheck = new TMP_InputField[] { PositionX, PositionY, PositionZ, RotationX, RotationY, RotationZ, ScaleX, ScaleY, ScaleZ };

        foreach (TMP_InputField input in allInputCheck)
        {
            float result;
            if (!float.TryParse(input.text, out result))
            {
                input.text = "0";
            }
        }

        Vector3 pos = new Vector3(float.Parse(PositionX.text), float.Parse(PositionY.text), float.Parse(PositionZ.text));
        BlackBoard.renderedObject.transform.position = pos;

        Vector3 rot = new Vector3(float.Parse(RotationX.text), float.Parse(RotationY.text), float.Parse(RotationZ.text));
        BlackBoard.renderedObject.transform.rotation = Quaternion.Euler(rot);

        Vector3 scl = new Vector3(float.Parse(ScaleX.text), float.Parse(ScaleY.text), float.Parse(ScaleZ.text));
        BlackBoard.renderedObject.transform.localScale = scl;

    }

    public void UpdateVisuals()
    {
        if(BlackBoard.renderedObject == null)
        {
            return;
        }

        PositionX.text = BlackBoard.renderedObject.transform.position.x.ToString();
        PositionY.text = BlackBoard.renderedObject.transform.position.y.ToString();
        PositionZ.text = BlackBoard.renderedObject.transform.position.z.ToString();

        RotationX.text = BlackBoard.renderedObject.transform.rotation.eulerAngles.x.ToString();
        RotationY.text = BlackBoard.renderedObject.transform.rotation.eulerAngles.y.ToString();
        RotationZ.text = BlackBoard.renderedObject.transform.rotation.eulerAngles.z.ToString();

        ScaleX.text = BlackBoard.renderedObject.transform.localScale.x.ToString();
        ScaleY.text = BlackBoard.renderedObject.transform.localScale.y.ToString();
        ScaleZ.text = BlackBoard.renderedObject.transform.localScale.z.ToString();
    }
}
