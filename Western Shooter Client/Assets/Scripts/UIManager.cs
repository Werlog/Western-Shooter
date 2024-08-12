using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Riptide;
using UnityEngine.UI;

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
    [SerializeField] private Image hurtOverlay;
    [SerializeField] private GameObject tabList;
    [SerializeField] private GameObject playerList;
    [SerializeField] private GameObject playerListItemPrefab;
    [SerializeField] private GameObject killEffectPrefab;

    [Header("Chat")]
    [SerializeField] private GameObject chatObject;
    [SerializeField] private CanvasGroup chatGroup;
    [SerializeField] private TMP_InputField chatInputField;

    private void Awake()
    {
        Singleton = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Semicolon) && NetworkManager.Singleton.Client.IsConnected && !chatInputField.isFocused)
        {
            ToggleChat(!chatObject.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            tabList.SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            tabList.SetActive(false);
        }

        hurtOverlay.color = new Color(hurtOverlay.color.r, hurtOverlay.color.g, hurtOverlay.color.b, Mathf.Lerp(hurtOverlay.color.a, 0f, Time.deltaTime * 2f));
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

    public void PlayHurtAnimation(float intensity)
    {
        hurtOverlay.color = new Color(hurtOverlay.color.r, hurtOverlay.color.g, hurtOverlay.color.b, hurtOverlay.color.a + intensity);
    }

    IEnumerator ChatAnimation(float duration, bool opening)
    {
        if (opening)
        {
            chatObject.SetActive(true);
            chatInputField.Select();
            chatInputField.ActivateInputField();
        }

        float timeElapsed = 0f;
        float startAlpha = chatGroup.alpha;

        while (timeElapsed < duration)
        {
            chatGroup.alpha = Mathf.Lerp(startAlpha, opening ? 1f : 0f, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        if (!opening)
            chatObject.SetActive(false);
    }

    public void ToggleChat(bool state)
    {
        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
        StopAllCoroutines();
        StartCoroutine(ChatAnimation(0.3f, state));
    }

    public void OnChatFieldEndEdit()
    {
        SendChatMessage(chatInputField.text);
        chatInputField.text = "";
        ToggleChat(false);
    }

    public void SendChatMessage(string chatMessage)
    {
        Message message = Message.Create(MessageSendMode.Reliable, ClientToServer.chatMessage);
        message.AddString(chatMessage);
        NetworkManager.Singleton.Client.Send(message);
    }

    public void UpdatePlayerList()
    {
        foreach (Transform child in playerList.transform)
        {
            Destroy(child.gameObject);
        }

        List<Player> players = new List<Player>();
        players.AddRange(GameManager.Singleton.players.Values);

        players.Sort((p1, p2) => ComparePlayerScores(p1, p2));

        RectTransform playerListRect = playerList.GetComponent<RectTransform>();

        for (int i = 0; i < players.Count; i++)
        {
            float ypos = -(30 + i * 60);
            Player player = players[i];

            GameObject listItem = Instantiate(playerListItemPrefab, transform.position, Quaternion.identity);
            listItem.transform.SetParent(playerList.transform);



            RectTransform rect = listItem.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(0, ypos);
            rect.sizeDelta = new Vector2(playerListRect.sizeDelta.x, rect.sizeDelta.y);


            PlayerListItem playerListItem = listItem.GetComponent<PlayerListItem>();

            playerListItem.nameText.text = player.Username;
            playerListItem.scoreText.text = player.Score.ToString();
        }
    }

    public void ShowKillEffect()
    {
        Transform uiObject = Instantiate(killEffectPrefab, transform.position, Quaternion.identity).transform;
        uiObject.SetParent(transform);
    }

    private int ComparePlayerScores(Player a, Player b)
    {
        if (a.Score > b.Score) return -1;
        else if (a.Score < b.Score) return 1;
        else return 0;
    }
}
