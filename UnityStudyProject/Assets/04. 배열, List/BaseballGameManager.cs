using Mono.Cecil.Cil;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BaseballGameManager : MonoBehaviour
{
    private const int MAX_BASEBALL_COUNT = 4;

    [Header("UI Panel")]
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject resultUI;

    [Header("InGameUI")]
    [Header("Log Panel")]
    [SerializeField] private RectTransform logTextPanelContent;
    [SerializeField] private LogTextPanel logTextPanel;

    [SerializeField] private TMP_Text[] previewTexts;
    [SerializeField] private TMP_Text turnCountText;

    [SerializeField] private Button enterButton;

    [Header("ResultUI")]
    [SerializeField] private TMP_Text answerText;


    private List<LogTextPanel> logTextPanels = new List<LogTextPanel>();

    private int[] answerNumbers;
    private int[] previewNumbers;

    private int turnCount;
    private int currentIndex;

    private void Start()
    {
        if (previewTexts.Length < MAX_BASEBALL_COUNT)
        {
            Debug.LogError($"PreviewTexts에 값이 {MAX_BASEBALL_COUNT} 만큼 할당되지 않았습니다.");
            return;
        }

        if (logTextPanels == null)
            logTextPanels = new List<LogTextPanel>();

        logTextPanels = logTextPanelContent.GetComponentsInChildren<LogTextPanel>().ToList();

        answerNumbers = new int[MAX_BASEBALL_COUNT];
        previewNumbers = new int[MAX_BASEBALL_COUNT];

        OnStartGame();
    }

    public void OnStartGame()
    {
        inGameUI?.SetActive(true);
        resultUI?.SetActive(false);

        turnCount = 0;

        UpdateAnswerNumbers();
        ResetLogTextPanels();
        UpdateEnterButtonVisible();

        NextTurn();
    }

    private void NextTurn()
    {
        currentIndex = 0;

        for (int i = 0; i < MAX_BASEBALL_COUNT; i++)
        {
            previewNumbers[i] = -1;
            previewTexts[i].text = string.Empty;
        }

        turnCount++;
        turnCountText.text = $"Turn : {turnCount}";
    }

    private void UpdateAnswerNumbers()
    {
        List<int> answerList = new List<int>();

        while (answerList.Count < 4)
        {
            int randomNumber = Random.Range(0, 10);

            if (!answerList.Contains(randomNumber))
            {
                answerList.Add(randomNumber);
            }
        }

        answerNumbers = answerList.ToArray();
        for (int i = 0; i < answerNumbers.Length; i++)
        {
            Debug.Log(answerNumbers[i]);
        }
    }

    private void ResetLogTextPanels()
    {
        if (logTextPanels == null || logTextPanels.Count <= 0)
            return;

        foreach (LogTextPanel panel in logTextPanels)
        {
            panel.gameObject.SetActive(false);
        }
    }

    private void UpdateLogTextPanel(string previewNumber, int strikeCount, int ballCount, int outCount)
    {
        if (logTextPanels == null || logTextPanels.Count <= 0)
            return;

        LogTextPanel currentLogTextPanel;

        if (turnCount >= logTextPanels.Count - 1)
        {
            currentLogTextPanel = Instantiate(logTextPanel, logTextPanelContent);
            currentLogTextPanel.gameObject.SetActive(false);
            logTextPanels.Add(currentLogTextPanel);
        }
        else
        {
            currentLogTextPanel = logTextPanels[turnCount];
        }

        currentLogTextPanel.UpdateLogText(previewNumber, strikeCount, ballCount, outCount);
        currentLogTextPanel.gameObject.SetActive(true);
    }

    private void UpdateEnterButtonVisible()
    {
        enterButton.interactable = (currentIndex == MAX_BASEBALL_COUNT);
    }

    private void UpdateResultUI()
    {
        inGameUI?.SetActive(false);
        resultUI?.SetActive(true);

        string answerNumberText = null;
        for (int i = 0; i < answerNumbers.Length; i++)
        {
            answerNumberText += answerNumbers[i];
        }

        if (answerText != null)
        {
            answerText.text = answerNumberText;
        }
    }

    private bool GetIsAnswer()
    {
        for (int i = 0; i < MAX_BASEBALL_COUNT; i++)
        {
            if (answerNumbers[i] != previewNumbers[i])
                return false;
        }

        return true;
    }

    public void OnClickNumber(int number)
    {
        if (currentIndex >= previewTexts.Length)
            return;

        previewNumbers[currentIndex] = number;
        previewTexts[currentIndex].text = number.ToString();
        currentIndex++;

        UpdateEnterButtonVisible();
    }

    public void OnClickBack()
    {
        if (currentIndex <= 0)
            return;

        currentIndex--;
        previewNumbers[currentIndex] = -1;
        previewTexts[currentIndex].text = string.Empty;

        UpdateEnterButtonVisible();
    }

    public void OnClickEnter()
    {
        if (GetIsAnswer())
        {
            UpdateResultUI();
            return;
        }

        string previewNumberText = null;
        int strikeCount = 0;
        int ballCount = 0;
        int outCount = 0;

        for (int i = 0; i < previewNumbers.Length; i++)
        {
            previewNumberText += previewNumbers[i].ToString();
        }

        for (int i = 0; i < MAX_BASEBALL_COUNT; i++)
        {
            for (int j = 0; j < MAX_BASEBALL_COUNT; j++)
            {
                if (answerNumbers[i] == previewNumbers[i])
                {
                    strikeCount++;
                    break;
                }

                if (answerNumbers[i] == previewNumbers[j])
                {
                    ballCount++;
                    break;
                }

                if (j == MAX_BASEBALL_COUNT - 1)
                {
                    outCount++;
                }
            }
        }

        UpdateLogTextPanel(previewNumberText, strikeCount, ballCount, outCount);
        NextTurn();
    }
}
