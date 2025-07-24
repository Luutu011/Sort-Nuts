using UnityEngine;

public class Nut : MonoBehaviour
{
    [SerializeField] private Sprite[] colorSprites;
    [SerializeField] private Sprite hiddenSprite; // Hidden nut sprite
    private SpriteRenderer spriteRenderer;
    private int colorIndex;
    private bool isHidden;

    public int ColorIndex => colorIndex;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(int colorIndex, bool isHidden, bool isTop)
    {
        this.colorIndex = colorIndex;
        this.isHidden = isHidden;
        SetTop(isTop);
    }

    public void SetTop(bool isTop)
    {
        spriteRenderer.sprite = (isHidden && !isTop) ? hiddenSprite : colorSprites[colorIndex];
    }
}