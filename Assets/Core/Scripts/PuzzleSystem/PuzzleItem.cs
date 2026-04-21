using TMPro;
using UnityEngine;

namespace Core.Scripts.PuzzleSystem
{
    public class PuzzleItem : MonoBehaviour
    {
        [SerializeField] private bool _isActive;
        [SerializeField][Range(0, 10)] private int _orderInSequence;
        [Header("Visuals")]
        [SerializeField] private TextMeshProUGUI _puzzleText;

        public void Initialize(bool isActive, int orderInSequence, string puzzleText)
        {
            _isActive = isActive;
            _orderInSequence = orderInSequence;
            _puzzleText.text = puzzleText;
        }
    }
}
