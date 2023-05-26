using UnityEngine;

public class UnlockPermutFonction : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision != null && collision.CompareTag("Player"))
        {
            if(collision.TryGetComponent(out Permutation permut))
            {
                permut.permutUnlocked = true;
                Destroy(gameObject);
            }
        }
    }
}
