/// <summary>
/// This answer class lets the student copy the exam of a classmate when they get close.
/// Standard coding documentation can be found in
/// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/documentation-comments
/// </summary>

using UnityEngine;

public class Answer : MonoBehaviour
{
    public GameObject sparkles; // Particles that show the answer can be copied

    private bool copied = false;

    void OnTriggerEnter(Collider other)
    {
        if (copied) return;

        if (other.CompareTag("Player"))
        {
            GameManager manager = Object.FindAnyObjectByType<GameManager>();
            if (manager != null) manager.CopyAnswer();

            copied = true;
            if (sparkles != null) sparkles.SetActive(false); // The answer is gone
        }
    }
}
