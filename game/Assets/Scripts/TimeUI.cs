/// <summary>
/// This time UI class shows the exam clock on the screen.
/// Standard coding documentation can be found in
/// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/documentation-comments
/// </summary>

using UnityEngine;
using TMPro;

public class TimeUI : MonoBehaviour
{
    public TextMeshProUGUI timeText;

    private void OnEnable()
    {
        TimeManager.OnMinuteChanged += UpdateTime;
        TimeManager.OnHourChanged += UpdateTime;
    }

    private void OnDisable()
    {
        TimeManager.OnMinuteChanged -= UpdateTime;
        TimeManager.OnHourChanged -= UpdateTime;
    }

    private void UpdateTime()
    {
        // Show the time like 08:05 AM
        string amPm = "AM";
        if (TimeManager.Hour >= 12) amPm = "PM";

        timeText.text = $"{TimeManager.Hour.ToString("00")}:{TimeManager.Minute:00} {amPm}";
    }
}
