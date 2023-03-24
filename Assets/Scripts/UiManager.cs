using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UiManager : MonoBehaviour
{
    public GameObject infoBox;
    // Start is called before the first frame update
    void Start()
    {
        HoverObject.Initialize(infoBox, infoBox.GetComponentInChildren<TextMeshProUGUI>());
        Debug.Log(HoverObject.visualObject.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
