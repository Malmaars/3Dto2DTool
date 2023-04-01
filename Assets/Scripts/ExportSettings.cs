using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExportSettings : MonoBehaviour
{
    int frameRate = 24;
    int totalFrames;

    int resolutionX, resolutionY;

    public TextMeshProUGUI spriteSheetResolution, animationLength, totalFramesText;
    public TMP_InputField frameRateInputField;

    public GameObject SpriteSheetBlocker;

    public RenderManager renManager;

    private void OnEnable()
    {
        if (CheckForAnimation())
        {
            SpriteSheetBlocker.SetActive(false);
            FetchAnimationInfo();
            setChangeableInfo();
        }
        else
        {
            SpriteSheetBlocker.SetActive(true);
        }
    }

    bool CheckForAnimation()
    {
        if(BlackBoard.anim != null)
        {
            return true;
        }
        return false;
    }

    public void SetFrameRate()
    {
        frameRate = int.Parse(frameRateInputField.text);
        renManager.SetFrameRate(frameRate);
        setChangeableInfo();
    }

    public void FetchAnimationInfo()
    {
        animationLength.text = BlackBoard.animClip.length.ToString() + "s";
    }

    public void setChangeableInfo()
    {
        totalFrames = Mathf.FloorToInt(frameRate * BlackBoard.animClip.length);
        totalFramesText.text = totalFrames.ToString();

        resolutionY = BlackBoard.visualRT.height;
        resolutionX = totalFrames * BlackBoard.visualRT.width;

        spriteSheetResolution.text = resolutionX + "x" + resolutionY;
    }

    public void CloseThis()
    {
        this.gameObject.SetActive(false);
    }
}
