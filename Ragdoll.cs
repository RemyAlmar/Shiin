using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ragdoll : MonoBehaviour
{
    [SerializeField] private BoxCollider2D myCollider;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D[] bodyParts;
    [SerializeField] private Collider2D[] collidBodyParts;

    [SerializeField] private Dictionary<Rigidbody2D, Vector3> initialPosition = new();
    [SerializeField] private Dictionary<Rigidbody2D, Quaternion> initialRotation = new();
    void Awake()
    {
        myCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        bodyParts = GetComponentsInChildren<Rigidbody2D>();
        collidBodyParts = GetComponentsInChildren<Collider2D>();
    }
    private void Start()
    {
        foreach(Rigidbody2D bodyPart in bodyParts)
        {
            initialPosition.Add(bodyPart, bodyPart.transform.localPosition);
            initialRotation.Add(bodyPart, bodyPart.transform.localRotation);
        } 
    }

    public void DisableRagdoll()
    {
        // Used for reappear correctly at the checkpoint (Player Only)
        foreach (Rigidbody2D bodyPart in bodyParts)
        {
            bodyPart.transform.localPosition = initialPosition[bodyPart];
            bodyPart.transform.localRotation = initialRotation[bodyPart];
            bodyPart.simulated = false;
            bodyPart.bodyType = RigidbodyType2D.Dynamic;
        }
        foreach (Collider2D collBodyPart in collidBodyParts)
        {
            collBodyPart.enabled = true;
        }
        animator.enabled = true;
        myCollider.enabled = true;
        rb.simulated = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    public void RecordTransform()
    {
        //Used to record transform for respawn (Player only)
        foreach (Rigidbody2D bodyPart in bodyParts)
        {
            initialPosition[bodyPart] = bodyPart.transform.localPosition;
            initialRotation[bodyPart] = bodyPart.transform.localRotation;
        }
        
    }
    // Ragdoll become active
    public IEnumerator RagdollOn(float timeToDie, int killerDirection)
    {
        animator.enabled = false;
        myCollider.enabled = false;
        foreach(Rigidbody2D bodyPart in bodyParts)
        {
            float randNumbX = Random.Range(5f, 15f);
            float randNumbY = Random.Range(-1f, 1f);
            bodyPart.bodyType = RigidbodyType2D.Dynamic;
            bodyPart.simulated = true;
            bodyPart.velocity = new(killerDirection * randNumbX, randNumbY);
        }
        rb.simulated = false;
        yield return new WaitForSeconds(timeToDie);
        foreach (Rigidbody2D bodyPart in bodyParts)
        {
            bodyPart.bodyType = RigidbodyType2D.Kinematic;
        }
        foreach (Collider2D collBodyPart in collidBodyParts)
        {
            collBodyPart.enabled = false;
        }
    }
}
