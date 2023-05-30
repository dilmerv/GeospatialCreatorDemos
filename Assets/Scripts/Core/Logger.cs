using System.Linq;
using DilmerGames.Core.Singletons;
using TMPro;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Logger : Singleton<Logger>
{
    [SerializeField]
    private TextMeshProUGUI debugAreaText = null;

    [SerializeField]
    private bool enableDebug = false;

    [SerializeField]
    private int maxLines = 15;

    // logger toggle functionality
    private Transform loggerContainer;

    private Button toggleButton;

    private bool isVisible = true;

    void Awake()
    {
        if (debugAreaText == null)
        {
            debugAreaText = GetComponent<TextMeshProUGUI>();
        }
        debugAreaText.text = string.Empty;
        
        // get logger container
        var loggerContainer = transform.GetChild(0);

        toggleButton = GetComponentInChildren<Button>();
        toggleButton.onClick.AddListener(() =>
        {
            isVisible = !isVisible;
            if(loggerContainer != null)
                loggerContainer.gameObject.SetActive(isVisible);

            toggleButton.GetComponentInChildren<TextMeshProUGUI>()
                .text = isVisible ? "Hide Log" : "Show Log";
        });
    }

    void OnEnable()
    {
        debugAreaText.enabled = enableDebug;
        enabled = enableDebug;

        if (enabled)
        {
            debugAreaText.text += $"<color=\"white\">{DateTime.Now.ToString("HH:mm:ss.fff")} {this.GetType().Name} enabled</color>\n";
        }
    }

    public void Clear() => debugAreaText.text = string.Empty;

    public void LogInfo(string message)
    {
        ClearLines();

        debugAreaText.text += $"<color=\"green\">{DateTime.Now.ToString("HH:mm:ss.fff")} {message}</color>\n";
    }

    public void LogError(string message)
    {
        ClearLines();
        debugAreaText.text += $"<color=\"red\">{DateTime.Now.ToString("HH:mm:ss.fff")} {message}</color>\n";
    }

    public void LogWarning(string message)
    {
        ClearLines();
        debugAreaText.text += $"<color=\"yellow\">{DateTime.Now.ToString("HH:mm:ss.fff")} {message}</color>\n";
    }

    private void ClearLines()
    {
        if (debugAreaText.text.Split('\n').Count() >= maxLines)
        {
            debugAreaText.text = string.Empty;
        }
    }
}