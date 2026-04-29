using UnityEngine;

namespace Core.Scripts.PuzzleSystem
{
    public class PuzzleSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _puzzleItemPrefab;

        public PuzzleItem SpawnedPuzzle { get; private set; }

        private PuzzleGameManager _puzzleGameManager;

        private void Start()
        {
            // looooool i hate myself
            _puzzleGameManager = transform.parent.parent.parent.parent.parent.GetComponent<PuzzleGameManager>();
        }

        public void CreatePuzzle(bool isActive, string puzzleText, int orderInSequence = -1)
        {
            if (SpawnedPuzzle != null)
            {
                Debug.LogError("There is already a puzzle in this spawner");
                return;
            }


            var puzzle = Instantiate(_puzzleItemPrefab, transform).GetComponent<PuzzleItem>();
            puzzle.Initialize(isActive, orderInSequence, puzzleText);
            puzzle.SetSpawner(this);
            InstantiatePuzzleContainer(puzzle, transform.position, transform);

            SpawnedPuzzle = puzzle;
        }

        private GameObject InstantiatePuzzleContainer(PuzzleItem nestedPuzzle, Vector3 position, Transform parent = null)
        {
            return _puzzleGameManager.InstantiateContainer(new PuzzleItem[] { nestedPuzzle }, position, parent);
        }

        public PuzzleGameManager GetPuzzleGameManager()
        {
            return _puzzleGameManager;
        }
    }
}