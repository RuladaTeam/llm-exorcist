using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Core.Scripts.PuzzleSystem
{
    public class PuzzleGameManager : MonoBehaviour
    {
        [SerializeField] private PuzzleSpawner[] _puzzleSpawners;
        [SerializeField] private GameObject _puzzleCanvas;
        [SerializeField] private GameObject _containerPrefab;

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
            //todo: clear spawners if they have puzzles 
            foreach (var spawner in _puzzleSpawners)
            {
                if (spawner.SpawnedPuzzle == null)
                {
                    spawner.CreatePuzzle(isActive, puzzleText, orderInSequence);
                    return;
                }
            }
        }

        public bool IsOnWorkspace(Vector2 point)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(_workspaceTransform, point)
                && !RectTransformUtility.RectangleContainsScreenPoint(_spawnersSpaceTransform, point);
        }

        public void MoveToPosition(Transform targetTransform, Vector2 position, Action onComplete, Transform parent = null)
        {
            var originalParent = targetTransform.parent;

            targetTransform.SetParent(transform.GetChild(0));
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
            targetTransform.SetParent(transform.GetChild(0));
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
        }

        public GameObject InstantiateContainer(PuzzleItem[] nestedPuzzles, Vector3 position, Transform parent = null)
        {
            // by default is being instantiated as a workspaceTransform's child
            parent = parent == null ? _workspaceTransform.transform : parent;

            var container = Instantiate(_containerPrefab, position, Quaternion.identity, parent)
                .GetComponent<PuzzleContainer>();
            container.PushPuzzle(nestedPuzzles);

            return container.gameObject;
        }
    }
}
