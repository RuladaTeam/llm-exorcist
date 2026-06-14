using Core.Scripts.PuzzleSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Core.Scripts.PuzzleSystem
{
    public class PuzzleSpawnerBuilder
    {
        public PuzzleSpawnerBuilder(PuzzleGameManager puzzleGameManager, PuzzleSpawner puzzleSpawner)
        {
            puzzleSpawner.PuzzleGameManager = puzzleGameManager;
        }
    }
}
