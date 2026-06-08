using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseballGameManager : MonoBehaviour
{
    private const int DEFAULT_MAX_NUMBER_COUNT = 4;

    [Header("Game Rule")]
    [SerializeField] private int numberCount = DEFAULT_MAX_NUMBER_COUNT;

    [Header("UI Panel")]
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject resultUI;

    [Header("InGameUI")]

    [Header("Log Panel")]
    [SerializeField] private RectTransform logTextPanelContent;
    [SerializeField] private LogTextPanel logTextPanelPrefab;

    [Header("PreviewText Panel")]
    [SerializeField] private RectTransform previewTextPanelContent;
    [SerializeField] private PreviewTextPanel previewTextPanelPrefab;

    [Header("Other")]
    [SerializeField] private TMP_Text turnCountText;
    [SerializeField] private Button enterButton;

    [Header("ResultUI")]
    [SerializeField] private TMP_Text answerText;


    private List<LogTextPanel> logTextPanels = new List<LogTextPanel>();
    private List<PreviewTextPanel> previewTextPanels = new List<PreviewTextPanel>();

    private string answerNumberText = string.Empty;

    private int[] answerNumbers;
    private int[] previewNumbers;

    private int turnCount;
    private int currentIndex;


    private void Start()
    {
        if (!TryPreviewTextPanelsInitialized())
        {
            Debug.LogError("PreviewTextPanels를 초기화 하는 과정 중 오류가 발생하였습니다.");
            return;
        }

        if (!TryLogTextPanelsInitialized())
        {
            Debug.LogError("LogTextPanels를 초기화 하는 과정 중 오류가 발생하였습니다.");
            return;
        }

        answerNumbers = new int[numberCount];
        previewNumbers = new int[numberCount];
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

        for (int i = 0; i < numberCount; i++)
        {
            previewNumbers[i] = -1;
            previewTextPanels[i].ResetPreviewText();
        }

        turnCount++;
        turnCountText.text = $"Turn : {turnCount}";
    }

    private bool TryPreviewTextPanelsInitialized()
    {
        if (previewTextPanelContent == null)
        {
            Debug.LogError("PreviewTextPanel을 추가할 Content 영역이 설정되지 않았습니다.");
            return false;
        }

        if (previewTextPanelPrefab == null)
        {
            Debug.LogError("PreviewTextPanel Prefab이 없습니다.");
            return false;
        }

        // Preview Text Panels 초기화
        if (previewTextPanels == null)
            previewTextPanels = new List<PreviewTextPanel>();

        // numberCount 갯수 만큼 PreviewTextPanel 생성
        for (int i = 0; i < numberCount; i++)
        {
            PreviewTextPanel newPreviewTextPanel = Instantiate(previewTextPanelPrefab, previewTextPanelContent);
            newPreviewTextPanel.ResetPreviewText();
            previewTextPanels.Add(newPreviewTextPanel);
        }

        return true;
    }

    private bool TryLogTextPanelsInitialized()
    {
        if (logTextPanelContent == null)
        {
            Debug.LogError("LogTextPanel을 추가할 Content 영역이 설정되지 않았습니다.");
            return false;
        }

        if (logTextPanelPrefab = null)
        {
            Debug.LogError("LogTextPanel Prefab이 없습니다.");
            return false;
        }

        if (logTextPanels == null)
            logTextPanels = new List<LogTextPanel>();

        logTextPanels = logTextPanelContent.GetComponentsInChildren<LogTextPanel>().ToList();
        return true;
    }

    private void UpdateAnswerNumbers()
    {
        List<int> answerList = new List<int>();
        answerNumberText = string.Empty;

        while (answerList.Count < numberCount)
        {
            int randomNumber = Random.Range(0, 10);

            if (!answerList.Contains(randomNumber))
            {
                answerNumberText += randomNumber.ToString();
                answerList.Add(randomNumber);
            }
        }

        answerNumbers = answerList.ToArray();
        Debug.Log("Answer : " + answerNumberText);
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
            currentLogTextPanel = Instantiate(logTextPanelPrefab, logTextPanelContent);
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
        enterButton.interactable = (currentIndex == numberCount);
    }

    private void UpdateResultUI()
    {
        inGameUI?.SetActive(false);
        resultUI?.SetActive(true);

        if (answerText != null)
        {
            answerText.text = answerNumberText;
        }
    }

    private bool GetIsAnswer()
    {
        for (int i = 0; i < numberCount; i++)
        {
            if (answerNumbers[i] != previewNumbers[i])
                return false;
        }

        return true;
    }

    public void OnClickNumber(int number)
    {
        if (currentIndex >= numberCount)
            return;

        previewNumbers[currentIndex] = number;
        previewTextPanels[currentIndex].UpdatePrivewText(number);
        currentIndex++;

        UpdateEnterButtonVisible();
    }

    public void OnClickBack()
    {
        if (currentIndex <= 0)
            return;

        currentIndex--;
        previewNumbers[currentIndex] = -1;
        previewTextPanels[currentIndex].ResetPreviewText();

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

        for (int i = 0; i < numberCount; i++)
        {
            for (int j = 0; j < numberCount; j++)
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

                if (j == numberCount - 1)
                {
                    outCount++;
                }
            }
        }

        UpdateLogTextPanel(previewNumberText, strikeCount, ballCount, outCount);
        NextTurn();
    }

    public void OnClickReStartButton()
    {
        OnStartGame();
    }
}
