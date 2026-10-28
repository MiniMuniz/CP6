using Fusion;
using System.Linq;
using UnityEngine;

public class Partida : NetworkBehaviour
{
    public static Partida Instance { get; private set; }

    [SerializeField] private float duracaoPartida = 60f;

    [Networked] public NetworkBool IsOver { get; set; }
    [Networked] public PlayerRef Winner { get; set; }
    [Networked] public PlayerRef BluePlayer { get; set; }
    [Networked] public int PontosAzul { get; set; }
    [Networked] public int PontosVermelho { get; set; }
    [Networked] public TickTimer TempoPartida { get; set; }
    [Networked] public NetworkBool Iniciada { get; set; }

    public bool CanPlay =>
        Iniciada && !IsOver &&
        Runner.ActivePlayers.Count() == 2;

    public override void Spawned()
    {
        Instance = this;

        if (HasStateAuthority)
        {
            IsOver = false;
            Iniciada = false;
            PontosAzul = 0;
            PontosVermelho = 0;
            Winner = PlayerRef.None;
            BluePlayer = Runner.LocalPlayer;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
            return;

        if (!Iniciada && Runner.ActivePlayers.Count() == 2)
        {
            Iniciada = true;
            TempoPartida = TickTimer.CreateFromSeconds(
                Runner, duracaoPartida
            );

            Debug.Log("Partida iniciada!");
        }

        if (!Iniciada || IsOver)
            return;

        if (TempoPartida.Expired(Runner))
            FinalizarPartida();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_AdicionarPontos(
        PlayerRef jogador,
        int pontos,
        RpcInfo info = default)
    {
        if (!HasStateAuthority || !CanPlay)
            return;

        if (pontos != 10 && pontos != 20)
            return;

        if (!Runner.ActivePlayers.Contains(jogador))
            return;

        if (jogador == BluePlayer)
            PontosAzul += pontos;
        else
            PontosVermelho += pontos;

        Debug.Log(
            "Pontos registrados: " + jogador +
            " recebeu " + pontos
        );
    }

    private void FinalizarPartida()
    {
        if (PontosAzul > PontosVermelho)
        {
            Winner = BluePlayer;
        }
        else if (PontosVermelho > PontosAzul)
        {
            foreach (PlayerRef jogador in Runner.ActivePlayers)
            {
                if (jogador != BluePlayer)
                {
                    Winner = jogador;
                    break;
                }
            }
        }
        else
        {
            Winner = PlayerRef.None;
        }

        IsOver = true;

        Debug.Log("Partida encerrada!");
    }

    public override void Despawned(
        NetworkRunner runner, bool hasState)
    {
        if (Instance == this)
            Instance = null;
    }
}