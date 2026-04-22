using Fusion;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(NetworkObject))]
public class NetworkedGrabbable : NetworkBehaviour, IStateAuthorityChanged
{
    private Rigidbody _rigidbody;
    private bool _pendingGrab = false;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public override void Spawned()
    {
        bool isTask1 = SceneManager.GetActiveScene().buildIndex == 2;
        if (isTask1 && NetworkManager.Instance.IsGuide)
        {
            GetComponent<XRGrabInteractable>().enabled = false;
            return;
        }

        var grab = GetComponent<XRGrabInteractable>();
        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        // Block grab if another player already holds it
        if (!Object.HasStateAuthority && Object.StateAuthority != PlayerRef.None)
            return;

        _pendingGrab = true;
        Object.RequestStateAuthority();
    }

    public void StateAuthorityChanged()
    {
        if (HasStateAuthority && _pendingGrab)
        {
            _pendingGrab = false;
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
            _rigidbody.constraints = RigidbodyConstraints.None;
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        bool isTask2 = SceneManager.GetActiveScene().buildIndex == 3;

        if (isTask2)
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.useGravity = true;
            _rigidbody.isKinematic = false;
        }
        else
        {
            // Task 1 and Warmup: freeze in place
            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;
        }
    }
}