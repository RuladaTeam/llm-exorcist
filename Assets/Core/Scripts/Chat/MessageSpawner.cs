using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MessageSpawner : MonoBehaviour
{
    private class MessageRequest
    {
        public string message;
    }

    private class MessageResponse
    {
        public string message;
    }

    [SerializeField] private string serverUrl;
    [Space(30)]
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Transform _contentTransform;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private GameObject _myMessage;
    [SerializeField] private GameObject _AIMessage;

    private InputSystem_Actions _inputSystem_Actions;
    private bool _isRequestInProgress = false;

    private void OnEnable()
    {
        _inputSystem_Actions = new();
        _inputSystem_Actions.Enable();

        _inputSystem_Actions.Chat.ChatConfirm.performed += SpawnMessage;
    }

    private void OnDisable()
    {
        _inputSystem_Actions.Chat.ChatConfirm.performed -= SpawnMessage;

        _inputSystem_Actions.Disable();
    }

    public void SendMessageToServer(string text)
    {
        if (_isRequestInProgress)
        {
            Debug.LogWarning("Запрос уже выполняется!");
            return;
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            Debug.LogWarning("Текст сообщения пуст!");
            return;
        }

        StartCoroutine(SendMessageCoroutine(text));
    }

    private IEnumerator SendMessageCoroutine(string text)
    {
        _isRequestInProgress = true;
        _inputField.interactable = false;

        MessageRequest request = new() { message = text };
        string json = JsonUtility.ToJson(request);

        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(serverUrl, ""))
        {
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    TextMeshProUGUI currentMessage = Instantiate(_AIMessage, _contentTransform).transform.GetComponentInChildren<TextMeshProUGUI>();
                    MessageResponse response = JsonUtility.FromJson<MessageResponse>(www.downloadHandler.text);
                    currentMessage.text = response.message;
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error: {e.Message}");
                }
            }
            else
            {
                Debug.LogError($"Request error: {www.error}");
            }
        }

        _isRequestInProgress = false;
        _inputField.interactable = true;
    }

    private void SpawnMessage(InputAction.CallbackContext obj)
    {
        if (_inputField.text.Length > 0)
        {
            TextMeshProUGUI currentMessage = Instantiate(_myMessage, _contentTransform).transform.GetComponentInChildren<TextMeshProUGUI>();
            currentMessage.text = _inputField.text;
            _inputField.text = "";

            StartCoroutine(SendMessageCoroutine(_inputField.text));

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_contentTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_scrollRect.transform);

            _scrollRect.verticalNormalizedPosition = 0f;

        }
    }
}
