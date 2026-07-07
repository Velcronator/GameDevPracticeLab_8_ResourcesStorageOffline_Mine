using System;
using UnityEngine;

public class AnimatorEventBridge : MonoBehaviour
{
    public event Action OnPickupComplete;
    public event Action OnPickupStart;

    // Called by the Unity Animation Event
    public void PickupCompleteAnimationEvent()
    {
        OnPickupComplete?.Invoke();
    }

    public void PickupStartingAnimationEvent()
    {
        OnPickupStart?.Invoke();
    }


}
