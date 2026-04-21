using System.Collections.Generic;
using UnityEngine;

namespace Core.Scripts.PuzzleSystem
{
    public class PuzzleContainer : MonoBehaviour
    {
        [SerializeField] private List<PuzzleItem> _nestedPuzzles = new();
        [SerializeField] private GameObject _freeSpaceItemPrefab;

        private GameObject _freeSpaceItem;

        public void PushPuzzle(PuzzleItem[] puzzles, int startIndex)
        {
            foreach (var puzzle in puzzles)
            {
                PushPuzzle(puzzle, startIndex++);
            }
        }

        public void PushPuzzle(PuzzleItem[] puzzles)
        {
            foreach (var puzzle in puzzles)
            {
                PushPuzzle(puzzle);
            }
        }

        public void PushPuzzle(PuzzleItem puzzle, int startIndex)
        {
            if (_nestedPuzzles.Contains(puzzle))
                return;

            puzzle.SetContainer(this);
            puzzle.transform.SetParent(transform);

            puzzle.transform.SetSiblingIndex(startIndex);
            _nestedPuzzles.Insert(startIndex, puzzle);
        }

        public void PushPuzzle(PuzzleItem puzzle)
        {
            if (_nestedPuzzles.Contains(puzzle))
                return;

            puzzle.SetContainer(this);
            puzzle.transform.SetParent(transform);
            _nestedPuzzles.Add(puzzle);
        }
    }
}