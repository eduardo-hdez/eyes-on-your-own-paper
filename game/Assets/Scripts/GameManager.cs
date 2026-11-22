/// <summary>
/// This game manager class counts the copied answers and decides when the student wins or loses.
/// Standard coding documentation can be found in
/// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/documentation-comments
/// </summary>

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int goal = 4;                 // How many answers the student has to copy
    public Renderer myDesk;              // Paint it with a color as feedback
    public TextMeshProUGUI statusText;   // Message on the screen
    public bool atDesk = true;           // True when the student is sitting at their own desk

    // UI variables
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject restartButton;
    public GameObject startPanel;        // Start screen with the story and the instructions
    public Slider copyBar;              // Shows how much is left to copy an answer
    public TextMeshProUGUI copyText;    // Text over the bar while copying

    // All the classmates that could have an answer
    public Answer[] classmates;

    private int copied = 0;
    private bool finished = false;

    // Are all the answers copied? MyDesk checks this before letting the student win.
    public bool AllCopied
    {
        get { return copied >= goal; }
    }

    public void OnEnable()
    {
        TimeManager.OnHourChanged += TimeUp;
    }

    public void OnDisable()
    {
        TimeManager.OnHourChanged -= TimeUp;
    }

    /// <summary>
    /// This method is called before the first frame update
    /// </summary>
    void Start()
    {
        statusText.text = "Answers: 0 / " + goal;
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        restartButton.SetActive(false);
        HideCopyBar();

        // Show the start screen and wait until the player presses the button
        startPanel.SetActive(true);
        Time.timeScale = 0;

        // Give the answers to random classmates, different every game
        if (goal > classmates.Length) goal = classmates.Length; // Or the loop would never end

        int given = 0;
        while (given < goal)
        {
            int i = Random.Range(0, classmates.Length);
            if (!classmates[i].hasAnswer)
            {
                classmates[i].GiveAnswer();
                given++;
            }
        }
    }

    // The StartButton calls this method
    public void StartGame()
    {
        startPanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void ShowCopyBar(float amount)
    {
        copyBar.gameObject.SetActive(true);
        copyBar.value = amount;
        copyText.text = "Copying answer... " + (int)(amount * 100) + "%";
    }

    public void HideCopyBar()
    {
        copyBar.gameObject.SetActive(false);
    }

    public void CopyAnswer()
    {
        if (finished) return;

        copied++;
        // Debug.Log("Answer copied (" + copied + " of " + goal + ")");
        statusText.text = "Answers: " + copied + " / " + goal;

        if (AllCopied)
        {
            // Debug.Log("All answers copied. Go back to your desk.");
            statusText.text = "Go back to your desk!";
            if (myDesk != null) myDesk.material.color = Color.green;
        }
    }

    public void Win()
    {
        if (finished) return;
        finished = true;
        // Debug.Log("You win!");
        statusText.text = "You win!";
        winPanel.SetActive(true);
        restartButton.SetActive(true);
        Time.timeScale = 0; // Stop the game
    }

    public void Lose(string reason)
    {
        if (finished) return;
        finished = true;
        // Debug.Log("You lose: " + reason);
        statusText.text = reason;
        if (myDesk != null) myDesk.material.color = Color.red;
        losePanel.SetActive(true);
        restartButton.SetActive(true);
        Time.timeScale = 0; // Stop the game
    }

    private void TimeUp()
    {
        Lose("Time is up!");
    }

    // The RestartButton calls this method
    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
