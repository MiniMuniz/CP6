using UnityEngine;
using Fusion;
using static Unity.Collections.Unicode;

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
        Partida match = Partida.Instance;
        if (match != null && match.IsOver)
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
        Vector3 movement = transform.forward * velocidadeBala * Runner.DeltaTime;
        bool hitSomething = Physics.SphereCast(transform.position,raioAcerto,movement.normalized,out RaycastHit hit,movement.magnitude,Physics.DefaultRaycastLayers,QueryTriggerInteraction.Ignore);
        if (hitSomething)
        {
            Health target = hit.collider.GetComponentInParent<Health>();
            if (target != null)
            {
                if (target.Object.StateAuthority ==
                Object.StateAuthority)
                {
                    transform.position += movement;
                    return;
                }
                target.RPC_TomarDano();
                Runner.Despawn(Object);
                return;
            }
            Runner.Despawn(Object);
            return;
        }
        transform.position += movement;
    }
}
