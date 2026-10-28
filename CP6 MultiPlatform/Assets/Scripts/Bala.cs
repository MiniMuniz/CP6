using UnityEngine;
using Fusion;

public class Bala : NetworkBehaviour
{
    [SerializeField] private float velocidadeBala = 12f;
    [SerializeField] private float vidaBala = 2.5f;
    [SerializeField] private float raioAcerto = 0.15f;

    private float vidaSobrando;

    public override void Spawned()
    {
        vidaSobrando = vidaBala;
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
            return;

        Partida partida = Partida.Instance;

        if (partida == null || !partida.CanPlay)
        {
            Runner.Despawn(Object);
            return;
        }

        vidaSobrando -= Runner.DeltaTime;

        if (vidaSobrando <= 0)
        {
            Runner.Despawn(Object);
            return;
        }

        Vector3 movimento =
            transform.forward * velocidadeBala * Runner.DeltaTime;

        bool acertou = Physics.SphereCast(
            transform.position,
            raioAcerto,
            movimento.normalized,
            out RaycastHit hit,
            movimento.magnitude,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Collide
        );

        if (acertou)
        {
            Alvo alvo = hit.collider.GetComponentInParent<Alvo>();

            if (alvo != null)
            {
                PlayerRef jogador = Object.StateAuthority;

                alvo.ReceberAcerto(jogador);

                Runner.Despawn(Object);
                return;
            }

            // Ignora colisões com jogadores
            Health jogadorAtingido =
                hit.collider.GetComponentInParent<Health>();

            if (jogadorAtingido != null)
            {
                transform.position += movimento;
                return;
            }

            // Colisão com cenário
            Runner.Despawn(Object);
            return;
        }

        transform.position += movimento;
    }
}
