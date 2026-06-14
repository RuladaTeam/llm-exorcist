using System;

namespace Assets.Core.Scripts.PuzzleSystem
{
    [Serializable]
    public struct PuzzleItemDataStruct
    {
        public bool isActive;
        public int orderInSequence;
        public string puzzleText;
    }
}
