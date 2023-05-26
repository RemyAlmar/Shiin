using System.Collections;
using UnityEngine;

public class CloudPlatform : MonoBehaviour
{
    private SpriteRenderer sprite;
    private BoxCollider2D collid;
    [SerializeField, Range(1f, 8f)] private float respawnTime = 3f;
    [SerializeField, Range(0.5f, 2f)] private float disappearTime = 1f;
    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        collid = GetComponent<BoxCollider2D>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Respawn());
        }
    }
    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(disappearTime);
        sprite.color = new Color32(0, 0, 0, 70);
        collid.enabled = false;
        yield return new WaitForSeconds(respawnTime);
        sprite.color = Color.white;
        collid.enabled = true;
    }
}
