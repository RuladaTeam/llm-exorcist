using Assets.Core.Scripts.PuzzleSystem;
using UnityEngine;

namespace Core.Scripts.PuzzleSystem
{
    public class PuzzleSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _puzzleItemPrefab;

        public PuzzleItem SpawnedPuzzle { get; private set; }

        public PuzzleGameManager PuzzleGameManager { private get; set; }

        //private void Start()
        //{
        //    // looooool i hate myself
        //    PuzzleGameManager = transform.parent.parent.parent.parent.parent.GetComponent<PuzzleGameManager>();
        //}

        public void CreatePuzzle(PuzzleItemDataStruct puzzleItemDataStruct)
        {
            if (SpawnedPuzzle != null)
            {
                Debug.LogError("There is already a puzzle in this spawner");
                return;
            }

            var puzzle = Instantiate(_puzzleItemPrefab, transform).GetComponent<PuzzleItem>();
            puzzle.Initialize(puzzleItemDataStruct.isActive, puzzleItemDataStruct.orderInSequence, puzzleItemDataStruct.puzzleText);
            puzzle.SetSpawner(this);
            InstantiatePuzzleContainer(puzzle, transform.position, transform);

            SpawnedPuzzle = puzzle;
        }

        private GameObject InstantiatePuzzleContainer(PuzzleItem nestedPuzzle, Vector3 position, Transform parent = null)
        {
            return PuzzleGameManager.InstantiateContainer(new PuzzleItem[] { nestedPuzzle }, position, parent);
        }

        public PuzzleGameManager GetPuzzleGameManager()
        {
            return PuzzleGameManager;
        }
    }
}