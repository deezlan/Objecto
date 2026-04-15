using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField roomCodeInputField;
    [SerializeField] private TMP_Dropdown scenarioDropdown; // 0=Warmup, 1=Task 1, 2=Task 2
    [SerializeField] private TMP_Dropdown roleDropdown;     // 0=Guide, 1=Mover
    [SerializeField] private GameObject roleText;

    public void ConnectRoom()
    {
        int sceneIndex = GetSceneIndex();
        bool isGuide = roleDropdown.value == 0;
        NetworkManager.Instance.SetSessionConfig(sceneIndex, isGuide);
        NetworkManager.Instance.ConnectSession(roomCodeInputField.text);
    }

    public void OnScenarioChanged(int index)
    {
        bool isTask1 = index == 1;
        roleText.SetActive(isTask1);
        roleDropdown.gameObject.SetActive(isTask1);
    }

    public void CreateRoom()
    {
        NetworkManager.Instance.CreateSession(roomCodeInputField.text);
    }

    public void JoinRoom()
    {
        NetworkManager.Instance.JoinSession(roomCodeInputField.text);

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
}
