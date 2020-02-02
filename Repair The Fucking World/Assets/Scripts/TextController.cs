using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextController : MonoBehaviour
{
    public static bool Play = true;
    public static string Title;
    public static string FirstAnswer;
    public static string SecondAnswer;

    public Text QuestionText;
    public Button FirstAnswerButton;
    public Button SecondAnswerButton;

    private Text FirstAnswerText;
    private Text SecondAnswerText;

    void Start()
    {
        FirstAnswerText = FirstAnswerButton.GetComponentInChildren<Text>();
        SecondAnswerText = SecondAnswerButton.GetComponentInChildren<Text>();
    }

    private void Update()
    {
        if (Play)
        {
            FirstAnswerButton.gameObject.SetActive(false);
            SecondAnswerButton.gameObject.SetActive(false);
            WritingAnimation();
            Play = false;
        }
    }
    public void WritingAnimation()
    {
        StartCoroutine(PrintAll());
    }

    private IEnumerator PrintAll()
    {
        yield return PrintText(QuestionText, Title);
        yield return new WaitForSeconds(0.5f);

        if (FirstAnswer != "")
        {
            FirstAnswerButton.gameObject.SetActive(true);
        }

        yield return PrintText(FirstAnswerText, FirstAnswer);
        yield return new WaitForSeconds(0.3f);

        if (SecondAnswer != "")
        {
            SecondAnswerButton.gameObject.SetActive(false);
        }
        yield return PrintText(SecondAnswerText, SecondAnswer);
    }

    private IEnumerator PrintText(Text textComponent, string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            textComponent.text = string.Concat(textComponent.text, text[i]);
            //Wait a certain amount of time, then continue with the for loop
            yield return new WaitForSeconds(0.1f);
        }
    }
}
