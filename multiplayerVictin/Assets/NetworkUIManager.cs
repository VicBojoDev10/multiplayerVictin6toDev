using UnityEngine;
using System.Net; //Este trae los DNS para consultar direccion ip de la computadora
using System.Net.Sockets;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP; //UDP User Datagram Protocol
using TMPro;
using UnityEngine.UI;
public class NetworkUIManager : MonoBehaviour
{
    [Header("UIConnection")]
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private TMP_Text ipText;
    [SerializeField] private TMP_Text statusText;
    [Header("Settings")]
    [SerializeField] private ushort port = 7777;
    [SerializeField] private UnityTransport transport;
    
    void Start()
    {
        if(NetworkManager.Singleton == null)
        {
            Debug.LogError("Network Manager doesn't have an assigned Unity Transport");
            return;
        }
        if(transport == null)
        {
            Debug.LogError("The GO of NM doesn't have a UTP assigned");
            return;
        }
        hostButton.onClick.AddListener(OnHostClicked);
        hostButton.onClick.AddListener(OnJoinClicked);
        ipText.text = $"Your IP: {GetLocalIPAdress()}";
    }
    private string GetLocalIPAdress()
    {
        foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
            if(ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return "No detectada";
    }
    private void OnHostClicked()
    {
        transport.SetConnectionData("0.0.0.0", port);
        bool started= NetworkManager.Singleton.StartHost();
        statusText.text = started ? $"Host initiated. share the IP: {GetLocalIPAdress()}": "Host could not be initiated";
        SetConnectionUIInteractable(false);
    }
    private void OnJoinClicked()
    {
        string hostIp = ipInputField.text.Trim();
        if (string.IsNullOrEmpty(hostIp))
        {
            statusText.text = "Escribe la IP del host antes de conectarte.";
            return;
        }

        transport.SetConnectionData(hostIp, port);

        bool started = NetworkManager.Singleton.StartClient();
        statusText.text = started
            ? $"Conectando a {hostIp}..."
            : "No se pudo iniciar el cliente.";

        SetConnectionUIInteractable(false);
    }
    private void SetConnectionUIInteractable(bool interactable)
    {
        hostButton.interactable = interactable;
        joinButton.interactable = interactable;
        ipInputField.interactable = interactable;
    }
}
