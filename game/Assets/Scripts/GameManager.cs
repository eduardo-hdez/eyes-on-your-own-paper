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
    public TextMeshProUGUI winText;      // Big text in the WinPanel
    public TextMeshProUGUI loseText;     // Big text in the LosePanel
    public GameObject restartButton;
    public GameObject startPanel;        // Start screen with the story and the instructions
    public Slider copyBar;              // Shows how much is left to copy an answer
    public TextMeshProUGUI copyText;    // Text over the bar while copying

    // All the classmates that could have an answer
    public Answer[] classmates;

    // Audio variables
    public AudioSource musicSource;      // Plays the music in a loop
    public AudioSource soundSource;      // Plays the short sounds
    public AudioSource writingSource;    // Pencil sound in a loop while copying
    public AudioClip startMusic;
    public AudioClip gameplayMusic;
    public AudioClip winSound;
    public AudioClip loseSound;
    public AudioClip copySound;
    public AudioClip clickSound;

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
        PlayMusic(startMusic);

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
        soundSource.PlayOneShot(clickSound);
        PlayMusic(gameplayMusic);
    }

    private void PlayMusic(AudioClip music)
    {
        musicSource.clip = music;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void ShowCopyBar(float amount)
    {
        copyBar.gameObject.SetActive(true);

        // Start the pencil sound only once, it keeps looping by itself
        if (!writingSource.isPlaying)
            writingSource.Play();

        copyBar.value = amount;
        copyText.text = "Copying answer... " + (int)(amount * 100) + "%";
    }

    public void HideCopyBar()
    {
        copyBar.gameObject.SetActive(false);
        writingSource.Stop();
    }

    public void CopyAnswer()
    {
        if (finished) return;

        copied++;
        // Debug.Log("Answer copied (" + copied + " of " + goal + ")");
        statusText.text = "Answers: " + copied + " / " + goal;
        soundSource.PlayOneShot(copySound);

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
        // Debug.Log("You passed!");
        statusText.text = "You passed!";
        winText.text = "You passed!\n<size=50%>Nobody needs to know how.</size>";
        musicSource.Stop();
        soundSource.PlayOneShot(winSound);
        winPanel.SetActive(true);
        restartButton.SetActive(true);
        Time.timeScale = 0; // Stop the game
    }

    // title is the big text, message is the small text under it
    public void Lose(string title, string message)
    {
        if (finished) return;
        finished = true;
        // Debug.Log("You lose: " + title);
        statusText.text = title;
        loseText.text = title + "\n<size=50%>" + message + "</size>";
        if (myDesk != null) myDesk.material.color = Color.red;
        musicSource.Stop();
        soundSource.PlayOneShot(loseSound);
        losePanel.SetActive(true);
        restartButton.SetActive(true);
        Time.timeScale = 0; // Stop the game
    }

    private void TimeUp()
    {
        Lose("Time's up!", "Pencils down. You failed the exam.");
    }

    // The RestartButton calls this method
    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
