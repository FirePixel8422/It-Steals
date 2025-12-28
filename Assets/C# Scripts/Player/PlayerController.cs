using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : UpdateMonoBehaviour
{
    [SerializeField] private PlayerAudio audioHandler;

    [SerializeField] private float moveSpeed = 5f;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckLength = 0.5f;

    [Header("Camera Settings")]
    [SerializeField] private float mouseSensitivity = 100f;

    [Header("Camera Transform and Lerp Speed")]
    [SerializeField] Transform camTransform;
    [SerializeField] private float cameraAcceleration = 0.1f;

    private Rigidbody rb;

    private Vector2 moveDir;
    private Vector2 mouseDelta;
    private float camRotX;
    private float camRotY;



    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveDir = ctx.ReadValue<Vector2>();
    }
    public void OnLook(InputAction.CallbackContext ctx)
    {
        mouseDelta = ctx.ReadValue<Vector2>() * mouseSensitivity;
    }

    private void Awake()
    {
        audioHandler.Init(this);
        audioHandler.StartHeartBeat();
        audioHandler.StartDangerSFX();

        rb = GetComponent<Rigidbody>();

        camRotX = camTransform.localEulerAngles.x;
        camRotY = transform.localEulerAngles.y;
        Cursor.lockState = CursorLockMode.Locked;
    }

    protected override void OnUpdate()
    {
        Move();
        CameraMove();

        Stalker stalker = StalkerManager.stalkers[0];
        float dangerPercentage01 = math.saturate(Vector3.Distance(transform.position, stalker.transform.position) / audioHandler.DangerMaxDist);
        audioHandler.UpdateHeartBeat(dangerPercentage01);
        audioHandler.UpdateDangerSFX(dangerPercentage01);
    }

    /// <summary>
    /// Move the player based on input
    /// </summary>
    private void Move()
    {
        Vector3 forwardDir = transform.TransformDirection(new Vector3(moveDir.x, 0f, moveDir.y));

        Vector3 velocity = forwardDir * moveSpeed + new Vector3(0f, rb.linearVelocity.y, 0f);

        if (rb.linearVelocity.y > -0.1f && Physics.Raycast(groundCheck.position, Vector3.down, out RaycastHit hit, groundCheckLength))
        {
            velocity = AlignVelocityToRamp(velocity, hit.normal);
        }
        rb.linearVelocity = velocity;
    }
    /// <summary>
    /// Update velocity to be aligned to the ramp normal
    /// </summary>
    private float3 AlignVelocityToRamp(float3 velocity, float3 normal)
    {
        float3 n = math.normalize(normal);

        // Remove the component pushing into the ramp
        float3 projected = velocity - n * math.dot(velocity, n);

        float mag = math.length(projected);
        if (mag < 1e-5f)
        {
            // Velocity is basically perpendicular to the ramp
            return float3.zero;
        }

        return projected * (math.length(velocity) / mag);
    }


    /// <summary>
    /// Update FPS Camera with Lerping
    /// </summary>
    private void CameraMove()
    {
        camRotX -= mouseDelta.y;
        camRotX = Mathf.Clamp(camRotX, -90f, 90f);

        camRotY += mouseDelta.x;

        camTransform.localRotation = Quaternion.Euler(camRotX, 0, 0f);
        transform.localRotation = Quaternion.Euler(0, camRotY, 0f);
    }
}
