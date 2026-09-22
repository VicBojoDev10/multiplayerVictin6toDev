using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    [Header("UI - Chat")]
    [SerializeField] private TMP_InputField messageInputField;
    [SerializeField] private Button sendButton;
    [SerializeField] private TMP_Text chatLogText;
    [SerializeField] private ScrollRect chatScrollRect; // opcional, para autoscroll

    private void Awake()
    {
        sendButton.onClick.AddListener(OnSendClicked);
    }

    private void OnSendClicked()
    {
        string message = messageInputField.text.Trim();
        if (string.IsNullOrEmpty(message)) return;

        SendChatServerRpc(message);
        messageInputField.text = "";
    }


    [ServerRpc(RequireOwnership = false)]
    private void SendChatServerRpc(string message, ServerRpcParams rpcParams = default)
    {
        ulong senderId = rpcParams.Receive.SenderClientId;
        string formatted = $"[Jugador {senderId}]: {message}";
        BroadcastChatClientRpc(formatted);
    }


    [ClientRpc]
    private void BroadcastChatClientRpc(string formattedMessage)
    {
        chatLogText.text += formattedMessage + "\n";

        if (chatScrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            chatScrollRect.verticalNormalizedPosition = 0f;
        }
    }
}
