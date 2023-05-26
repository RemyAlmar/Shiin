using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Kunai kunai))
        {
            kunai.ReturnToPLayer();
        }
        if(collision.TryGetComponent(out HealthLifeAndDeath hld))
        {
            hld.health = 0;
        }
        if(collision.TryGetComponent(out HealthEnemy enemy))
        {
            enemy.Respawn();
        }
    }
}
