/// <summary>
/// This teacher controller class makes the teacher walk between waypoints and turn around to watch the students every few minutes of the exam.
/// Standard coding documentation can be found in
/// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/documentation-comments
/// </summary>

using UnityEngine;
using System.Collections;

public class TeacherController : MonoBehaviour
{
    // Teacher variables
    public float timeToTurn = 1.0f; // Seconds it takes to turn around
    public float walkSpeed = 1.2f;
    public bool isWatching = false; // True when the teacher is looking at the students
    public Light watchLight;        // Light that turns on when the teacher is watching

    // Waypoint variables
    public Transform[] waypoints;
    private Vector3[] waypointPositions; // The waypoints are children of the teacher, so save where they are
    private int currentWaypoint = 0;

    private bool isTurning = false;
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
        if (isWatching)
        {
            // If the teacher is watching and the student is not at their desk, they get caught
            GameManager manager = Object.FindAnyObjectByType<GameManager>();
            if (manager != null && !manager.atDesk)
                manager.Lose("The teacher caught you!");

            animator.SetFloat("speed", 0);
        }
        else if (!isTurning)
        {
            // Walk to the next waypoint
            Vector3 target = waypointPositions[currentWaypoint];
            target.y = transform.position.y;
            transform.position = Vector3.MoveTowards(transform.position, target, walkSpeed * Time.deltaTime);

            Vector3 direction = target - transform.position;
            if (direction.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(direction);

            if (Vector3.Distance(transform.position, target) < 0.05f)
                currentWaypoint = (currentWaypoint + 1) % waypointPositions.Length;

            animator.SetFloat("speed", 1);
        }
        else
        {
            animator.SetFloat("speed", 0);
        }
    }

    private void TimeCheck()
    {
        // Every 5 minutes the teacher turns around
        if (TimeManager.Minute % 5 == 0)
        {
            StartCoroutine(TurnTeacher());
        }
    }

    private IEnumerator TurnTeacher()
    {
        isTurning = true;
        bool watchNow = !isWatching;

        // Stop watching right away when turning back to the board
        if (!watchNow)
        {
            isWatching = false;
            if (watchLight != null) watchLight.enabled = false;
        }

        Quaternion currentRot = transform.rotation;
        Quaternion targetRot = Quaternion.LookRotation(Vector3.forward); // Looking at the board

        if (watchNow)
            targetRot = Quaternion.LookRotation(Vector3.back); // Looking at the students

        float timeElapsed = 0;

        while (timeElapsed < timeToTurn)
        {
            transform.rotation = Quaternion.Lerp(currentRot, targetRot, timeElapsed / timeToTurn);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRot;

        // Start watching only when the turn is done, so the student has time to run back
        if (watchNow)
        {
            isWatching = true;
            if (watchLight != null) watchLight.enabled = true;
        }

        isTurning = false;
    }
}
