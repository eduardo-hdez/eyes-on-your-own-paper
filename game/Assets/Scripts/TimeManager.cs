/// <summary>
/// This time manager class is the clock of the exam and tells other scripts when the minutes and hours change.
/// Standard coding documentation can be found in
/// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/documentation-comments
/// </summary>

using UnityEngine;
using System;

public class TimeManager : MonoBehaviour
{
    public static Action OnMinuteChanged;
    public static Action OnHourChanged;

    public static int Minute{get; private set;}
    public static int Hour{get; private set;}

    // Clock variables
    public float minuteToRealTime = 1.0f; // Real seconds that one exam minute lasts
    private float timer;

    /// <summary>
    /// This method is called before the first frame update
    /// </summary>
    void Start()
    {
        // The exam starts at 8:00
        Minute = 0;
        Hour = 8;
        timer = minuteToRealTime;
    }
    /// <summary>
    /// This method is called once per frame
    /// </summary>
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            Minute++;

            // Change the hour before telling the other scripts, so the clock never shows :60
            if (Minute >= 60)
            {
                Hour++;
                Minute = 0;
                OnHourChanged?.Invoke();
            }

            OnMinuteChanged?.Invoke();

            timer = minuteToRealTime;
        }
    }
}
