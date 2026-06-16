using System;
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

        private float _hintShowTimer;
        private bool _isTimerCounting;

        private void Start()
        {
            _onHoverHint.SetActive(false);
        }

        private void Update()
        {
            if (_isTimerCounting && _hintShowTimer < _hintShowDelay)
            {
                _hintShowTimer += Time.deltaTime;
                if (_hintShowTimer > _hintShowDelay)
                {
                    _onHoverHint.SetActive(true);
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
            _onHoverHint.SetActive(false);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _hintShowTimer = 0f;
            _isTimerCounting = false;
            _onHoverHint.SetActive(false);
        }
    }
}