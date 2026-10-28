using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Launcher : MonoBehaviour

{
    [SerializeField] private NetworkRunner runner;
    [SerializeField] private TMP_InputField salaInputField;
    [SerializeField] private Text textoStatus;
    [SerializeField] private GameObject menuPanel;
    private bool connecting;


    private void Start()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(true);
            Debug.Log("Menu de conexão ativado!");
        }
    }




    public async void ConnectToRoom()
    {
        if (connecting || runner.IsRunning)
            return;
        string roomName = salaInputField.text.Trim();
        if (string.IsNullOrEmpty(roomName))
        {
            textoStatus.text =
            "É preciso digitar o nome da sala";
            return;
        }

        connecting = true;

        textoStatus.text = "Conectando...";

        runner.ProvideInput = false;

        var sceneManager =

        GetComponent<NetworkSceneManagerDefault>();

        var result = await runner.StartGame(

        new StartGameArgs

        {
            GameMode = GameMode.Shared,
            SessionName = roomName,
            PlayerCount = 2,
            SceneManager = sceneManager
        }
        );
        if (result.Ok)
        {
            menuPanel.SetActive(false);
        }
        else
        {
            textoStatus.text =
            "Erro: " + result.ShutdownReason;
            Debug.LogError(result.ShutdownReason);
        }
        
    connecting = false;
    }
}

