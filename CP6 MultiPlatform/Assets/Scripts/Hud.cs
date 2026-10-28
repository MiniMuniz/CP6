using Fusion;
using TMPro;
using UnityEngine;

public class Hud : MonoBehaviour
{
    [Header("Photon Fusion")]
    [SerializeField] private NetworkRunner runner;

    [Header("Panels")]
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject resultsPanel;

    [Header("HUD da Partida")]
    [SerializeField] private TMP_Text pontosAzul;
    [SerializeField] private TMP_Text pontosVermelho;
    [SerializeField] private TMP_Text textoTempo;
    [SerializeField] private TMP_Text partidaStatus;

    [Header("Resultado")]
    [SerializeField] private TMP_Text resultText;

    private void Start()
    {
        if (gamePanel != null)
            gamePanel.SetActive(false);

        if (resultsPanel != null)
            resultsPanel.SetActive(false);
    }

    private void Update()
    {
        if (runner == null || !runner.IsRunning)
            return;

        Partida match = Partida.Instance;

        if (match == null)
        {
            if (partidaStatus != null)
                partidaStatus.text =
                    "Aguardando estado da partida...";
            return;
        }

        AtualizarPontuacao(match);
        AtualizarTempo(match);

        if (match.IsOver)
        {
            MostrarResultado(match);
        }
        else
        {
            if (resultsPanel != null)
                resultsPanel.SetActive(false);

            if (gamePanel != null)
                gamePanel.SetActive(true);

            if (partidaStatus != null)
            {
                partidaStatus.text = match.CanPlay
                    ? "Tiro ao alvo em andamento"
                    : "Aguardando segundo jogador...";
            }
        }
    }

    private void AtualizarPontuacao(Partida match)
    {
        if (pontosAzul != null)
            pontosAzul.text = "Azul: " + match.PontosAzul;

        if (pontosVermelho != null)
            pontosVermelho.text =
                "Vermelho: " + match.PontosVermelho;
    }

    private void AtualizarTempo(Partida match)
    {
        if (textoTempo == null)
            return;

        float segundos = 60f;

        if (match.Iniciada)
        {
            segundos = match.TempoPartida
                .RemainingTime(runner) ?? 0f;
        }

        segundos = Mathf.Max(0, segundos);

        int total = Mathf.CeilToInt(segundos);
        int minutos = total / 60;
        int segundosRestantes = total % 60;

        textoTempo.text =
            $"{minutos:00}:{segundosRestantes:00}";
    }

    private void MostrarResultado(Partida match)
    {
        if (gamePanel != null)
            gamePanel.SetActive(false);

        if (resultsPanel != null)
            resultsPanel.SetActive(true);

        if (partidaStatus != null)
            partidaStatus.text = "Partida encerrada";

        if (resultText == null)
            return;

        if (match.Winner == PlayerRef.None)
        {
            resultText.text =
                "EMPATE!\n" +
                "Azul: " + match.PontosAzul +
                "\nVermelho: " + match.PontosVermelho;
            return;
        }

        bool venceu =
            match.Winner == runner.LocalPlayer;

        string vencedor =
            match.Winner == match.BluePlayer
                ? "AZUL"
                : "VERMELHO";

        resultText.text =
            (venceu ? "VOCÊ VENCEU!" : "VOCÊ PERDEU!") +
            "\nVencedor: Jogador " + vencedor +
            "\n\nAzul: " + match.PontosAzul +
            "\nVermelho: " + match.PontosVermelho;
    }
}