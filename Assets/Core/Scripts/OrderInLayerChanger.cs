using UnityEngine;

public class OrderInLayerChanger : MonoBehaviour
{
    private int _underPlayerOrderInLayer;
    private int _overPlayerOrderInLayer;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _overPlayerOrderInLayer = _spriteRenderer.sortingOrder;
        _underPlayerOrderInLayer = _overPlayerOrderInLayer - 2;
    }

    void Update()
    {
        if(Player.Instance.transform.position.y >
            transform.position.y)
        {
            _spriteRenderer.sortingOrder = _overPlayerOrderInLayer;
        }
        else
        {
            _spriteRenderer.sortingOrder = _underPlayerOrderInLayer;
        }
    }
}
