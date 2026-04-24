using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkedHandAnimator : NetworkBehaviour
{
    public InputActionProperty pinchAnimationAction;
    public InputActionProperty gripAnimationAction;
    public Animator handAnimator;

    [Networked] private float NetworkedTrigger { get; set; }
    [Networked] private float NetworkedGrip { get; set; }

    public override void FixedUpdateNetwork()
    {
        if (HasInputAuthority)
        {
            NetworkedTrigger = pinchAnimationAction.action.ReadValue<float>();
            NetworkedGrip = gripAnimationAction.action.ReadValue<float>();
        }
    }

    public override void Render()
    {
        handAnimator.SetFloat("Trigger", NetworkedTrigger);
        handAnimator.SetFloat("Grip", NetworkedGrip);
    }
}