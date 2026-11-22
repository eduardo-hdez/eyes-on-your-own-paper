/// <summary>
/// This student idle class makes the students move a little while they take the exam.
/// Standard coding documentation can be found in
/// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/documentation-comments
/// </summary>

using UnityEngine;

public class StudentIdle : MonoBehaviour
{
    // Look around variables
    public float lookAngle = 15.0f; // How far the student turns to the sides
    public float lookSpeed = 1.0f;

    // Writing variables
    public float bobHeight = 0.03f; // Small up and down movement, like writing
    public float bobSpeed = 5.0f;

    private Quaternion startRotation;
    private Vector3 startPosition;
    private float offset; // So every student moves at a different time

    /// <summary>
    /// This method is called before the first frame update
    /// </summary>
    void Start()
    {
        startRotation = transform.rotation;
        startPosition = transform.position;
        offset = Random.Range(0f, 10f);
        lookSpeed = lookSpeed * Random.Range(0.6f, 1.4f);
    }
    /// <summary>
    /// This method is called once per frame
    /// </summary>
    void Update()
    {
        // Turn a little to the left and to the right
        float angle = Mathf.Sin(Time.time * lookSpeed + offset) * lookAngle;
        transform.rotation = startRotation * Quaternion.Euler(0, angle, 0);

        // Move up and down a little, like writing on the exam
        float bob = Mathf.Abs(Mathf.Sin(Time.time * bobSpeed + offset)) * bobHeight;
        transform.position = startPosition + Vector3.up * bob;
    }
}
