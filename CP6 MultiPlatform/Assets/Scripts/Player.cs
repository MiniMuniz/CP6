using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.Collections.Unicode;

public class Player : NetworkBehaviour
{


    [SerializeField] private float velocidade = 5f;
    [SerializeField] private CharacterController controller;

    [SerializeField] private MeshRenderer bodyRenderer;

    [SerializeField] private NetworkObject bulletPrefab;
    [SerializeField] private Transform armaPosition;

    [Networked, OnChangedRender(nameof(CorTeam))]

    public byte TeamColor { get; set; }

    private bool firePending;


    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            TeamColor = (byte)(
            Runner.IsSharedModeMasterClient ? 0 : 1
            );
        }
        CorTeam();
    }
    private void CorTeam()
    {
        if (bodyRenderer == null)
            return;
        bodyRenderer.material.color =
        TeamColor == 0 ? Color.blue : Color.red;
    }

    private void Update()
    {
        if (!HasStateAuthority)
            return;
        if (Mouse.current != null &&
        Mouse.current.leftButton.wasPressedThisFrame)
        {
            firePending = true;
        }
    }
    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
            return;
        DuelMatch match = DuelMatch.Instance;
        if (match == null || !match.CanPlay)
        {
            firePending = false;
            return;
        }
        Mover();
        Mirar();
        if (firePending)
        {
            firePending = false;
            Atirar();
        }
    }
    private void Mover()
    {
        if (Keyboard.current == null)
            return;
        float x = (Keyboard.current.dKey.isPressed ? 1 : 0) - (Keyboard.current.aKey.isPressed ? 1 : 0);
        float z = (Keyboard.current.wKey.isPressed ? 1 : 0) - (Keyboard.current.sKey.isPressed ? 1 : 0);
        Vector3 direction = new Vector3(x, 0, z).normalized;
        Vector3 velocity = direction * velocidade + Vector3.down * 2f; controller.Move(velocity * Runner.DeltaTime);
    }
    private void Mirar()

    {
        if (Camera.main == null ||
        Mouse.current == null)
            return;
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane plane = new Plane(Vector3.up, transform.position);
        if (plane.Raycast(ray, out float distance))
        {
            Vector3 point = ray.GetPoint(distance);
            Vector3 direction = point - transform.position;
            direction.y = 0;
            if (direction.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }
    private void Atirar()
    {
        if (bulletPrefab == null || armaPosition == null)
            return;
        Runner.Spawn(bulletPrefab, armaPosition.position, armaPosition.rotation);
    }
}
