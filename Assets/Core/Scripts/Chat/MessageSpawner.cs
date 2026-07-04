using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MessageSpawner : MonoBehaviour
{
    private class MessageResponse
    {
        public string text;
        public bool passed;
    }

    public class MessageRequest
    {
        public string level;
        public string text;
    }

    public static event Action OnChatEnded;

    [SerializeField] private string serverUrl = "http://localhost:8000/predict";
    [Space(30)]
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Transform _contentTransform;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private GameObject _myMessage;
    [SerializeField] private GameObject _AIMessage;
    [Space(30)]
    [SerializeField] private string _levelNameToServer;

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

        StartCoroutine(SendMessageCoroutine(text, _levelNameToServer));
    }

    private IEnumerator SendMessageCoroutine(string text, string level)
    {
        _isRequestInProgress = true;
        _inputField.interactable = false;

        // Формируем URL с параметром msg
        // UnityWebRequest.EscapeURL кодирует специальные символы для безопасной передачи
        string escapedMessage = UnityWebRequest.EscapeURL(text);
        string fullUrl = $"{serverUrl}";

        Debug.Log($"Отправка запроса: {fullUrl}");

        MessageRequest requestData = new MessageRequest();
        requestData.level = level;
        requestData.text = text;

        string jsonData = JsonUtility.ToJson(requestData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        // Используем GET запрос вместо POST
        using (UnityWebRequest www = new UnityWebRequest("http://localhost:8000/predict", "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.uploadHandler.contentType = "application/json"; // Crucial: tells the server it's JSON

            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    MessageResponse response = JsonUtility.FromJson<MessageResponse>(www.downloadHandler.text);

                    TextMeshProUGUI currentMessage = Instantiate(_AIMessage, _contentTransform).transform.GetComponentInChildren<TextMeshProUGUI>();

                    if (response.passed)
                    {
                        currentMessage.text = "Уровень пройден. Поздравляю!";
                        OnChatEnded.Invoke();

                    }
                    else
                    {
                        currentMessage.text = response.text;

                    }

                    Debug.Log($"Получен ответ: {response.text}, прошел: {response.passed}");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Ошибка парсинга ответа: {e.Message}");
                    Debug.Log($"Сырой ответ сервера: {www.downloadHandler.text}");
                }
            }
            else
            {
                Debug.LogError($"Ошибка запроса: {www.error}");
                Debug.LogError($"HTTP код: {www.responseCode}");
            }
        }

        _isRequestInProgress = false;
        _inputField.interactable = true;
        _inputField.ActivateInputField(); // Возвращаем фокус на поле ввода
    }

    private void SpawnMessage(InputAction.CallbackContext obj)
    {
        if (_inputField.text.Length > 0)
        {
            TextMeshProUGUI currentMessage = Instantiate(_myMessage, _contentTransform).transform.GetComponentInChildren<TextMeshProUGUI>();
            currentMessage.text = _inputField.text;

            SendMessageToServer(_inputField.text);

            _inputField.text = "";

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_contentTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_scrollRect.transform);

            _scrollRect.verticalNormalizedPosition = 0f;
        }
    }
}