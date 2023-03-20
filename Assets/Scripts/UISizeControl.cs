using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISizeControl : MonoBehaviour
{
    public Camera wideViewCam;
    public RawImage wideViewImage;
    public RenderTexture wideViewTexture;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (wideViewTexture.width != (int)wideViewImage.rectTransform.rect.width || wideViewTexture.height != (int)wideViewImage.rectTransform.rect.height)
        {
            wideViewTexture = new RenderTexture((int)wideViewImage.rectTransform.rect.width, (int)wideViewImage.rectTransform.rect.height, 16, RenderTextureFormat.ARGB32);

            wideViewCam.targetTexture = wideViewTexture;
            wideViewImage.texture = wideViewTexture;

            wideViewCam.aspect = wideViewImage.rectTransform.rect.x / wideViewImage.rectTransform.rect.y;
        }

    }
}
