using UnityEngine;
using UnityEngine.InputSystem;

public class ZoneManager : MonoBehaviour
{
    [SerializeField] private GameObject guideZones;
    [SerializeField] private GameObject moverZones;
    [SerializeField] private InputActionReference cycleAction; // e.g. Secondary Button (X/A)

    private GameObject _activeZones;
    private int _currentIndex = -1; // -1 = all hidden

    private void Start()
    {
        _activeZones = NetworkManager.Instance.IsGuide ? guideZones : moverZones;
        
        _activeZones.SetActive(true); // activate the parent
        SetAllZones(false);           // but hide all children initially
    }

    private void OnEnable()
    {
        cycleAction.action.performed += OnCycle;
        cycleAction.action.Enable();
    }

    private void OnDisable()
    {
        cycleAction.action.performed -= OnCycle;
    }

    private void OnCycle(InputAction.CallbackContext ctx)
    {
        int childCount = _activeZones.transform.childCount;

        // Hide current
        if (_currentIndex >= 0 && _currentIndex < childCount)
            _activeZones.transform.GetChild(_currentIndex).gameObject.SetActive(false);

        // Advance — wraps back to -1 (all hidden) after last zone
        _currentIndex++;
        if (_currentIndex >= childCount)
            _currentIndex = -1;

        // Show next, or leave all hidden if back at -1
        if (_currentIndex >= 0)
            _activeZones.transform.GetChild(_currentIndex).gameObject.SetActive(true);
    }

    private void SetAllZones(bool active)
    {
        foreach (Transform child in _activeZones.transform)
            child.gameObject.SetActive(active);
    }
}