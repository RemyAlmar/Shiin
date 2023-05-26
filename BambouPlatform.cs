using UnityEngine;

public class BambouPlatform: MonoBehaviour
{
    [SerializeField] private BoxCollider2D box;
    [SerializeField] private SpriteRenderer sprite;
    private void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        BoxPositionning();
    }
    // Update is called once per frame
    void Update()
    {
        BoxPositionning();
    }
    private void BoxPositionning()
    {
        box.offset = new(box.offset.x, sprite.size.y - box.size.y * 1.5f);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        box.isTrigger = false;
    }
}
