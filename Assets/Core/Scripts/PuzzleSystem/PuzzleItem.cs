using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Scripts.PuzzleSystem
{
    public class PuzzleItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public static event Action<PuzzleItem> OnPuzzleDrag;
        public static event Action<PuzzleItem> OnPuzzleDrop;

        [field: SerializeField] public bool IsActive { get; private set; }
        [field: SerializeField] public int OrderInSequence { get; private set; }
        [field: SerializeField] public PuzzleContainer ParentContainer { get; private set; }
        [SerializeField] private RectTransform _connectionAreaRect;

        [Header("Visuals")]
        [SerializeField] private TextMeshProUGUI _puzzleText;

        private PuzzleItem _overlappingPuzzle; // currently static puzzle that is being overlapped by this puzzle while in drag
        private RectTransform _rectTransform;
        private PuzzleSpawner _spawner;
        private bool _isDraggedNow;

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            OnPuzzleDrag += Puzzle_OnPuzzleDrag;
        }

        private void OnDisable()
        {
            OnPuzzleDrag -= Puzzle_OnPuzzleDrag;
        }

        public void Initialize(bool isActive, int orderInSequence, string puzzleText)
        {
            IsActive = isActive;
            OrderInSequence = orderInSequence;
            _puzzleText.text = puzzleText;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (ParentContainer == null)
            {
                Debug.LogError("parent is null watafuk");
                Destroy(gameObject);
                return;
            }

            // if (!_wasDragged)
            // {
            //     CanBeConnectedTo = true;
            //     _wasDragged = true;
            //     OnFirstDrag?.Invoke();
            // }

            ParentContainer.HandleBeginDrag(this);

            _isDraggedNow = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDraggedNow) return;

            if (ParentContainer == null)
            {
                Debug.LogError("parent is null watafuk");
                Destroy(gameObject);
                return;
            }

            ParentContainer.HandleDrag(this);

            // if (_positionInSequence != SequencePosition.First)
            //     OnNodeDrag?.Invoke(this);

            OnPuzzleDrag?.Invoke(this);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnPuzzleDrop?.Invoke(this);

            if (ParentContainer == null)
            {
                Debug.LogError("parent is null watafuk");
                Destroy(gameObject);
                return;
            }

            ParentContainer.HandleEndDrag(this, _overlappingPuzzle);

            _isDraggedNow = false;
        }

        private void Puzzle_OnPuzzleDrag(PuzzleItem draggingPuzzle)
        {
            if (draggingPuzzle == this) return;

            // todo: check what did i mean here (now does not work properly for loops)

            if (RectTransformUtility.RectangleContainsScreenPoint(_connectionAreaRect, draggingPuzzle.GetTopPoint()))
            {
                ParentContainer.OnPuzzleOverlap(this, draggingPuzzle);
                return;
            }

            ParentContainer.OnPuzzleOverlapStopped(this, draggingPuzzle);
        }


        public void SetContainer(PuzzleContainer container)
        {
            ParentContainer = container;
        }

        public bool TrySetOverlappingPuzzle(PuzzleItem overlappingPuzzle)
        {
            if (_overlappingPuzzle == null)
            {
                _overlappingPuzzle = overlappingPuzzle;
                return true;
            }
            return false;
        }

        public bool TryRemoveOverlappingPuzzle(PuzzleItem overlappingPuzzle)
        {
            if (_overlappingPuzzle == overlappingPuzzle)
            {
                _overlappingPuzzle = null;
                return true;
            }
            return false;
        }

        public void CheckLocation()
        {
            if (!_spawner.GetPuzzleGameManager().IsOnWorkspace(transform.position))
            {
                var targetTransform = ParentContainer.transform;
                _spawner.GetPuzzleGameManager().MoveToPosition(targetTransform, _spawner.transform.position, () =>
                {
                    // if (_destroyOnReturnToInitialPosition)
                    // {
                    //     DestroyNode();
                    // }
                    // else
                    // {
                    //     //i set this only for start node just not to leave the screen bounds (shitty solution)
                    //     targetTransform.localPosition = Vector3.zero;
                    //     _wasDragged = false;

                    //     OnNonDestroyableNodeReturnedToSpawner?.Invoke();
                    // }
                }, _spawner.transform);
            }
        }

        public bool HasOverlappingPuzzle() => _overlappingPuzzle != null;

        public Vector2 GetTopPoint()
        {
            Vector3[] corners = new Vector3[4];
            _rectTransform.GetWorldCorners(corners);

            Vector2 topLeft = corners[1];
            Vector2 topRight = corners[2];

            return (topLeft + topRight) / 2f;
        }

        public Vector2 GetBottomPoint()
        {
            Vector3[] corners = new Vector3[4];
            _rectTransform.GetWorldCorners(corners);

            Vector2 bottomLeft = corners[0];
            Vector2 bottomRight = corners[3];

            return (bottomLeft + bottomRight) / 2f;
        }

        public void SetSpawner(PuzzleSpawner spawner)
        {
            _spawner = spawner;
        }

        public PuzzleGameManager GetPuzzleGameManager()
        {
            return _spawner.GetPuzzleGameManager();
        }
    }
}
