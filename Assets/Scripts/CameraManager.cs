using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Vector3 Pivot;
    public Camera basePhotoCamera;

    List<Camera> photoCameras;

    Vector2 mousePos;

    public float mouseSensitivity;
    public float moveSensitivity;
    public float scrollSpeed;
    // Start is called before the first frame update
    void Start()
    {
        mousePos = Input.mousePosition;

        photoCameras = new List<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        basePhotoCamera.transform.position = transform.position;
        basePhotoCamera.transform.rotation = transform.rotation;

        if (Input.GetMouseButton(1))
        {
            //rotate the camera around the object

            RotateAroundPivot();
            mousePos = Input.mousePosition;
        }

        if (Input.GetMouseButton(2))
        {
            MoveCamera();
        }

        if (Input.mouseScrollDelta.y != 0)
            MoveCloser();
    }

    void SetPivot(Vector3 _newPivot)
    {
        Pivot = _newPivot;
    }

    void RotateAroundPivot()
    {
        float distanceFromTarget = Vector3.Distance(Pivot, transform.position);
        Vector2 mouseDir = mousePos - new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        Vector3 rotateDirection = ((transform.up * mouseDir.y).normalized + (transform.right * mouseDir.x).normalized).normalized;

        transform.RotateAround(Pivot, transform.up, Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime);
        transform.RotateAround(Pivot, transform.right, -Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime);
    }

    void MoveCamera()
    {
        transform.position -= transform.up * Time.deltaTime * Input.GetAxis("Mouse Y") * moveSensitivity;
        transform.position -= transform.right * Time.deltaTime * Input.GetAxis("Mouse X") * moveSensitivity;
    }

    void MoveCloser()
    {
        transform.position = Pivot + (transform.position - Pivot).normalized * (Vector3.Distance(transform.position, Pivot) - Input.mouseScrollDelta.y * scrollSpeed * Time.deltaTime);
    }

    public void Focus()
    {
        SetPivot(BlackBoard.renderedObject.transform.position);
        transform.LookAt(BlackBoard.renderedObject.transform.position);
    }

    public void AddCamera()
    {
        //when the player adds a camera, it will be added from the perspective they're in right then
        GameObject temp = new GameObject();

        Camera newCam = temp.AddComponent<Camera>();

        newCam.CopyFrom(basePhotoCamera);

        photoCameras.Add(newCam);
    }
}
