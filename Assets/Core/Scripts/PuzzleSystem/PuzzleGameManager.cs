using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Core.Scripts.PuzzleSystem
{
    public class PuzzleGameManager : MonoBehaviour
    {
        // subscribe to this event to get notified when the puzzle is solved
        public static event Action OnPuzzleSolved;

        [SerializeField] private int _totalActivePuzzles;
        [SerializeField] private GameObject _puzzleCanvas;
        [SerializeField] private Transform _spawnerSpaceTransform;
        [SerializeField] private GameObject _containerPrefab;
        [SerializeField] private GameObject _spawnerPrefab;

        private RectTransform _workspaceTransform;
        private RectTransform _spawnersSpaceTransform;

        private void Start()
        {
            _workspaceTransform = GameObject.FindGameObjectWithTag("CodeSpace")
                .GetComponent<RectTransform>();
            _spawnersSpaceTransform = GameObject.FindGameObjectWithTag("BlocksSpace")
                .GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            //SomeEventSystem.OnPuzzleStart += StartPuzzle;
            //SomeEventSystem.OnPuzzleEnd += EndPuzzle;
            //SomeEventSystem.OnCreatePuzzle += CreatePuzzle;
        }

        private void OnDisable()
        {
            //SomeEventSystem.OnPuzzleStart -= StartPuzzle;
            //SomeEventSystem.OnPuzzleEnd -= EndPuzzle;
            //SomeEventSystem.OnCreatePuzzle -= CreatePuzzle;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                StartPuzzle();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                EndPuzzle();
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                CreatePuzzle(true, "Test Puzzle", 0);
            }
        }

        private void StartPuzzle()
        {
            _puzzleCanvas.SetActive(true);
        }

        private void EndPuzzle()
        {
            _puzzleCanvas.SetActive(false);
        }

        private void CreatePuzzle(bool isActive, string puzzleText, int orderInSequence = -1)
        {
            var spawner = Instantiate(_spawnerPrefab, _spawnerSpaceTransform).GetComponent<PuzzleSpawner>();

            spawner.CreatePuzzle(isActive, puzzleText, orderInSequence);
        }

        private void CalculateResult(PuzzleContainer container)
        {
            var puzzles = container.GetNestedPuzzlesArray();

            int currentSequenceIndex = 0;
            foreach (var puzzle in puzzles)
            {
                if (!puzzle.IsActive)
                    continue;

                if (puzzle.OrderInSequence != currentSequenceIndex)
                {
                    Debug.Log("Puzzles are not in the right order");
                    return;
                }
                currentSequenceIndex++;
            }

            if (currentSequenceIndex != _totalActivePuzzles)
            {
                Debug.Log("Not all active puzzles are connected (or too many if you have a mistake in code)");
                return;
            }

            Debug.Log("Puzzle is solved!");
            OnPuzzleSolved?.Invoke();
        }

        public bool IsOnWorkspace(Vector2 point)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(_workspaceTransform, point)
                && !RectTransformUtility.RectangleContainsScreenPoint(_spawnersSpaceTransform, point);
        }

        public void MoveToPosition(Transform targetTransform, Vector2 position, Action onComplete, Transform parent = null)
        {
            var originalParent = targetTransform.parent;

            targetTransform.SetParent(_puzzleCanvas.transform);
            targetTransform.DOMove(position, .3f)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    if (parent != null)
                        targetTransform.SetParent(parent);
                    else
                        targetTransform.SetParent(originalParent);

                    onComplete?.Invoke();
                });
        }

        public void SetToRoot(Transform targetTransform)
        {
            targetTransform.SetParent(_puzzleCanvas.transform);
        }

        public void SetToWorkspace(Transform targetTransform)
        {
            targetTransform.SetParent(_workspaceTransform.transform);
        }

        public void MergeContainers(PuzzleContainer fromContainer, PuzzleContainer toContainer, int startIndex = -1)
        {
            var puzzles = fromContainer.GetNestedPuzzlesArray();

            if (startIndex == -1)
                toContainer.PushPuzzle(puzzles);
            else
                toContainer.PushPuzzle(puzzles, startIndex);

            Destroy(fromContainer.gameObject);

            CalculateResult(toContainer);
        }

        public GameObject InstantiateContainer(PuzzleItem[] nestedPuzzles, Vector3 position, Transform parent = null)
        {
            // by default is being instantiated as a workspaceTransform's child
            parent = parent == null ? _puzzleCanvas.transform : parent;

            var container = Instantiate(_containerPrefab, position, Quaternion.identity, parent)
                .GetComponent<PuzzleContainer>();
            container.PushPuzzle(nestedPuzzles);

            return container.gameObject;
        }
    }
}
