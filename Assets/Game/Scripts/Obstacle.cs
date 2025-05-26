using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public SpriteRenderer SR => _spriteRenderer;
}
