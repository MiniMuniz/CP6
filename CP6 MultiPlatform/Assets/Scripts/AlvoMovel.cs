using UnityEngine;
using Fusion;

public class AlvoMovel : NetworkBehaviour
{
    [SerializeField] private float distancia = 3f;
    [SerializeField] private float velocidade = 2f;

    [Networked]
    private Vector3 CentroMovimento { get; set; }

    [Networked]
    private float TempoMovimento { get; set; }

    private Alvo alvo;

    public override void Spawned()
    {
        alvo = GetComponent<Alvo>();

        if (HasStateAuthority)
        {
            CentroMovimento = transform.position;
            TempoMovimento = 0f;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
            return;

        if (alvo != null && !alvo.Ativo)
            return;

        TempoMovimento += Runner.DeltaTime;

        float deslocamento =
            Mathf.Sin(TempoMovimento * velocidade) * distancia;

        transform.position =
            CentroMovimento + Vector3.right * deslocamento;
    }

    public void AtualizarCentro(Vector3 novaPosicao)
    {
        if (!HasStateAuthority)
            return;

        CentroMovimento = novaPosicao;
        TempoMovimento = 0f;

        transform.position = novaPosicao;
    }
}