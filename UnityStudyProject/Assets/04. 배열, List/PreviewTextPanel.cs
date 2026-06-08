using TMPro;
using UnityEngine;

public class PreviewTextPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text privewNumberText;

    public void UpdatePrivewText(int number)
    {
        if (privewNumberText == null)
            return;

        privewNumberText.text = number.ToString();
    }

    public void ResetPreviewText()
    {
        privewNumberText.text = string.Empty;
    }
}
