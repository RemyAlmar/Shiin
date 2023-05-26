using System.Collections;
using UnityEngine;

public class DetectingNoise : MonoBehaviour
{
    private ComportementAI comportementAI;
    [SerializeField, Range(1f, 5f)] private float timeMax;
    [SerializeField] private LayerMask layersHitRay;

    private Vector2 myPosition;
    private Vector2 noiseDirection;
    private Vector2 noisePosition;
    private float distance;

    private void Awake()
    {
        comportementAI = GetComponent<ComportementAI>();
    }

    private void Update()
    {
        DistanceWithNoise();
        MoveTowardNoise();
    }
    // Recupere la position du bruit et se met en mode Cherche
    public IEnumerator ComeNoise(Vector2 targetPosition)
    {
        if (!comportementAI.IsFollowingTarget)
        {
            noisePosition = targetPosition;
            noiseDirection = (targetPosition - myPosition).normalized;
            comportementAI.SearchNoiseOrigin = true;
            comportementAI.IsAlerted = true;
            yield return new WaitForSeconds(10f);
            comportementAI.SearchNoiseOrigin = false;
        }
    }
    // Se deplace jusqu'a la position du bruit jusqu'a etre tres proche ou avoir mis trop de temps pour y aller
    private void MoveTowardNoise()
    {
        if (comportementAI.SearchNoiseOrigin)
        {
            if (RaycastToNoisePosition(noiseDirection) && distance >= 1f)
            {
                comportementAI.SearchNoiseOrigin = true;
                comportementAI.GoToNoise(noiseDirection);
            }
            else
            {
                comportementAI.SearchNoiseOrigin = false;
                comportementAI.IsStopped = true;
                StartCoroutine(comportementAI.StopMoveAndBack());
                comportementAI.WatchAnywhere();
            }
        }
    }
    // Distance entre moi et le bruit
    private void DistanceWithNoise()
    {
        if(noisePosition != null)
        {
            myPosition = transform.position;
            distance = Vector2.Distance(new(myPosition.x, 0f), new(noisePosition.x, 0f));
        }
    }
    // Est ce que je vois l'origine du bruit
    private RaycastHit2D RaycastToNoisePosition(Vector2 targetDir)
    {
        RaycastHit2D ray = Physics2D.Raycast(transform.position, targetDir, Mathf.Infinity,layersHitRay);
        return ray;
    }
}
