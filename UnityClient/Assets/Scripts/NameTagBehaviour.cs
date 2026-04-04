using UnityEngine;

public class NameTagBehaviour : MonoBehaviour
{
    [SerializeField] private Transform headTransform;
    [SerializeField] private float heightOffset = 0.25f;
    private Camera _camera;

    void Start()
    {
        _camera = Camera.main;
    }

    void LateUpdate()
    {
        // Follow head position with offset
        if (headTransform != null)
            transform.position = headTransform.position + Vector3.up * heightOffset;

        // Face local camera
        if (_camera != null)
            transform.rotation = Quaternion.LookRotation(
                transform.position - _camera.transform.position
            );
    }
}