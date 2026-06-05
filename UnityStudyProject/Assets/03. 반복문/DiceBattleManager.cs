using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class DiceBattleManager : MonoBehaviour
{
    [Header("UI Panel")]
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject resultUI;

    [Header("InGame UI")]
    [Header("Player")]
    [SerializeField] private int playerMaxHp;
    [SerializeField] private TMP_Text[] playerDiceTexts;
    [SerializeField] private TMP_Text playerHpText;
    [SerializeField] private TMP_Text playerResultText;
    [SerializeField] private TMP_Text playerTakeDemageText;

    [Header("Computer")]
    [SerializeField] private int computerMaxHp;
    [SerializeField] private TMP_Text[] computerDiceTexts;
    [SerializeField] private TMP_Text computerHpText;
    [SerializeField] private TMP_Text computerResultText;
    [SerializeField] private TMP_Text computerTakeDemageText;

    [Header("Result UI")]
    [SerializeField] private TMP_Text resultText;

    [Header("TakeDemage Tween")]
    [SerializeField] private float takeDemageTextDuration = 1f;
    [SerializeField] private float takeDemageMoveValue = 100f;
    [SerializeField] private Ease takeDemageTextEase = Ease.OutQuart;

    private int[] computerDiceNumbers;
    private int[] playerDiceNumbers;

    private int playerCurrentHp;
    private int computerCurrentHp;

    private int playerResult;
    private int computerResult;

    private Vector3 startPlayerTakeDemageTextPos;
    private Vector3 startComputerTakeDemageTextPos;

    private Tween takeDemageTextTween;


    private void Start()
    {
        computerDiceNumbers = new int[computerDiceTexts.Length];
        playerDiceNumbers = new int[playerDiceTexts.Length];

        startPlayerTakeDemageTextPos = playerTakeDemageText.transform.localPosition;
        startComputerTakeDemageTextPos = computerTakeDemageText.transform.localPosition;

        StartGame();
    }

    public void StartGame()
    {
        inGameUI.SetActive(true);
        resultUI.SetActive(false);

        playerCurrentHp = playerMaxHp;
        computerCurrentHp = computerMaxHp;

        playerResult = 0;
        computerResult = 0;

        takeDemageTextTween?.Kill();
        takeDemageTextTween = null;

        // HP Text 초기화
        UpdatePlayerHpText();
        UpdateComputerHpText();

        // TakeDemageText 초기화
        ResetTakeDemageText();

        // Result Text 초기화
        if (playerResultText != null)
        {
            playerResultText.text = playerResult.ToString();
        }
        if (computerResultText != null)
        {
            computerResultText.text = computerResult.ToString();
        }

        // Dice Text 초기화
        for (int i = 0; i < playerDiceTexts.Length; i++)
        {
            playerDiceTexts[i].text = "0";
        }

        for (int i = 0; i < computerDiceTexts.Length; i++)
        {
            computerDiceTexts[i].text = "0";
        }
    }

    private void ResultBattle(bool isWin)
    {
        inGameUI.SetActive(false);
        resultUI.SetActive(true);

        if (resultText != null)
        {
            resultText.text = (isWin) ? "Win!!" : "Lose..";
        }
    }

    private void UpdatePlayerHpText()
    {
        if (playerHpText == null)
            return;

        playerHpText.text = $"{playerCurrentHp}/{playerMaxHp}";
    }

    private void UpdateComputerHpText()
    {
        if (computerHpText == null)
            return;

        computerHpText.text = $"{computerCurrentHp}/{computerMaxHp}";
    }

    private void ResetTakeDemageText()
    {
        if (playerTakeDemageText != null)
        {
            playerTakeDemageText.transform.localPosition = startPlayerTakeDemageTextPos;
            playerTakeDemageText.color = Color.clear;
            playerTakeDemageText.text = string.Empty;
        }

        if (computerTakeDemageText != null)
        {
            computerTakeDemageText.transform.localPosition = startComputerTakeDemageTextPos;
            computerTakeDemageText.color = Color.clear;
            computerTakeDemageText.text = string.Empty;
        }
    }

    private void PlayTakeDemageAnim(TMP_Text takeDemageText, Vector3 startPos, int takeDemage)
    {
        ResetTakeDemageText();

        takeDemageText.color = Color.white;
        takeDemageText.text = $"-{takeDemage}";

        Vector3 targetPos = startPos + (Vector3.up * takeDemageMoveValue);

        takeDemageTextTween?.Kill();
        takeDemageTextTween = null;

        takeDemageTextTween = DOTween.Sequence()
            .Append(
                takeDemageText.transform
                    .DOLocalMove(targetPos, takeDemageTextDuration)
                    .SetEase(takeDemageTextEase)
            )
            .Append(
                takeDemageText
                    .DOColor(Color.clear, takeDemageTextDuration)
            );
    }

    private int GetCriticalMultiplier(int[] diceNubmers)
    {
        if ((diceNubmers[0] == diceNubmers[1]) && (diceNubmers[1] == diceNubmers[2]))
        {
            return 3;
        }
        else if ((diceNubmers[0] == diceNubmers[1]) ||
            diceNubmers[0] == diceNubmers[2] ||
            diceNubmers[1] == diceNubmers[2])
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }

    public void OnClickBattle()
    {
        playerResult = 0;
        computerResult = 0;

        // 플레이어 다이스 굴리기
        for (int i = 0; i < playerDiceTexts.Length; i++)
        {
            int randomDiceNumber = Random.Range(1, 7);
            playerDiceNumbers[i] = randomDiceNumber;
            playerDiceTexts[i].text = randomDiceNumber.ToString();
            playerResult += randomDiceNumber;
        }

        // 플레이어 치명타 적용
        playerResult *= GetCriticalMultiplier(playerDiceNumbers);


        // 컴퓨터 다이스 굴리기
        for (int i = 0; i < computerDiceTexts.Length; i++)
        {
            int randomDiceNumber = Random.Range(1, 7);
            computerDiceNumbers[i] = randomDiceNumber;
            computerDiceTexts[i].text = randomDiceNumber.ToString();
            computerResult += randomDiceNumber;
        }

        // 컴퓨터 치명타 적용
        computerResult *= GetCriticalMultiplier(computerDiceNumbers);


        if (playerResultText != null)
        {
            playerResultText.text = playerResult.ToString();
        }

        if (computerResultText != null)
        {
            computerResultText.text = computerResult.ToString();
        }


        if (playerResult > computerResult)
        {
            int demage = playerResult - computerResult;
            computerCurrentHp -= demage;

            if (computerCurrentHp <= 0)
            {
                computerCurrentHp = 0;
                ResultBattle(true);
            }

            PlayTakeDemageAnim(computerTakeDemageText, startComputerTakeDemageTextPos, demage);
            UpdateComputerHpText();
        }
        else if (playerResult < computerResult)
        {
            int demage = computerResult - playerResult;
            playerCurrentHp -= demage;

            if (playerCurrentHp <= 0)
            {
                playerCurrentHp = 0;
                ResultBattle(false);
            }

            PlayTakeDemageAnim(playerTakeDemageText, startPlayerTakeDemageTextPos, demage);
            UpdatePlayerHpText();
        }
        else
        {
            Debug.Log("같은 숫자");
        }
    }

    public void OnClickReStart()
    {
        StartGame();
    }
}
