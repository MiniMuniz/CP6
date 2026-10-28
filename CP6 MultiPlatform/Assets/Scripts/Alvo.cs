using UnityEngine;
using Fusion;

public class Alvo : NetworkBehaviour
{
    [Header("Pontuação")]
    [SerializeField] private int pontosPorAcerto = 10;

    [Header("Respawn")]
    [SerializeField] private float tempoRespawn = 3f;
    [SerializeField] private Transform[] posicoesRespawn;

    [Header("Componentes")]
    [SerializeField] private Collider colisor;
    [SerializeField] private Renderer visual;

    [Networked] public NetworkBool Ativo { get; set; }
    [Networked] private TickTimer TimerRespawn { get; set; }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            Ativo = true;
            TimerRespawn = TickTimer.None;
        }

        AtualizarVisual();
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
            return;

        if (!Ativo && TimerRespawn.Expired(Runner))
        {
            Reaparecer();
        }
    }

    public void ReceberAcerto(PlayerRef jogador)
    {
        if (!Ativo)
            return;

        if (HasStateAuthority)
            RegistrarAcerto(jogador);
        else
            RPC_RegistrarAcerto(jogador);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RegistrarAcerto(
        PlayerRef jogador,
        RpcInfo info = default)
    {
        if (info.Source != jogador)
            return;

        RegistrarAcerto(jogador);
    }

    private void RegistrarAcerto(PlayerRef jogador)
    {
        if (!HasStateAuthority || !Ativo)
            return;

        Partida partida = Partida.Instance;

        if (partida == null)
        {
            Debug.LogWarning("Alvo: Partida não encontrada!");
            return;
        }

        if (!partida.CanPlay)
        {
            Debug.LogWarning("Alvo: Partida não está ativa!");
            return;
        }

        Ativo = false;

        TimerRespawn = TickTimer.CreateFromSeconds(
            Runner, tempoRespawn
        );

        AtualizarVisual();

        partida.RPC_AdicionarPontos(
            jogador, pontosPorAcerto
        );

        Debug.Log(
            "Alvo atingido! Respawn em " +
            tempoRespawn + " segundos."
        );
    }

    private void Reaparecer()
    {
        if (posicoesRespawn != null &&
            posicoesRespawn.Length > 0)
        {
            int indice = Random.Range(
                0, posicoesRespawn.Length
            );

            if (posicoesRespawn[indice] != null)
            {
                Vector3 novaPosicao =
                    posicoesRespawn[indice].position;

                AlvoMovel movel =
                    GetComponent<AlvoMovel>();

                if (movel != null)
                    movel.AtualizarCentro(novaPosicao);
                else
                    transform.position = novaPosicao;
            }
        }

        TimerRespawn = TickTimer.None;
        Ativo = true;

        AtualizarVisual();

        Debug.Log("Alvo reapareceu!");
    }

    public override void Render()
    {
        AtualizarVisual();
    }

    private void AtualizarVisual()
    {
        if (visual != null)
            visual.enabled = Ativo;

        if (colisor != null)
            colisor.enabled = Ativo;
    }
}