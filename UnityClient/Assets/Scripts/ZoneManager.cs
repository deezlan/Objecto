using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ZoneManager : MonoBehaviour
{
    [SerializeField] private InputActionReference cycleAction; // e.g. A Button on Right Hand controller

    [Header("Task 1 - Goal State Ghost Objects")]
    [SerializeField] private GameObject goalStateObjects;

    [Header("Task 2 - Negotiation Zones")]
    [SerializeField] private GameObject guideZones;
    [SerializeField] private GameObject moverZones;

    private GameObject _activeZones;
    private int _currentIndex = -1; // -1 = all hidden
    private bool _isTask1;
    private bool _goalVisible = false;

    private void Start()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        _isTask1 = sceneIndex == 2;

        if (_isTask1)
        {
            // Only Guide sees the goal toggle — hide for Mover entirely
            if (goalStateObjects != null)
                goalStateObjects.SetActive(false);

            if (!NetworkManager.Instance.IsGuide)
                enabled = false; // disable ZoneManager entirely for Mover in Task 1
        }
        else
        {
            // Task 2 setup
            guideZones.SetActive(false);
            moverZones.SetActive(false);

            if (NetworkManager.Instance.IsGuide) {
                guideZones.SetActive(true);
            } else {
                moverZones.SetActive(true);
            }
            _activeZones = NetworkManager.Instance.IsGuide 
                ? guideZones : moverZones; // activate the parent
            SetAllZones(false);            // but hide all children initially
        }
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
        if (_isTask1)
        {
            // Simple toggle for goal state
            _goalVisible = !_goalVisible;
            goalStateObjects.SetActive(_goalVisible);
        }
        else
        {
            // Task 2 Zone cycling
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
    }

    private void SetAllZones(bool active)
    {
        foreach (Transform child in _activeZones.transform)
            child.gameObject.SetActive(active);
    }
}