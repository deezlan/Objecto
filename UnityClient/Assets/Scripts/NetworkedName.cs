using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;

public class NetworkedName : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI nameTagText;
    [SerializeField] private GameObject nameTagObject;

    [Networked, OnChangedRender(nameof(OnNameChanged))]
    public NetworkString<_16> PlayerName { get; set; }

    public override void Spawned()
    {
        if (IsLocalNetworkRig())
        {
            // Hide own name tag
            nameTagObject.SetActive(false);
            // Set name from device/random
            string name = "Player " + Runner.LocalPlayer.PlayerId;
            RPC_SetName(name);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetName(string name)
    {
        PlayerName = name;
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