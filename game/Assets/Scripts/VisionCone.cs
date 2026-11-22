/// <summary>
/// This vision cone class draws on the floor the area the teacher can see, and stops it at the walls.
/// Standard coding documentation can be found in
/// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/documentation-comments
/// </summary>

using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class VisionCone : MonoBehaviour
{
    public int segments = 40; // More segments make the edge rounder

    private TeacherController teacher;
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    /// <summary>
    /// This method is called before the first frame update
    /// </summary>
    void Start()
    {
        teacher = GetComponentInParent<TeacherController>();

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // One point for the teacher and one for every border point
        vertices = new Vector3[segments + 2];
        triangles = new int[segments * 3];

        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }
    }
    /// <summary>
    /// This method is called once per frame
    /// </summary>
    void Update()
    {
        vertices[0] = Vector3.zero; // The teacher

        for (int i = 0; i <= segments; i++)
        {
            // Direction of this border point, from the left side of the cone to the right side
            float angle = Mathf.Lerp(-teacher.viewAngle, teacher.viewAngle, (float)i / segments) * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));

            // Stop the cone at the walls
            float distance = teacher.viewDistance;
            Vector3 eyes = teacher.transform.position + Vector3.up * 1.0f;
            RaycastHit[] hits = Physics.RaycastAll(eyes, transform.TransformDirection(direction), teacher.viewDistance, ~0, QueryTriggerInteraction.Ignore);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.name.StartsWith("Wall") && hit.distance < distance)
                    distance = hit.distance;
            }

            vertices[i + 1] = direction * distance;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
    }
}
