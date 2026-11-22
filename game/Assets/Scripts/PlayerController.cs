/// <summary>
/// This player controller class will update the movement and animation of the student player.
/// Standard coding documentation can be found in
/// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/documentation-comments
/// </summary>

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float speed = 3.0f;

    // Footstep variables
    public AudioSource audioSource;
    public AudioClip[] footsteps;       // A random one plays on every step
    public float timeBetweenSteps = 0.35f;
    private float stepTimer = 0;

    private Rigidbody rb;
    private Animator animator;
    private Vector3 movement;

    /// <summary>
    /// This method is called before the first frame update
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // So the student does not fall over
        animator = GetComponentInChildren<Animator>();
    }
    /// <summary>
    /// This method is called once per frame
    /// </summary>
    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        movement = new Vector3(x, 0f, z).normalized;

        // Tell the Animator how fast the student is going
        if (animator != null)
            animator.SetFloat("speed", movement.magnitude);

        // Look where the student walks
        if (movement.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.Slerp(
                transform.rotation, Quaternion.LookRotation(movement), 12f * Time.deltaTime);

        // Play a footstep while the student walks
        if (movement.sqrMagnitude > 0.01f)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0)
            {
                audioSource.PlayOneShot(footsteps[Random.Range(0, footsteps.Length)]);
                stepTimer = timeBetweenSteps;
            }
        }
    }
    /// <summary>
    /// This method is called every physics step
    /// </summary>
    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }
}
