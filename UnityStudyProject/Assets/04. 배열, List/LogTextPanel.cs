using TMPro;
using UnityEngine;

public class LogTextPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text previewText;
    [SerializeField] private TMP_Text countText;

    public void UpdateLogText(string previewNumber, int strikeCount, int ballCount, int outCount)
    {
        if (previewText != null)
        {
            previewText.text = previewNumber;
        }

        if (countText != null)
        {
            countText.text = $"{strikeCount}<color=#00FF00>S</color> {ballCount}<color=#FFD700>B</color> {outCount}<color=#FF0000>O</color>";
        }
    }
}
