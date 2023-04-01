using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public void PlayAnimation()
    {
        foreach (AnimationState state in BlackBoard.anim)
        {
            state.time = 0;
            state.speed = 1;
        }
        BlackBoard.anim.Play();
    }

    public void PauseAnimation()
    {
        foreach (AnimationState state in BlackBoard.anim)
        {
            state.speed = 0;
        }
    }

    public void StopAnimation()
    {
        foreach (AnimationState state in BlackBoard.anim)
        {
            state.time = 0;
            state.speed = 0;
        }
    }
}
