/// <summary>
/// This teacher controller class keeps the teacher busy at the desk and makes them look up to watch the class every few minutes of the exam.
/// Standard coding documentation can be found in
/// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/documentation-comments
/// </summary>

using UnityEngine;
using System.Collections;

public class TeacherController : MonoBehaviour
{
    // Teacher variables
    public float timeToLook = 4.0f;   // Seconds the teacher looks at the class
    public float sweepAngle = 50.0f;  // How far left and right the teacher looks
    public bool isLooking = false;    // True when the teacher looks up from the desk

    // Vision variables
    public GameObject player;
    public float viewDistance = 14.0f; // Enough to reach the back of the classroom
    public float viewAngle = 40.0f;   // Half of the cone
    public bool canSeePlayer = false;
    public Light watchLight;          // Light that shows where the teacher is looking
    public MeshRenderer visionCone;   // Shape on the floor that shows everything the teacher sees
    public Color coneColor = new Color(1f, 0.95f, 0.6f, 0.25f);  // See-through yellow
    public Color seenColor = new Color(1f, 0.1f, 0.1f, 0.35f);   // See-through red

    private Quaternion busyRotation;  // Looking down at the desk

    public void OnEnable()
    {
        TimeManager.OnMinuteChanged += TimeCheck;
    }

    public void OnDisable()
    {
        TimeManager.OnMinuteChanged -= TimeCheck;
    }

    /// <summary>
    /// This method is called before the first frame update
    /// </summary>
    void Start()
    {
        busyRotation = transform.rotation;
        watchLight.enabled = false;
        visionCone.enabled = false;
    }
    /// <summary>
    /// This method is called once per frame
    /// </summary>
    void Update()
    {
        // The teacher only sees the class while looking up
        if (isLooking)
            CheckVision();
    }

    private void CheckVision()
    {
        canSeePlayer = false;

        Vector3 eyes = transform.position + Vector3.up * 1.0f;
        Vector3 head = player.transform.position + Vector3.up * 0.4f; // Look at the head, over the desks
        Vector3 toPlayer = head - eyes;

        // Only the floor direction matters for the angle
        Vector3 flatToPlayer = toPlayer;
        flatToPlayer.y = 0;

        // Is the student close enough and inside the cone?
        if (toPlayer.magnitude < viewDistance && Vector3.Angle(transform.forward, flatToPlayer) < viewAngle)
        {
            // Is there something in the middle, like a desk?
            RaycastHit hit;
            if (Physics.Raycast(eyes, toPlayer.normalized, out hit, viewDistance, ~0, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.CompareTag("Player"))
                    canSeePlayer = true;
            }
        }

        // The light follows the teacher
        watchLight.transform.position = transform.position + Vector3.up * 2.5f;
        watchLight.transform.rotation = transform.rotation * Quaternion.Euler(15, 0, 0);
        watchLight.spotAngle = viewAngle * 2;
        watchLight.range = viewDistance + 3;

        // The cone turns red when it sees the student
        if (canSeePlayer)
            visionCone.material.color = seenColor;
        else
            visionCone.material.color = coneColor;

        // If the teacher sees the student out of their desk, they get caught
        if (canSeePlayer)
        {
            GameManager manager = Object.FindAnyObjectByType<GameManager>();
            if (manager != null && !manager.atDesk)
                manager.Lose("The teacher caught you!");
        }
    }

    private void TimeCheck()
    {
        // Every 10 minutes the teacher looks up at the class
        if (TimeManager.Minute % 10 == 0 && !isLooking)
        {
            StartCoroutine(LookAtClass());
        }
    }

    private IEnumerator LookAtClass()
    {
        isLooking = true;
        watchLight.enabled = true;
        visionCone.enabled = true;

        float timeElapsed = 0;

        // Look from one side of the classroom to the other
        while (timeElapsed < timeToLook)
        {
            float angle = Mathf.Lerp(-sweepAngle, sweepAngle, timeElapsed / timeToLook);
            transform.rotation = busyRotation * Quaternion.Euler(0, angle, 0);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Back to work
        transform.rotation = busyRotation;
        isLooking = false;
        canSeePlayer = false;
        watchLight.enabled = false;
        visionCone.enabled = false;
    }
}
