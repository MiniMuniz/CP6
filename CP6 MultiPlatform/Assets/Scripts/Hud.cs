using Fusion;
using TMPro;
using UnityEngine;

public class Hud : MonoBehaviour
{
    [SerializeField] private NetworkRunner runner;
    [Header("HUD")]
    [SerializeField] private TMP_Text textoVida;
    [SerializeField] private TMP_Text partidaStatus;
    [Header("Resultado")]
    [SerializeField] private GameObject resultsPanel;
    [SerializeField] private TMP_Text resultText;
    private void Start()
    {
        resultsPanel.SetActive(false);
    }
    private void Update()
    {
        if (runner == null || !runner.IsRunning)
            return;
        Partida match = Partida.Instance;
        if (match == null)
        {
            partidaStatus.text = "Aguardando estado da partida...";
            return;
        }
        UpdateHealth();
        if (match.IsOver)
        {
            ShowResult(match);
        }
        else
        {
            resultsPanel.SetActive(false);
            partidaStatus.text = match.CanPlay? "Duelo em andamento": "Aguardando segundo jogador...";
        }
    }
    private void UpdateHealth()
    {
        if (runner.TryGetPlayerObject(runner.LocalPlayer,out NetworkObject playerObject))
        {
            Health health = playerObject.GetComponent<Health>();
            if (health != null)
            {
                textoVida.text = "Vida: " + health.Vida + " / 100";
            }
        }
    }
    private void ShowResult(Partida match)
    {
        resultsPanel.SetActive(true);
        partidaStatus.text = "Partida encerrada";
        bool localPlayerWon = match.Winner == runner.LocalPlayer;
        string winnerName = match.Winner == match.BluePlayer? "AZUL": "VERMELHO";
        string message = localPlayerWon? "VOCÊ VENCEU!": "VOCÊ PERDEU!";
        resultText.text = message + "\nVencedor: Jogador " + winnerName;
    }
}
