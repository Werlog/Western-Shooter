using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Riptide;

public class UIManager : MonoBehaviour
{
    private static UIManager _singleton;

    public static UIManager Singleton
    {
        get => _singleton;

        set
        {
            if (_singleton == null)
            {
                _singleton = value;
            }
            else
            {
                Debug.LogWarning($"{nameof(UIManager)}: Singleton already exists, destroying duplicate");
                Destroy(value);
            }
        }
    }

    [SerializeField] private GameObject uiCamera;

    [Header("Connect UI")]
    [SerializeField] private GameObject connectUI;
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField ipField;

    [Header("HUD")]
    [SerializeField] private Animator healthDisplayAnimator;
    [SerializeField] private TextMeshProUGUI healthText;

    private void Awake()
    {
        Singleton = this;
    }

    public void ConnectClicked()
    {
        if (string.IsNullOrEmpty(usernameField.text)) return;
        if (string.IsNullOrEmpty(ipField.text) || ipField.text == "0" || ipField.text == "localhost")
        {
            ipField.text = "127.0.0.1";
        }
        if (!CheckIpAddressFormat(ipField.text)) return;

        NetworkManager.Singleton.Connect(ipField.text, 2589);

        connectUI.SetActive(false);
    }

    public void BackToConnectScreen()
    {
        Cursor.lockState = CursorLockMode.None;
        uiCamera.SetActive(true);
        connectUI.SetActive(true);
    }

    public void DisableConnectScreen()
    {
        uiCamera.SetActive(false);
        connectUI.SetActive(false);
    }

    private bool CheckIpAddressFormat(string addressString)
    {
        string[] segments = addressString.Split(".");
        if (segments.Length != 4) return false;
        foreach (string segment in segments)
        {
            if (segment.StartsWith('-') || segment.StartsWith('+')) return false;
            if (int.TryParse(segment, out int value))
            {
                if (value > 255) return false;
            }
            else return false;
        }

        return true;
    }

    public void SendUsername()
    {
        Message message = Message.Create(MessageSendMode.Reliable, ClientToServer.username);
        message.AddString(usernameField.text);
        NetworkManager.Singleton.Client.Send(message);
    }

    public void SetHealthText(int health)
    {
        healthText.text = health.ToString();

        if (health < Player.PlayerMaxHealth * 0.25 && !healthDisplayAnimator.GetCurrentAnimatorStateInfo(0).IsName("LowHealth"))
        {
            healthDisplayAnimator.Play("LowHealth");
        } else if (health >= Player.PlayerMaxHealth * 0.25)
        {
            healthDisplayAnimator.Play("NormalHealth");
        }
    }
}
