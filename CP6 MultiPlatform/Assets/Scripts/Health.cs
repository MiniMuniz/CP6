using UnityEngine;
using Fusion;

public class Health : NetworkBehaviour
{
    [Networked]
    public int Vida { get; set; }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            Vida = 100;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_TomarDano(RpcInfo info = default)
    {
       
    }
}