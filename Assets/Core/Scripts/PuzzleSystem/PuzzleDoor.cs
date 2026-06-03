using System;
using UnityEngine;

public class PuzzleDoor : MonoBehaviour
{
    public event Action OnPuzzleStart;
    public event Action OnPuzzleEnd;

    public Collider2D _collider;

    [SerializeField] private Animator _animator;

    public void OpenDoor()
    {
        _collider.enabled = false;

        //animation;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnPuzzleStart?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        OnPuzzleEnd.Invoke();
    }
}
