using UnityEngine;

namespace Assets.Core.Scripts.Chat
{
    public class ChatInteractor : MonoBehaviour
    {
        [SerializeField] private MessageSpawner _messageSpawner;

        private void Start()
        {
            _messageSpawner.gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            _messageSpawner.gameObject.SetActive(true);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            _messageSpawner.gameObject.SetActive(false);
        }
    }
}
