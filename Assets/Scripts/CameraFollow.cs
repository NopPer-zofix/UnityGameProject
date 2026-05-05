using UnityEngine;

/// <summary>
/// CameraFollow — attach to the Main Camera.
///
/// - Waits "inputDelay" seconds before following the player.
/// - Smoothly zooms out (increases orthographic size) while player moves,
///   zooms back in when player stops.
///
/// All values are tunable in the Inspector.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("Leave empty — will find the Player tag automatically.")]
    public Transform player;

    [Header("Follow Delay")]
    [Tooltip("How many seconds the player must be moving before the camera reacts.")]
    public float inputDelay = 1f;

    [Header("Smoothing")]
    [Tooltip("How quickly the camera catches up once it starts moving. Lower = smoother.")]
    public float smoothTime = 0.3f;
    [Tooltip("Maximum speed the camera can travel.")]
    public float maxSpeed   = 20f;

    [Header("Zoom")]
    [Tooltip("Camera size when the player is standing still.")]
    public float zoomIdle    = 5f;
    [Tooltip("Camera size when the player is moving.")]
    public float zoomMoving  = 7f;
    [Tooltip("How quickly the zoom transitions. Lower = slower.")]
    public float zoomSpeed   = 3f;

    // ── private ──────────────────────────────────────────────────────────────
    private Camera        cam;
    private Vector3       velocity     = Vector3.zero;
    private float         movingTimer  = 0f;
    private bool          cameraActive = false;
    private Vector3       targetPos;

    // ─────────────────────────────────────────────────────────────────────────
    void Start()
    {
        cam = GetComponent<Camera>();

        if (player == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("Player");
            if (obj != null) player = obj.transform;
        }

        if (player != null)
            targetPos = GetTargetPos();

        transform.position = targetPos;
        cam.orthographicSize = zoomIdle;
    }

    // ─────────────────────────────────────────────────────────────────────────
    void LateUpdate()
    {
        if (player == null) return;

        bool playerIsMoving = IsPlayerMoving();

        // ── Follow delay logic ───────────────────────────────────────────────
        if (playerIsMoving)
        {
            movingTimer += Time.deltaTime;
            if (movingTimer >= inputDelay)
                cameraActive = true;
        }
        else
        {
            movingTimer  = 0f;
            cameraActive = false;
        }

        if (cameraActive)
            targetPos = GetTargetPos();

        transform.position = Vector3.SmoothDamp(
            transform.position, targetPos, ref velocity, smoothTime, maxSpeed);

        // ── Zoom logic ───────────────────────────────────────────────────────
        float targetZoom = playerIsMoving ? zoomMoving : zoomIdle;
        cam.orthographicSize = Mathf.Lerp(
            cam.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
    }

    // ─────────────────────────────────────────────────────────────────────────
    bool IsPlayerMoving()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        return Mathf.Abs(h) > 0.01f || Mathf.Abs(v) > 0.01f;
    }

    Vector3 GetTargetPos()
    {
        return new Vector3(player.position.x, player.position.y, transform.position.z);
    }
}