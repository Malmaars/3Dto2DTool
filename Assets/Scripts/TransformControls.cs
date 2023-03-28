using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RuntimeHandle;

[Serializable]
public enum transformOptions
{
    //since we want to allow only one transform option at a time, let's make it an enum
    none = 0,
    position = 1,
    rotation = 2,
    scaling = 3
}
public class TransformControls : MonoBehaviour
{
    transformOptions currentOption;
    public GameObject positionTransform, rotationTransform, scaleTransform;
    RuntimeTransformHandle positionHandle, rotationHandle, scaleHandle;

    public float transformSize;
    // Start is called before the first frame update
    void Start()
    {
        positionHandle = positionTransform.GetComponent<RuntimeTransformHandle>();
        rotationHandle = rotationTransform.GetComponent<RuntimeTransformHandle>();
        scaleHandle = scaleTransform.GetComponent<RuntimeTransformHandle>();

        StartCoroutine(InitializeTransformOptions());

        ChangeTransformOption(0);
    }

    // Update is called once per frame
    void Update()
    {
        //keep up with the UI representation of the object

        if (BlackBoard.renderedObject != null)
        {
            transform.position = BlackBoard.renderedObject.transform.position;
            transform.up = BlackBoard.renderedObject.transform.up;
            positionHandle.target = BlackBoard.renderedObject.transform;
            rotationHandle.target = BlackBoard.renderedObject.transform;
            scaleHandle.target = BlackBoard.renderedObject.transform;
            
            //rezise the transform controls relative to the camera
            float newScale = Vector3.Distance(BlackBoard.renderedObject.transform.position, Camera.main.transform.position) * transformSize;
            transform.localScale = new Vector3(newScale, newScale, newScale);
        }
    }

    IEnumerator InitializeTransformOptions()
    {
        positionTransform.SetActive(true);
        rotationTransform.SetActive(true);
        scaleTransform.SetActive(true);

        yield return new WaitForEndOfFrame();

        SetLayerOfChildren(positionTransform.transform);
        SetLayerOfChildren(rotationTransform.transform);
        SetLayerOfChildren(scaleTransform.transform);

        positionTransform.SetActive(false);
        rotationTransform.SetActive(false);
        scaleTransform.SetActive(false);

    }

    void SetLayerOfChildren(Transform _parent)
    {
        for(int i = 0; i < _parent.childCount; i++)
        {
            _parent.GetChild(i).gameObject.layer = 6;
            SetLayerOfChildren(_parent.GetChild(i));
        }
    }

    public void ChangeTransformOption(int _newOptionAsInt)
    {
        transformOptions _newOption = (transformOptions)_newOptionAsInt;

        if (_newOption == currentOption)
            return;

        if(BlackBoard.renderedObject == null)
        {
            currentOption = transformOptions.none;
            return;
        }

        currentOption = _newOption;

        positionTransform.SetActive(false);
        rotationTransform.SetActive(false);
        scaleTransform.SetActive(false);

        switch (currentOption)
        {
            case transformOptions.none:
                break;
            case transformOptions.position:
                positionTransform.SetActive(true);
                break;
            case transformOptions.rotation:
                rotationTransform.SetActive(true);
                break;
            case transformOptions.scaling:
                scaleTransform.SetActive(true);
                break;
        }
    }
}
