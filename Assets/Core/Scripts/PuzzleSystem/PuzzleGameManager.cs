using UnityEngine;

namespace Core.Scripts.PuzzleSystem
{
    public class PuzzleGameManager : MonoBehaviour
    {
        [Header("UI elements")]
        [SerializeField] private GameObject _puzzleCanvas;
        [SerializeField] private Transform _workspaceTransform;
        [Header("Prefabs")]
        [SerializeField] private GameObject _puzzleItemPrefab;
        [SerializeField] private GameObject _containerPrefab;

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
            var puzzle = Instantiate(_puzzleItemPrefab, _workspaceTransform).GetComponent<PuzzleItem>();
            puzzle.Initialize(isActive, orderInSequence, puzzleText);

            InstantiatePuzzleContainer(puzzle, transform.position, transform);
        }

        private GameObject InstantiatePuzzleContainer(PuzzleItem nestedPuzzle, Vector3 position, Transform parent = null)
        {
            return InstantiatePuzzleContainer(new PuzzleItem[] { nestedPuzzle }, position, parent);
        }

        public GameObject InstantiatePuzzleContainer(PuzzleItem[] nestedPuzzles, Vector3 position, Transform parent = null)
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
