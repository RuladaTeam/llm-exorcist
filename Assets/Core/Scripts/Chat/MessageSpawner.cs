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
        public string response;
    }

    [SerializeField] private string serverUrl = "http://192.168.0.228/exorcist/api/chat";
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

        // Формируем URL с параметром msg
        // UnityWebRequest.EscapeURL кодирует специальные символы для безопасной передачи
        string escapedMessage = UnityWebRequest.EscapeURL(text);
        string fullUrl = $"{serverUrl}?msg={escapedMessage}";

        Debug.Log($"Отправка запроса: {fullUrl}");

        // Используем GET запрос вместо POST
        using (UnityWebRequest www = UnityWebRequest.Get(fullUrl))
        {
            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    MessageResponse response = JsonUtility.FromJson<MessageResponse>(www.downloadHandler.text);

                    TextMeshProUGUI currentMessage = Instantiate(_AIMessage, _contentTransform).transform.GetComponentInChildren<TextMeshProUGUI>();
                    currentMessage.text = response.response;

                    Debug.Log($"Получен ответ: {response.response}");
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

            StartCoroutine(SendMessageCoroutine(_inputField.text));

            _inputField.text = "";

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_contentTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_scrollRect.transform);

            _scrollRect.verticalNormalizedPosition = 0f;
        }
    }
}