using UnityEngine;

namespace Core.Scripts.PuzzleSystem
{
    public class PuzzleGameManager : MonoBehaviour
    {
        [SerializeField] private GameObject _puzzleCanvas;
        [SerializeField] private PuzzleItem _puzzleItemPrefab;
        [SerializeField] private Transform _workspaceTransform;

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
            Instantiate(_puzzleItemPrefab, _workspaceTransform).Initialize(isActive, orderInSequence, puzzleText);
        }
    }
}
