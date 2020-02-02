using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AnimationController : MonoBehaviour
{
    public Text TitleText;
    public Button FirstAnswerButton;
    public Button SecondAnswerButton;
    public Image AnimatedSprite;

    public AudioClip[] KeyboardSFX;
    public AudioSource KeyboardAudio;

    private string Title;
    private string FirstAnswer;
    private string SecondAnswer;

    private void Awake()
    {
        FirstAnswerButton.onClick.AddListener(() =>
        {
            IntroductionText();
        });

        SecondAnswerButton.onClick.AddListener(() =>
        {
            Quit();
        });
    }

    public void NextQuestion()
    {
        StartCoroutine(AskServer());

        /*
        Title = "LOOK ! THE SPARK OF LIFE !\n\nWHAT SHOULD WE DO ?";
        FirstAnswer = "WATCH IT AS IF IT WAS A MOVIE";
        SecondAnswer = "BLOW IT LIKE A CANDLE !";
        FadeOutUI();
        */
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void IntroductionText()
    {
        Title = "WORLD WAR III HAS BEEN DECLARED !\n\nYOU HAVE BEEN CHOSEN TO CHANGE THE COURSE OF TIME\n\nMAKE IT QUICK, THERE IS NO TIME FOR DREAMING !";
        FirstAnswer = "LET'S GO !";
        SecondAnswer = "NOT MY PROBLEM...";
        FadeOutUI();
    }

    private void FadeOutUI()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator AskServer()
    {
        float test = Time.time;
        yield return NetworkManager.SendRequest();
        Debug.Log(Time.time - test);
    }

    private IEnumerator FadeOut()
    {
        Color aColor = new Color(0f, 0f, 0f, 0.1f);
        for (float a = 1.0f; a > 0f; a -= 0.1f)
        {
            TitleText.color -= aColor;
            FirstAnswerButton.GetComponent<Image>().color -= aColor;
            FirstAnswerButton.GetComponentInChildren<Text>().color -= aColor;
            SecondAnswerButton.GetComponent<Image>().color -= aColor;
            SecondAnswerButton.GetComponentInChildren<Text>().color -= aColor;
            AnimatedSprite.color -= aColor;
            yield return new WaitForSeconds(0.1f);
        }

        FirstAnswerButton.gameObject.SetActive(false);
        SecondAnswerButton.gameObject.SetActive(false);

        TitleText.fontSize = 20;
        ClearUI();

        yield return PrintAll();

        FirstAnswerButton.onClick.RemoveAllListeners();

        FirstAnswerButton.onClick.AddListener(() =>
        {
            NextQuestion();
        });

        SecondAnswerButton.onClick.RemoveAllListeners();

        SecondAnswerButton.onClick.AddListener(() =>
        {
            IntroductionText();
        });

    }

    private void ClearUI()
    {
        Color aColor = new Color(0f, 0f, 0f, 1f);
        TitleText.text = "";
        TitleText.color += aColor;
        FirstAnswerButton.GetComponentInChildren<Text>().text = "";
        FirstAnswerButton.GetComponent<Image>().color += aColor;
        FirstAnswerButton.GetComponentInChildren<Text>().color += aColor;
        SecondAnswerButton.GetComponentInChildren<Text>().text = "";
        SecondAnswerButton.GetComponent<Image>().color += aColor;
        SecondAnswerButton.GetComponentInChildren<Text>().color += aColor;
    }

    public void WritingAnimation()
    {
        StartCoroutine(PrintAll());
    }

    private IEnumerator PrintAll()
    {
        KeyboardAudio.Play();
        yield return PrintText(TitleText, Title);
        yield return new WaitForSeconds(0.5f);

        if (FirstAnswer != "")
        {
            FirstAnswerButton.gameObject.SetActive(true);
        }

        yield return PrintText(FirstAnswerButton.GetComponentInChildren<Text>(), FirstAnswer);
        yield return new WaitForSeconds(0.3f);

        if (SecondAnswer != "")
        {
            SecondAnswerButton.gameObject.SetActive(true);
        }
        yield return PrintText(SecondAnswerButton.GetComponentInChildren<Text>(), SecondAnswer);

        KeyboardAudio.Stop();
    }

    private IEnumerator PrintText(Text textComponent, string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            KeyboardAudio.clip = KeyboardSFX[Random.Range(0, KeyboardSFX.Length)];
            KeyboardAudio.Play();

            if (text[i] == ' ')
            {
                yield return new WaitForSeconds(Random.Range(0f, 0.2f));
            }

            if (text[i] == '\n')
            {
                yield return new WaitForSeconds(0.25f);
            }

            textComponent.text = string.Concat(textComponent.text, text[i]);
            //Wait a certain amount of time, then continue with the for loop
            yield return new WaitForSeconds(0.08f);
        }
    }
}
