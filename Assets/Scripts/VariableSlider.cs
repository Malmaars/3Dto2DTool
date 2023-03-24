using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class VariableSlider : MonoBehaviour, IPointerDownHandler
{
    public float changeSpeed;
    public TMP_InputField[] variableToChange;
    bool sliding;
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Clicked on slider");

        sliding = true;
        BlackBoard.movingObject = true;
        StartCoroutine(ChangeVariable());
    }

    IEnumerator ChangeVariable()
    {
        Vector3 oldMousePosition = Input.mousePosition;

        while (sliding)
        {
            //change the variable based on mouse position

            Vector3 newMousePos = Input.mousePosition;

            //difference between new and old 

            foreach (TMP_InputField vari in variableToChange)
            {
                string newText = (float.Parse(vari.text) + (newMousePos.x - oldMousePosition.x) * changeSpeed).ToString();
                vari.text = newText;
            }

            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            {
                sliding = false;
                BlackBoard.movingObject = false;
            }

            oldMousePosition = Input.mousePosition;
            yield return null;
        }
    }
}
