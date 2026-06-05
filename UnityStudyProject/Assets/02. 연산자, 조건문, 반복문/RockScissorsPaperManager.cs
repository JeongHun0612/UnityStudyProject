using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RockScissorsPaperManager : MonoBehaviour
{
    public enum EChoice
    {
        Rock,
        Scissors,
        Paper
    }

    public enum EResult
    {
        Win,
        Lose,
        Draw
    }

    [Header("UI Panel")]
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject resultUI;

    [Header("Player Choice Panel")]
    [SerializeField] private TMP_Text playerChoiceText;
    [SerializeField] private Image playerChoiceImage;

    [Header("Computer Choice Panel")]
    [SerializeField] private TMP_Text computerChoiceText;
    [SerializeField] private Image computerChoiceImage;

    [Header("Result")]
    [SerializeField] private TMP_Text resultText;

    [Header("Choice Sprite")]
    [SerializeField] private Sprite rockSprite;
    [SerializeField] private Sprite scissorsSprite;
    [SerializeField] private Sprite paperSprite;

    private EChoice playerChoice;
    private EChoice computerChoice;

    private void Start()
    {
        OnGameStart();
    }

    public void OnGameStart()
    {
        inGameUI.SetActive(true);
        resultUI.SetActive(false);

        playerChoiceText.text = string.Empty;
        computerChoiceText.text = string.Empty;
        resultText.text = string.Empty;
    }

    public void OnGameResult()
    {
        inGameUI.SetActive(false);
        resultUI.SetActive(true);

        computerChoice = (EChoice)Random.Range(0, 3);

        EResult result = GetResult(playerChoice, computerChoice);

        // UI ¹Ý¿µ
        playerChoiceImage.sprite = GetSpriteFromChoice(playerChoice);
        playerChoiceText.text = playerChoice.ToString();

        computerChoiceImage.sprite = GetSpriteFromChoice(computerChoice);
        computerChoiceText.text = computerChoice.ToString();

        resultText.text = GetResultText(result);
    }

    //public EResult GetResult(EChoice playerChoice, EChoice computerChoice)
    //{
    //    if (playerChoice == EChoice.Rock)
    //    {
    //        if (computerChoice == EChoice.Rock)
    //            return EResult.Draw;
    //        else if (computerChoice == EChoice.Scissors)
    //            return EResult.Win;
    //        else if (computerChoice == EChoice.Paper)
    //            return EResult.Lose;
    //    }
    //    else if (playerChoice == EChoice.Scissors)
    //    {
    //        if (computerChoice == EChoice.Rock)
    //            return EResult.Lose;
    //        else if (computerChoice == EChoice.Scissors)
    //            return EResult.Draw;
    //        else if (computerChoice == EChoice.Paper)
    //            return EResult.Win;
    //    }
    //    else if (playerChoice == EChoice.Paper)
    //    {
    //        if (computerChoice == EChoice.Rock)
    //            return EResult.Win;
    //        else if (computerChoice == EChoice.Scissors)
    //            return EResult.Lose;
    //        else if (computerChoice == EChoice.Paper)
    //            return EResult.Draw;
    //    }

    //    return EResult.Draw;
    //}

    private EResult GetResult(EChoice playerChoice, EChoice computerChoice)
    {
        int playerChoiceNum = (int)playerChoice;
        int computerChoiceNum = (int)computerChoice;

        if (playerChoiceNum == computerChoiceNum)
        {
            return EResult.Draw;
        }
        else if ((playerChoiceNum + 1) % 3 == computerChoiceNum)
        {
            return EResult.Win;
        }
        else if ((playerChoiceNum + 2) % 3 == computerChoiceNum)
        {
            return EResult.Lose;
        }

        return EResult.Draw;
    }

    private string GetResultText(EResult result)
    {
        switch (result)
        {
            case EResult.Win:
                return "Win!!";
            case EResult.Lose:
                return "Lose..";
            case EResult.Draw:
                return "Draw";
            default:
                return "Draw";
        }
    }

    private Sprite GetSpriteFromChoice(EChoice choice)
    {
        switch (choice)
        {
            case EChoice.Rock:
                return rockSprite;
            case EChoice.Scissors:
                return scissorsSprite;
            case EChoice.Paper:
                return paperSprite;
            default:
                return null;
        }
    }

    public void OnClickRock()
    {
        playerChoice = EChoice.Rock;
        OnGameResult();
    }

    public void OnClickScissors()
    {
        playerChoice = EChoice.Scissors;
        OnGameResult();
    }

    public void OnClickPaper()
    {
        playerChoice = EChoice.Paper;
        OnGameResult();
    }

    public void OnClickRetry()
    {
        OnGameStart();
    }
}
