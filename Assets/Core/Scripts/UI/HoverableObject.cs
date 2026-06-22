using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Scripts.UI
{
    public class HoverableObject : MonoBehaviour, IPointerMoveHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Objects")]
        [SerializeField] private GameObject _onHoverHint;
        [Header("Settings")]
        [SerializeField] private float _hintShowDelay = 0.5f;
        [SerializeField] private Transform _globalTransform;

        private float _hintShowTimer;
        private bool _isTimerCounting;
        private TextMeshProUGUI _hintText;

        private void Start()
        {
            HideHint();
            _hintText = _onHoverHint.GetComponentInChildren<TextMeshProUGUI>();
        }

        private void Update()
        {
            var hintRect = _hintText.transform.parent.GetComponent<RectTransform>();
            if (_hintText.isTextOverflowing)
            {
                hintRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, hintRect.sizeDelta.y + 5);
                Debug.Log("rescale");
            }

            if (_isTimerCounting && _hintShowTimer < _hintShowDelay)
            {
                _hintShowTimer += Time.deltaTime;
                if (_hintShowTimer > _hintShowDelay)
                {
                    ShowHint();
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _hintShowTimer = 0f;
            _isTimerCounting = true;
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            _hintShowTimer = 0f;
            _isTimerCounting = true;
            HideHint();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _hintShowTimer = 0f;
            _isTimerCounting = false;
            HideHint();
        }

        public void SetGlobalTransform(Transform globalTransform)
        {
            _globalTransform = globalTransform;
        }

        private void ShowHint()
        {
            // change transform's parent to avoid other puzzle's overlapping
            _onHoverHint.transform.SetParent(_globalTransform);
            _onHoverHint.SetActive(true);
        }

        private void HideHint()
        {
            _onHoverHint.transform.SetParent(transform);
            _onHoverHint.SetActive(false);
        }
    }
}