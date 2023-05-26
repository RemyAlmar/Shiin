using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockKunaiFunction : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision != null)
        {
            if(collision.CompareTag("Player"))
            {
                AimAndShoot ams = collision.gameObject.GetComponentInChildren<AimAndShoot>();
                ams.launchKunaiUnlocked = true;
                Destroy(gameObject);
            }
        }
    }
}
