/// <summary>
/// This my desk class checks when the student is back at their own desk.
/// Standard coding documentation can be found in
/// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/documentation-comments
/// </summary>

using UnityEngine;

public class MyDesk : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager manager = Object.FindAnyObjectByType<GameManager>();
        if (manager == null) return;

        manager.atDesk = true;

        if (manager.AllCopied)
            manager.Win();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager manager = Object.FindAnyObjectByType<GameManager>();
        if (manager != null) manager.atDesk = false;
    }
}
