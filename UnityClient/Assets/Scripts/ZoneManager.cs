using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZoneManager : MonoBehaviour
{
    [SerializeField] private GameObject guideZones;
    [SerializeField] private GameObject moverZones;

    private void Start()
    {
        guideZones.SetActive(NetworkManager.Instance.IsGuide);
        moverZones.SetActive(!NetworkManager.Instance.IsGuide);
    }
}