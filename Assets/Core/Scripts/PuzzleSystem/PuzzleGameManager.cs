using System;
using Assets.Core.Scripts.PuzzleSystem;
using DG.Tweening;
using UnityEngine;

namespace Core.Scripts.PuzzleSystem
{
    public class PuzzleGameManager : MonoBehaviour
    {
        [SerializeField] private int _totalActivePuzzles;
        [SerializeField] private GameObject _puzzleCanvas;
        [SerializeField] private Transform _spawnerSpaceTransform;
        [SerializeField] private GameObject _containerPrefab;
        [SerializeField] private GameObject _spawnerPrefab;
        [SerializeField] private string[] _puzzleOrders;
        [Space(10)]
        [SerializeField] private PuzzleDoor _puzzleDoor;

        private RectTransform _workspaceTransform;
        private RectTransform _spawnersSpaceTransform;

        private void Start()
        {
            _workspaceTransform = GameObject.FindGameObjectWithTag("CodeSpace")
                .GetComponent<RectTransform>();
            _spawnersSpaceTransform = GameObject.FindGameObjectWithTag("BlocksSpace")
                .GetComponent<RectTransform>();

            EndPuzzle();

        }

        private void OnEnable()
        {
            _puzzleDoor.OnPuzzleStart += StartPuzzle;
            _puzzleDoor.OnPuzzleEnd += EndPuzzle;
            DialogueViewer.OnDialogueEnded += CreatePuzzle;
        }

        private void OnDisable()
        {
            _puzzleDoor.OnPuzzleStart -= StartPuzzle;
            _puzzleDoor.OnPuzzleEnd -= EndPuzzle;
            DialogueViewer.OnDialogueEnded -= CreatePuzzle;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                CreatePuzzle(new PuzzleItemDataStruct()
                {
                    puzzleText = "на наху =)",
                    puzzleInfo = "Ну что ты фраер сдал назад не по масти я тебе епта бля =)"
                });
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

        private void CreatePuzzle(PuzzleItemDataStruct puzzleItemDataStruct)
        {
            var spawner = Instantiate(_spawnerPrefab, _spawnerSpaceTransform).GetComponent<PuzzleSpawner>();
            new PuzzleSpawnerBuilder(this, spawner);

            spawner.CreatePuzzle(puzzleItemDataStruct);
        }

        private void CalculateResult(PuzzleContainer container)
        {
            var puzzles = container.GetNestedPuzzlesArray();

            foreach (string order in _puzzleOrders)
            {
                if (puzzles.Length != order.Length)
                {
                    continue;
                }

                bool correct = true;
                for (int i = 0; i < order.Length; i++)
                {
                    if (puzzles[i].OrderInSequence != order[i] || puzzles[i].IsActive == false)
                    {
                        correct = false;
                        break;
                    }
                }

                if (correct)
                {
                    Debug.Log("Puzzle is solved!");
                    _puzzleDoor.OpenDoor();
                    return;
                }
            }

            //foreach (var puzzle in puzzles)
            //{
            //    if (!puzzle.IsActive)
            //        continue;

            //    if (puzzle.OrderInSequence != currentSequenceIndex)
            //    {
            //        correct = false;
            //        break;
            //    }
            //    currentSequenceIndex++;
            //}

            //if (currentSequenceIndex != _totalActivePuzzles)
            //{
            //    Debug.Log("Not all active puzzles are connected (or too many if you have a mistake in code)");
            //    return;
            //}
            //if (!correct)
            //{
            //    Debug.Log("Puzzles are not in the right order");
            //    return;
            //}

            //OnPuzzleSolved?.Invoke();
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
