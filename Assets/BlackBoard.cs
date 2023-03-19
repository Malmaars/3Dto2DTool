using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class BlackBoard
{
    public static RenderTexture visualRT;

    public static void SetRenderTexture(RenderTexture _newRt)
    {
        visualRT = _newRt;
    }
}
