using System;
using System.Collections.Generic;
using UnityEngine;

public class StrawTarget : MonoBehaviour
{
    [SerializeField] private GameObject Platform;
    [SerializeField] private SpriteRenderer sprite_Platform;
    [SerializeField] private List<Transform> listDestinationPoint = new();
    [SerializeField] private List<GameObject> kunai_List;
    [SerializeField] private Transform nextPos;
    [SerializeField] private int nextPosIndex;
    [SerializeField] private Vector2 refVelocity;
    [SerializeField, Range(0f, 30f)] private float movingTime;
    [SerializeField, Range(0f, 2f)] private float radius = 0.5f;
    [SerializeField] private bool nextMoveIsActived;
    // Start is called before the first frame update
    void Awake()
    {
        sprite_Platform = Platform.GetComponent<SpriteRenderer>();
        foreach(Transform t in transform.parent)
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
    }
    private void Update()
    {
        if (sprite_Platform.sortingOrder != -5)
        {
            sprite_Platform.sortingOrder = -5;
        }
        StrawTrigger();
        ModifyDestinationPlatform();
        ClearList();
    }
    private void StrawTrigger()
    {
        Collider2D collision = Physics2D.OverlapCircle(transform.position, radius);
        if (collision.gameObject.CompareTag("CollectibleWeapon"))
        {
            if (!kunai_List.Contains(collision.gameObject))
            {
                nextMoveIsActived = !nextMoveIsActived;
                kunai_List.Add(collision.gameObject);
            }
            else
            {
                return;
            }
        }
        else
        {
            return;
        }
    }
    private void ClearList()
    {
        foreach(GameObject gameObject in kunai_List)
        {
            if(gameObject == null)
            {
                kunai_List.Remove(gameObject);
            }
        }
    }
    private void ModifyDestinationPlatform()
    {
        if(nextMoveIsActived)
        {
            nextPosIndex ++;
            if(nextPosIndex >= listDestinationPoint.Count)
            {
                nextPosIndex = 0;
            }
            nextPos = listDestinationPoint[nextPosIndex];
            nextMoveIsActived = false;
        }
        Platform.transform.position = Vector2.SmoothDamp(Platform.transform.position, nextPos.position, ref refVelocity, movingTime);
        float distToNextPos = Vector2.Distance(Platform.transform.position, nextPos.position);
        if(distToNextPos < 0.1f)
        {
            Platform.transform.position = nextPos.position;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
