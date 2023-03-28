using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//sadly just the normal button function can't fulfill my wish to simplify this, so I'l need this script to help
public class ObjectChildSelector : MonoBehaviour
{
    AnimatorApplier applier;
    GameObject sceneRef;

    private void Awake()
    {
        FetchApplier();
    }
    public void FetchApplier()
    {
        applier = GetComponentInParent<AnimatorApplier>();
    }
    public void SelectThis()
    {
        Debug.Log("Click");
        applier.ApplyAnimation(sceneRef);
    }

    public void SetSceneReference(GameObject _newReference)
    {
        sceneRef = _newReference;
    }
}
