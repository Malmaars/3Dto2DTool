using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimatorApplier : MonoBehaviour
{
    public GameObject hierarchyElementPrefab;
    GameObject currentAnimationObject;

    public GameObject animCheck, playButton, pauseButton, stopButton;

    public Transform contentParent;
    int totalChildCount;
    int maxDepth;

    AnimationClip clip;
    Animation currentAnim;

    string selectedObjectName;
    public void ApplyAnimation(GameObject _applyTo)
    {
        if(currentAnimationObject != null && currentAnimationObject != _applyTo)
        {
            //remove the animator from the current animator object
            Destroy(currentAnimationObject.GetComponent<Animation>());
        }

        //and apply it to the new one

        Animation motion;
        if (_applyTo.GetComponent<Animation>())
        {
            motion = _applyTo.GetComponent<Animation>();
        }

        else
        {
            motion = _applyTo.AddComponent<Animation>();
        }

        motion.playAutomatically = true;

        currentAnimationObject = _applyTo;
        motion.clip = clip;

        motion.AddClip(clip, clip.name);
        motion.Play();

        currentAnim = motion;

        selectedObjectName = _applyTo.name;
    }

    public void SetContentSize()
    {
        //since the content doesn't like children (same), I need to edit its width and height manually depending on the actual content

        //the normal width is 0, the normal height is 330
        int newheight = totalChildCount * 10;
        if(newheight < 330)
        {
            newheight = 330;
        }

        int newWidth = (maxDepth * 10);


        contentParent.GetComponent<RectTransform>().sizeDelta = new Vector2(newWidth, newheight);
    }

    public int CheckChildren(Transform _parent)
    {
        int totalChildren = _parent.hierarchyCount;

        return totalChildren;
    }

    public void SetHierarchyVisuals(GameObject _targetObject, Transform _parent)
    {
        GameObject temp;
        if (_parent == null)
        {
            temp = Instantiate(hierarchyElementPrefab, contentParent);
            totalChildCount = 1;
            maxDepth = 0;
        }

        else
        {
            //add as child to parent
            temp = Instantiate(hierarchyElementPrefab, _parent);
            totalChildCount ++;
        }

        //set the name
        temp.name = _targetObject.name;

        //store the scene reference
        temp.GetComponent<ObjectChildSelector>().SetSceneReference(_targetObject);

        temp.GetComponentInChildren<TextMeshProUGUI>().text = temp.name;

        //for the object and each child, create the visuals
        if (_targetObject.transform.childCount > 0 && !temp.GetComponent<VerticalLayoutGroup>())
        {
            //add a vertical layout group with specific settings
            VerticalLayoutGroup layoutGroup = temp.AddComponent<VerticalLayoutGroup>();

            layoutGroup.padding = new RectOffset(20, 0, 10, 0);
            layoutGroup.childForceExpandHeight = true;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childControlHeight = false;
            layoutGroup.childControlWidth = false;
        }

        for (int i = 0; i < _targetObject.transform.childCount; i++)
        {
            SetHierarchyVisuals(_targetObject.transform.GetChild(i).gameObject, temp.transform);
        }

        if(_targetObject.transform.childCount == 0)
        {
            //search for the maxwidth
            Transform toBaseParent = _targetObject.transform;

            int childDepth = 0;

            while(toBaseParent != null)
            {
                childDepth++;
                toBaseParent = toBaseParent.parent;
            }

            if(childDepth > maxDepth)
            {
                maxDepth = childDepth;
            }
        }

        SetContentSize();
    }

    public void ClearHierarchyVisuals()
    {
        for(int i = contentParent.childCount -1; i >= 0; i--)
        {
            Debug.Log("Destroying " + contentParent.GetChild(i).name);
            Destroy(contentParent.GetChild(i).gameObject);
        }
    }


    public void SetClip(AnimationClip _newClip)
    {
        //!IMPORTANT! Mark the animation as legacy, otherwise it won't work with the animation component
        _newClip.legacy = true;

        clip = _newClip;
    }

    public void Cancel()
    {
        selectedObjectName = null;
        currentAnim = null;
        clip = null;
        SetVisuals(false);
        ClearHierarchyVisuals();
        Close();
    }

    public void Close()
    {
        transform.gameObject.SetActive(false);
    }

    public void ApplyAnim()
    {
        BlackBoard.animObjectName = selectedObjectName;
        BlackBoard.anim = currentAnim;
        SetVisuals(true);
        Close();
    }

    void SetVisuals(bool _setTo)
    {
        animCheck.SetActive(_setTo);
        playButton.SetActive(_setTo);
        pauseButton.SetActive(_setTo);
        stopButton.SetActive(_setTo);
    }
}
