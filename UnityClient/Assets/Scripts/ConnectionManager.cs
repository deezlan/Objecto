using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    [SerializeField] private int minRoomCodeLength = 1;
    [SerializeField] private TMP_InputField roomCodeInputField;
    [SerializeField] private TMP_Dropdown scenarioDropdown; // 0=Warmup, 1=Task 1, 2=Task 2
    [SerializeField] private TMP_Dropdown roleDropdown;     // 0=Guide, 1=Mover
    [SerializeField] private GameObject roleText;
    [SerializeField] private UnityEngine.UI.Button connectButton;

    private void Start()
    {
        connectButton.onClick.AddListener(ConnectRoom);
        roomCodeInputField.onValueChanged.AddListener(OnRoomCodeChanged);
        connectButton.interactable = false; // disabled until minimum met
    }

    public void ConnectRoom()
    {
        int sceneIndex = GetSceneIndex();
        bool isGuide = roleDropdown.value == 0;
        NetworkManager.Instance.SetSessionConfig(sceneIndex, isGuide, roomCodeInputField.text);
        NetworkManager.Instance.ConnectSession(roomCodeInputField.text);
    }

    public void OnScenarioChanged(int index)
    {
        bool isTask1 = index == 1;
        bool isTask2 = index == 2;

        roleText.SetActive(isTask1 || isTask2);
        roleDropdown.gameObject.SetActive(isTask1 || isTask2);

        if (isTask1)
        {
            roleDropdown.options[0].text = "Guide";
            roleDropdown.options[1].text = "Mover";
        }
        else if (isTask2)
        {
            roleDropdown.options[0].text = "Player A";
            roleDropdown.options[1].text = "Player B";
        }

        roleDropdown.RefreshShownValue();
    }

    private int GetSceneIndex()
    {
        // Map dropdown index to your Build Settings scene index
        // 0 = Warmup, 1 = Task1, 2 = Task2
        return scenarioDropdown.value switch
        {
            0 => 1, // Warmup scene
            1 => 2, // Task 1 scene
            2 => 3, // Task 2 scene
            _ => 1
        };
    }

    private void OnRoomCodeChanged(string value)
    {
        connectButton.interactable = value.Length >= minRoomCodeLength;
    }
}
