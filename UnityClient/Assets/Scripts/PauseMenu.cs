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

    private Camera _camera;
    private bool _isPaused = false;

    private void OnEnable()
    {
        pauseAction.action.performed += TogglePause;
        pauseAction.action.Enable();
    }

    private void OnDisable()
    {
        pauseAction.action.performed -= TogglePause;
    }

    private void TogglePause(InputAction.CallbackContext ctx)
    {
        _isPaused = !_isPaused;
        menuCanvas.SetActive(_isPaused);

        if(_isPaused)
            UpdateInfoDisplay();
    }

    private void UpdateInfoDisplay()
    {
        roomCodeText.text = NetworkManager.Instance.RoomCode;
        scenarioText.text = NetworkManager.Instance.GetScenarioName();

        string role = NetworkManager.Instance.GetRoleName();
        roleText.text = role != "" ? role : "";
        roleText.gameObject.SetActive(role != "");
    }

    private void LateUpdate()
    {
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

        if (!_isPaused || _camera == null) return;

        transform.position = _camera.transform.position + _camera.transform.forward * 1.5f;
        transform.rotation = Quaternion.LookRotation(transform.position - _camera.transform.position);
    }

    public void Resume()
    {
        _isPaused = false;
        menuCanvas.SetActive(false);
    }

    public async void Disconnect()
    {
        await NetworkManager.Instance.Shutdown();
        await Task.Delay(500); // give Photon time to close the session server-side
        SceneManager.LoadScene(0);
    }
}