using UnityEngine;

public class Respawn : MonoBehaviour
{
    [SerializeField] private Vector2 mySpawnPosition;
    [SerializeField] private Ragdoll recordPosition;
    public Vector2 SpawnPosition => mySpawnPosition;

    // Au départ du jeu apparait au point enregistré, 
    //  Enregistre position des checkpoints traverser pour reapparaitre au dernier check point
    private void Awake()
    {
        recordPosition = GetComponent<Ragdoll>();
    }
    void Start()
    {
        RecordNewCheckPoint(transform.position);
    }
    // Teleporte le joueur au point de spawn
    public void RespawnToLastCheckPoint()
    {
        transform.position = mySpawnPosition;
        recordPosition.DisableRagdoll();
    }
    // Enregistre le nouveau checkpoint
    public void RecordNewCheckPoint(Vector2 newPosition)
    {
        mySpawnPosition = newPosition;
        recordPosition.RecordTransform();
    }
}
