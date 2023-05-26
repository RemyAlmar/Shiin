using UnityEngine;
using System.Collections.Generic;

public class AmbiantMusicManager : MonoBehaviour
{
    private HealthLifeAndDeath hld;
    public static bool isSpotted;
    public List<Collider2D> enemies;

    private void Awake()
    {
        hld = GetComponent<HealthLifeAndDeath>();
    }
    private void Update()
    {
        PlayerIsSpotted();
    }
    private void PlayerIsSpotted()
    {
        if(enemies != null)
        {
            isSpotted = enemies.Count > 0 && hld.health > 0;
        } else
        {
            isSpotted = false;
        }
    }
}
