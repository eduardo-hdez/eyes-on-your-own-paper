/// <summary>
/// This answer class lets the student copy the exam of a classmate when they stay close for a few seconds.
/// Standard coding documentation can be found in
/// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/documentation-comments
/// </summary>

using UnityEngine;

public class Answer : MonoBehaviour
{
    public GameObject sparkles;   // Particles that show the answer can be copied
    public float copyTime = 2.0f; // Seconds the student has to stay close to copy
    public bool hasAnswer = false; // The GameManager picks which classmates have an answer

    private bool copied = false;
    private float copyTimer = 0;

    // The GameManager calls this method when the game starts
    public void GiveAnswer()
    {
        hasAnswer = true;
        sparkles.SetActive(true);
    }

    void OnTriggerStay(Collider other)
    {
        if (!hasAnswer || copied) return;

        if (other.CompareTag("Player"))
        {
            GameManager manager = Object.FindAnyObjectByType<GameManager>();
            if (manager == null) return;

            copyTimer += Time.deltaTime;
            manager.ShowCopyBar(copyTimer / copyTime);

            if (copyTimer >= copyTime)
            {
                manager.CopyAnswer();
                manager.HideCopyBar();

                copied = true;
                sparkles.SetActive(false); // The answer is gone
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!hasAnswer || copied) return;

        if (other.CompareTag("Player"))
        {
            // The student walked away: start again
            copyTimer = 0;

            GameManager manager = Object.FindAnyObjectByType<GameManager>();
            if (manager != null) manager.HideCopyBar();
        }
    }
}
