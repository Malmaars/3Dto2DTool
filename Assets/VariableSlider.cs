using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VariableSlider : MonoBehaviour
{
    public float changeSpeed;
    public TMP_InputField variableToChange;
    bool sliding;
    private void OnMouseDown()
    {
        Debug.Log("Clicked on slider");

        sliding = true;
        StartCoroutine(ChangeVariable());
    }

    IEnumerator ChangeVariable()
    {
        Debug.Log("sliding started");

        Vector3 oldMousePosition = Input.mousePosition;

        while (sliding)
        {
            //change the variable based on mouse position

            Vector3 newMousePos = Input.mousePosition;

            Debug.Log(newMousePos.x + ", " + oldMousePosition.x);

            //difference between new and old 

            string newText = (float.Parse(variableToChange.text) + (newMousePos.x - oldMousePosition.x) * changeSpeed).ToString();
            variableToChange.text = newText;
            //get the difference betwee

            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            {
                sliding = false;
            }

            oldMousePosition = Input.mousePosition;
            yield return null;
        }

        Debug.Log("sliding ended");
    }
}
