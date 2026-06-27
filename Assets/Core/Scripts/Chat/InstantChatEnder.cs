using UnityEngine;

public class InstantChatEnder : AbstractChatEnder
{
    [SerializeField] private GameState _nextGameState;

    protected override void EndChatMethod()
    {
        GameStateManager.State = _nextGameState;
        GameManager.Instance.LoadMapScene();
    }
}
