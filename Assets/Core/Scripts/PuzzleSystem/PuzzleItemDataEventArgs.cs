using System;

namespace Assets.Core.Scripts.PuzzleSystem
{
    [Serializable]
    public class PuzzleItemDataEventArgs : EventArgs
    {
        public readonly bool isActive;
        public readonly int orderInSequence;
        public readonly string puzzleText;

        public PuzzleItemDataEventArgs(bool isActive, int orderInSequence, string puzzleText)
        {
            this.isActive = isActive;
            this.orderInSequence = orderInSequence;
            this.puzzleText = puzzleText;
        }
    }
}
