using System.Collections.Generic;
using UnityEngine;

public class ObjectiveDone : MonoBehaviour
{
    [SerializeField] private List<GameObject> objects = new();

    private void FixedUpdate()
    {
        if(objects.Count > 0)
        {
            objects.RemoveAll(GameObject => GameObject == null);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision != null)
        {
            if (collision.CompareTag("Player"))
            {
                if(objects.Count == 0)
                {
                    CurrentSceneManager.Instance.objectiveIsDone = true;
                }
            }
        }
    }
}
