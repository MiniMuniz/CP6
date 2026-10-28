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
        if (!HasStateAuthority)
            return;
        Partida match = Partida.Instance;
        if (match == null || !match.CanPlay)
            return;
        if (Vida <= 0)
            return;
        if (info.Source == Object.StateAuthority)
            return;
        Vida = Mathf.Max(0, Vida - 20);
        Debug.Log("Jogador recebeu dano. Vida: " + Vida);
        if (Vida == 0)
        {
            match.RPC_ReportDefeat(Object.StateAuthority);
        }
    }
}