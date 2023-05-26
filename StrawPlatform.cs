using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrawPlatform : MonoBehaviour
{
    [SerializeField] private GameObject Platform;
    [SerializeField] private SpriteRenderer sprite_Platform;
    [SerializeField] private SpriteRenderer spriteStraw;
    [SerializeField] private List<Transform> listDestinationPoint = new();
    [SerializeField] private Transform nextPos;
    [SerializeField] private int nextPosIndex;
    [SerializeField] private Vector2 refVelocity;
    [SerializeField, Range(0f, 30f)] private float movingTime;
    [SerializeField] private bool isActive;
    // Start is called before the first frame update
    void Awake()
    {
        sprite_Platform = Platform.GetComponent<SpriteRenderer>();
        spriteStraw = GetComponent<SpriteRenderer>();
        foreach (Transform t in transform.parent)
        {
            if (t.CompareTag("DestPoint"))
            {
                listDestinationPoint.Add(t);
            }
        }
    }
    private void Start()
    {
        nextPos = listDestinationPoint[0];
        Platform = Instantiate(Platform, nextPos.position, nextPos.rotation, transform);
        if (sprite_Platform.sortingOrder != -5)
        {
            sprite_Platform.sortingOrder = -5;
        }
    }

    private void Update()
    {
        PlatformMoving();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("CollectibleWeapon"))
        {
            isActive = true;
            spriteStraw.color = new Color32(255, 0, 90, 255);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("CollectibleWeapon"))
        {
            isActive = false;
        spriteStraw.color = Color.white;
        }
    }
    private void PlatformMoving()
    {
        nextPos = isActive ? listDestinationPoint[1] : listDestinationPoint[0];
        Platform.transform.position = Vector2.SmoothDamp(Platform.transform.position, nextPos.position, ref refVelocity, movingTime);
        float distToNextPos = Vector2.Distance(Platform.transform.position, nextPos.position);
        if (distToNextPos < 0.1f)
        {
            Platform.transform.position = nextPos.position;
        }
    }
}
