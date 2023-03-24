using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformArrow : MonoBehaviour
{
    ObjectProperties properties;
    public GameObject mouseTracker;
    public GameObject rayPlane;
    Vector2 oldMousePos;

    bool moving;

    float distanceFromMouseToObject;
    Vector3 directionFromMouseToObject;
    Vector3 mouseLocation3D;
    // Start is called before the first frame update
    void Start()
    {
        properties = FindObjectOfType<ObjectProperties>();
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
            MoveUpOrNot();
    }

    private void OnMouseDown()
    {
        Debug.Log("Clicked on arrow");
        oldMousePos = Input.mousePosition;
        mouseTracker.SetActive(true);
        moving = true;

        RaycastHit hit;

        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);

        Vector3 mouseLocation3D = hit.point;

        mouseTracker.transform.position = mouseLocation3D;

        distanceFromMouseToObject = Vector3.Distance(mouseLocation3D, BlackBoard.renderedObject.transform.position);
        directionFromMouseToObject = (BlackBoard.renderedObject.transform.position - mouseLocation3D).normalized;

        properties.movingInScene = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(BlackBoard.renderedObject.transform.position, BlackBoard.renderedObject.transform.position + transform.up * 5);
    }

    public void MoveUpOrNot()
    {
        if (Input.GetMouseButtonUp(0))
        {
            moving = false;
            mouseTracker.SetActive(false);
            properties.movingInScene = false;
        }

        //I somehow need to check what direction it's facing, and get a relative mouse position out of that

        //alternative: get the mouse position when it hits the arrow, then get the exact distance and direction from the object, then always maintain that distance
        RaycastHit hit;
        int layerMask = 1 << 7;

        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, layerMask);

        mouseLocation3D = hit.point;

        mouseTracker.transform.position = BlackBoard.renderedObject.transform.position + (BlackBoard.renderedObject.transform.position - Camera.main.transform.position) * 2;
        mouseTracker.transform.up = Camera.main.transform.position - mouseLocation3D;

        rayPlane.transform.position = mouseLocation3D;
        rayPlane.transform.up = -transform.up;

        layerMask = 1 << 8;
        Physics.Raycast(new Ray(BlackBoard.renderedObject.transform.position, transform.up), out hit, Mathf.Infinity, layerMask);

        Debug.Log("normal direction: " + transform.up);
        if (hit.collider == null)
        {
            Debug.Log("different direction: " +  -transform.up);
            Physics.Raycast(new Ray(BlackBoard.renderedObject.transform.position, -transform.up), out hit, Mathf.Infinity, layerMask);
        }

        Vector3 oldObjectPos = BlackBoard.renderedObject.transform.position;

        BlackBoard.SetRenderObjectPosition(hit.point - (transform.up));

        //BlackBoard.SetRenderObjectPosition(mouseLocation3D + directionFromMouseToObject * distanceFromMouseToObject);
    }
}
