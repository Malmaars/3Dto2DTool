using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public static class HoverObject
{
    public static GameObject visualObject;
    static TextMeshProUGUI objectText;

    public static void Initialize(GameObject _obj, TextMeshProUGUI _txt)
    {
        visualObject = _obj;
        objectText = _txt;
    }

    public static void SetPosition(Vector2 _newPos)
    {
        visualObject.transform.position = _newPos;
    }

    public static void SetText(string _newText)
    {
        objectText.text = _newText;
    }

    public static void EnableObject()
    {
        visualObject.SetActive(true);
    }

    public static void DisableObject()
    {
        visualObject.SetActive(false);
    }
}

public class HoverOverUIElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea(5,10)]
    public string text;

    float timeTillPopUp = 1;

    float timer;
    bool onElement;
    private void Start()
    {
        timer = timeTillPopUp;
    }

    private void Update()
    {
        if (onElement)
        {
            timer -= Time.deltaTime;
        }

        if(timer < 0 && !HoverObject.visualObject.activeSelf)
        {
            //set infobox to just under the mouse
            Vector3 newPos = Input.mousePosition - Vector3.up * 20;

            //check if it falls out of the screen, if it does, nudge it back in

            HoverObject.SetPosition(newPos);
            HoverObject.SetText(text);

            //enable info box
            HoverObject.EnableObject();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onElement = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        onElement = false;
        timer = timeTillPopUp;
        //disable info box
        HoverObject.DisableObject();
    }
}
