using UnityEngine;

public class C_FollowingPlayer : MonoBehaviour
{
    [SerializeField] private float distanceFollowMax;
    [SerializeField] private float smoothTime;
    [SerializeField] private float offsetX;
    [SerializeField] private float offsetYGround;
    [SerializeField, Range(0f, -30f)] private float offsetMaxYMin;
    [SerializeField, Range(0f, 30f)] private float offsetMaxYMax;

    [SerializeField] private bool followPlayer;
    [SerializeField, Range(1f, 10f)] private float sizeCamera = 5f;

    
    private Camera m_Camera;
    private GameObject player;
    private Rigidbody2D rb_Player;

    private Vector3 playerPosition;


    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb_Player = player.GetComponent<Rigidbody2D>();
        m_Camera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        m_Camera.orthographicSize = sizeCamera;
        FollowPlayer();
    }
    private void FollowPlayer()
    {
        if(Vector2.Distance(transform.position, playerPosition) >= distanceFollowMax)
        {
            smoothTime = 10f;
        }else
        {
            smoothTime = Mathf.Max(smoothTime -= Time.deltaTime * smoothTime, 2f);
        }
        //float smoothTimeModified = rb_Player.velocity.y > -0.1f ? smoothTime : smoothTime * 2f;
        if (Mathf.Abs(rb_Player.velocity.x) > 0.2f && Mathf.Abs(rb_Player.velocity.x) < 20f)
        {
            playerPosition = new(player.transform.position.x + offsetX * rb_Player.velocity.x, player.transform.position.y + Mathf.Clamp(rb_Player.velocity.y, offsetMaxYMin, offsetMaxYMax) + offsetYGround, player.transform.position.z - Mathf.Floor(m_Camera.farClipPlane));
        } else
        {
            playerPosition = new(player.transform.position.x, player.transform.position.y + Mathf.Clamp(rb_Player.velocity.y, offsetMaxYMin, offsetMaxYMax) + offsetYGround, player.transform.position.z - Mathf.Floor(m_Camera.farClipPlane));
        }
        transform.position = Vector3.Lerp(transform.position, playerPosition, smoothTime * Time.deltaTime);
    }
}
