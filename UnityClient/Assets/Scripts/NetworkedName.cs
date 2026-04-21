using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;

public class NetworkedName : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI nameTagText;
    [SerializeField] private GameObject nameTagObject;
    [SerializeField] private Renderer headRenderer;

    [Networked, OnChangedRender(nameof(OnNameChanged))]
    public NetworkString<_16> PlayerName { get; set; }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            // Hide own name tag
            nameTagObject.SetActive(false);
            // Set name directly since we have state authority
            PlayerName = NetworkManager.Instance.PlayerName;

            if (headRenderer != null)
                headRenderer.enabled = false; // hide head for local player
        }
        
        // Always update text on spawn for late joiners
        nameTagText.text = PlayerName.ToString();
    }

    private void OnNameChanged()
    {
        nameTagText.text = PlayerName.ToString();
    }

    private bool IsLocalNetworkRig()
    {
        return Object.HasStateAuthority;
    }
}