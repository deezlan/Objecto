using Fusion;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(NetworkObject))]
public class NetworkedGrabbable : MonoBehaviour
{
    private NetworkObject _networkObject;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _networkObject = GetComponent<NetworkObject>();
        _rigidbody = GetComponent<Rigidbody>();
        
        var grab = GetComponent<XRGrabInteractable>();
        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (!_networkObject.HasStateAuthority && _networkObject.StateAuthority != PlayerRef.None)
            return;

        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = true;
        _rigidbody.constraints = RigidbodyConstraints.None;
        _networkObject.RequestStateAuthority();
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        bool isTask1 = SceneManager.GetActiveScene().buildIndex == 2;

        if (isTask1)
        {
            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;
        }
        else
        {
            _rigidbody.useGravity = true;
            _rigidbody.isKinematic = false;
            _rigidbody.drag = 20f;
            _rigidbody.angularDrag = 20f;
        }

        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }
}