/// <summary>
/// This teacher controller class makes the teacher walk around the classroom and stop to look around every few minutes of the exam.
/// Standard coding documentation can be found in
/// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/documentation-comments
/// </summary>

using UnityEngine;
using System.Collections;

public class TeacherController : MonoBehaviour
{
    // Teacher variables
    public float walkSpeed = 1.5f;
    public float timeToLookAround = 3.0f; // Seconds the teacher stops to look around
    public bool isLookingAround = false;

    // Waypoint variables
    public Transform[] waypoints;
    private Vector3[] waypointPositions; // The waypoints are children of the teacher, so save where they are
    private int currentWaypoint = 0;

    private Animator animator;

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
        animator = GetComponentInChildren<Animator>();

        waypointPositions = new Vector3[waypoints.Length];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypointPositions[i] = waypoints[i].position;
        }
    }
    /// <summary>
    /// This method is called once per frame
    /// </summary>
    void Update()
    {
        if (isLookingAround)
        {
            animator.SetFloat("speed", 0);
            return;
        }

        // Walk to the next waypoint
        Vector3 target = waypointPositions[currentWaypoint];
        target.y = transform.position.y;
        transform.position = Vector3.MoveTowards(transform.position, target, walkSpeed * Time.deltaTime);

        Vector3 direction = target - transform.position;
        if (direction.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 8f * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.05f)
            currentWaypoint = (currentWaypoint + 1) % waypointPositions.Length;

        animator.SetFloat("speed", 1);
    }

    private void TimeCheck()
    {
        // Every 8 minutes the teacher stops and looks around
        if (TimeManager.Minute % 8 == 0 && !isLookingAround)
        {
            StartCoroutine(LookAround());
        }
    }

    private IEnumerator LookAround()
    {
        isLookingAround = true;

        Quaternion startRot = transform.rotation;
        float timeElapsed = 0;

        // Turn all the way around slowly
        while (timeElapsed < timeToLookAround)
        {
            float angle = Mathf.Lerp(0, 360, timeElapsed / timeToLookAround);
            transform.rotation = startRot * Quaternion.Euler(0, angle, 0);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = startRot;
        isLookingAround = false;
    }
}
