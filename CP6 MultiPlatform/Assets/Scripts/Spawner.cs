using Fusion;
using UnityEngine;

public class Spawner : SimulationBehaviour, IPlayerJoined
{
    [SerializeField] private NetworkObject playerPrefab;
    [SerializeField] private NetworkObject matchPrefab;

    [Header("Alvos")]
    [SerializeField] private NetworkObject[] alvosPrefabs;
    [SerializeField] private Transform[] pontosSpawnAlvos;

    public void PlayerJoined(PlayerRef player)
    {
        if (player != Runner.LocalPlayer)
            return;

        bool isMaster = Runner.IsSharedModeMasterClient;

        if (isMaster)
        {
            Runner.Spawn(
                matchPrefab,
                Vector3.zero,
                Quaternion.identity
            );

            SpawnarAlvos();
        }

        Vector3 spawnPosition = isMaster
            ? new Vector3(-4, 1, 0)
            : new Vector3(4, 1, 0);

        Quaternion spawnRotation = isMaster
            ? Quaternion.Euler(0, 90, 0)
            : Quaternion.Euler(0, -90, 0);

        NetworkObject playerObject = Runner.Spawn(
            playerPrefab,
            spawnPosition,
            spawnRotation
        );

        Runner.SetPlayerObject(player, playerObject);
    }

    private void SpawnarAlvos()
    {
        if (alvosPrefabs == null || pontosSpawnAlvos == null)
            return;

        int quantidade = Mathf.Min(
            alvosPrefabs.Length,
            pontosSpawnAlvos.Length
        );

        for (int i = 0; i < quantidade; i++)
        {
            if (alvosPrefabs[i] == null || pontosSpawnAlvos[i] == null)
                continue;

            Runner.Spawn(
                alvosPrefabs[i],
                pontosSpawnAlvos[i].position,
                pontosSpawnAlvos[i].rotation
            );
        }
    }
}