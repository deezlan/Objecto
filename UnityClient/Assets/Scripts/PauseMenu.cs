using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using System.Threading.Tasks;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private InputActionReference pauseAction; // e.g. Menu button
    [SerializeField] private TextMeshProUGUI roomCodeText;
    [SerializeField] private TextMeshProUGUI scenarioText;
    [SerializeField] private TextMeshProUGUI roleText;
    [SerializeField] private TextMeshProUGUI roleLabel;

    private Camera _camera;
    private bool _isPaused = false;
    private bool _rayInteractorsInitialised = false;
    private XRRayInteractor[] _rayInteractors;
    private NetworkObject _networkObject;

    private void Awake()
    {
        _networkObject = GetComponentInParent<NetworkObject>();
    }

    private void Start()
    {
        if (_networkObject != null && !_networkObject.HasStateAuthority)
        {
            enabled = false;
            menuCanvas.SetActive(false);
            return;
        }

        pauseAction.action.performed += TogglePause;
        pauseAction.action.Enable();
    }

    private void OnDisable()
    {
        pauseAction.action.performed -= TogglePause;
    }

    private void SetRayInteractors(bool enabled)
    {
        if (_rayInteractors == null) return;
        foreach (var ray in _rayInteractors)
            ray.enabled = enabled;
    }

    private void TogglePause(InputAction.CallbackContext ctx)
    {
        _isPaused = !_isPaused;

        if(_isPaused)
        {
            PositionMenuInFrontOfCamera();
            UpdateInfoDisplay();
        }

        menuCanvas.SetActive(_isPaused);
        SetRayInteractors(_isPaused);
    }

    private void PositionMenuInFrontOfCamera()
    {
        if (_camera == null) return;

        Vector3 forward = _camera.transform.forward;
        forward.y = 0; // keep upright, ignore camera tilt
        forward.Normalize();

        transform.position = _camera.transform.position + forward * 1.5f;
        transform.rotation = Quaternion.LookRotation(forward);
    }

    private void UpdateInfoDisplay()
    {
        roomCodeText.text = NetworkManager.Instance.RoomCode;
        scenarioText.text = NetworkManager.Instance.GetScenarioName();

        string role = NetworkManager.Instance.GetRoleName();
        roleText.text = role != "" ? role : "";
        roleText.gameObject.SetActive(role != "");
        roleLabel.gameObject.SetActive(role != "");
    }

    private void LateUpdate()
    {
        if (!_rayInteractorsInitialised)
        {
            _rayInteractors = FindObjectsOfType<XRRayInteractor>();
            if (_rayInteractors != null && _rayInteractors.Length > 0)
            {
                _rayInteractorsInitialised = true;
                SetRayInteractors(false); // disable immediately on first find
            }
        }

        if (_camera == null)
        {
            _camera = Camera.main;
            if (_camera != null && menuCanvas != null)
            {
                Canvas canvas = menuCanvas.GetComponent<Canvas>();
                if (canvas != null)
                    canvas.worldCamera = _camera;
            }
        }
    }

    public void Resume()
    {
        if (!_isPaused) return;
        _isPaused = false;
        menuCanvas.SetActive(false);
        SetRayInteractors(false);
    }

    public async void Disconnect()
    {
        SessionLogger.Instance?.StopLogging();
        await NetworkManager.Instance.Shutdown();
        await Task.Delay(500); // give Photon time to close the session server-side
        SceneManager.LoadScene(0);
    }
}