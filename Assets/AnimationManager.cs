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
        BlackBoard.anim.Stop();
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
