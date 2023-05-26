using UnityEngine;

public class PlatformRules : MonoBehaviour
{
    private void OnCollisionStay2D(Collision2D collision)
    {
        collision.gameObject.transform.SetParent(transform);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        collision.gameObject.transform.SetParent(null);        
    }
}
