using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AnimationController : MonoBehaviour
{
    public Text TitleText;
    public Button FirstAnswerButton;
    public Button SecondAnswerButton;
    public GameObject AnimatedSprite;

    public AudioClip[] KeyboardSFX;
    public AudioClip[] ButtonSFX;
    public AudioSource KeyboardAudio;
    public AudioSource ButtonAudio;
    public AudioSource MainMusic;

    public GameObject Background;
    public GameObject TransitionScreen;

    public static Event CurrentEvent;

    private string Title;
    private string FirstAnswer;
    private string SecondAnswer;

    private float writingSpeed;
    private int numberOfEvents;

    void Awake()
    {
        numberOfEvents = 0;
        writingSpeed = 1f;
        CurrentEvent = EventsGenerator.FirstEvent();

        SetQuestionText();

        FirstAnswerButton.onClick.RemoveAllListeners();
        FirstAnswerButton.onClick.AddListener(() =>
        {
            IntroductionText();
        });

        SecondAnswerButton.onClick.RemoveAllListeners();
        SecondAnswerButton.onClick.AddListener(() =>
        {
            Quit();
        });
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            writingSpeed = 5f;
        }
    }

    private void SetQuestionText()
    {
        Title = CurrentEvent.Question;
        FirstAnswer = CurrentEvent.Answers[0].Item1;
        SecondAnswer = CurrentEvent.Answers[1].Item1;
    }

    public void NextQuestion(int questionID)
    {
        StartCoroutine(AskServer(questionID));
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void IntroductionText()
    {
        FirstAnswerButton.onClick.RemoveAllListeners();

        FirstAnswerButton.onClick.AddListener(() =>
        {
            PlayRandomSFX();
            HandleResponse(CurrentEvent.Answers[0]);
        });

        SecondAnswerButton.onClick.RemoveAllListeners();

        SecondAnswerButton.onClick.AddListener(() =>
        {
            PlayRandomSFX();
            HandleResponse(CurrentEvent.Answers[1]);
        });

        StartCoroutine(HideAndDisplay());
    }

    private IEnumerator HideAndDisplay()
    {
        yield return FadeOut();
        yield return DisplayText();
    }

    private IEnumerator AskServer(int questionID)
    {
        yield return NetworkManager.NextQuestion(questionID);

        SetQuestionText();

        yield return new WaitForSeconds(0.1f);
        yield return HideAndDisplay();
    }

    private void HandleResponse(Tuple<string, int, string> answer)
    {
        StartCoroutine(AsyncHandler(answer));
    }

    private IEnumerator AsyncHandler(Tuple<string, int, string> answer)
    {
        int nextQuestionID = answer.Item2;

        if (nextQuestionID < 0)
        {
            switch (nextQuestionID)
            {
                // NATURE
                case -2:
                    yield return ChangeBackground(ImageManager._NatureBackground, true, null);
                    break;

                // DICTATEUR
                case -3:
                    yield return ChangeBackground(ImageManager._DictatorBackground, true, null);
                    break;

                // Chaos
                case -1:
                    yield return ChangeBackground(ImageManager._ChaosBackground, true, null);
                    break;

                // Explosion
                case -4:
                    yield return ChangeBackground(ImageManager._ExplosionBackground, true, ImageManager._ExplosionAnimation);
                    break;

                case -5:
                case -8:
                    yield return ChangeBackground(ImageManager._UtopiaBackground, true, null);
                    break;

            }
        }
        else
        {
            if (numberOfEvents == 1)
            {
                yield return BigBang();
            }
        }

        Title = answer.Item3;
        FirstAnswer = "";
        SecondAnswer = "";

        // Affiche le résultat
        yield return HideAndDisplay();
        yield return new WaitForSeconds(2f);

        if (nextQuestionID < 0)
        {
            yield return new WaitForSeconds(3f);
            if (nextQuestionID != -5 && nextQuestionID != -8)
            {
                CurrentEvent = EventsGenerator.FirstEvent();
                yield return ResetMenu();
            }
            else
            {
                yield return DisplayCredits();
            }
        }
        else
        {
            if (numberOfEvents == 0)
            {
                yield return ChangeBackground(ImageManager._BlackBackground, false, null);
            }

            if (numberOfEvents == 7)
            {
                yield return ChangeBackground(ImageManager._Antique, false, null);
            }

            if (numberOfEvents == 13)
            {
                yield return ChangeBackground(ImageManager._Medieval, false, null);
            }

            if (numberOfEvents == 15)
            {
                yield return ChangeBackground(ImageManager._Industry, false, null);
            }

            if (numberOfEvents == 16)
            {
                yield return ChangeBackground(ImageManager._Current, false, null);
            }

            numberOfEvents++;
            NextQuestion(answer.Item2);
        }
    }

    private IEnumerator ResetMenu()
    {
        TransitionScreen.SetActive(true);
        Color aColor = new Color(0f, 0f, 0f, 0.1f);
        for (float a = 0f; a < 1f; a += 0.1f)
        {
            Background.GetComponent<AudioSource>().volume -= 0.1f;
            TransitionScreen.GetComponent<Image>().color += aColor;
            yield return new WaitForSeconds(0.1f);
        }

        ClearUI();

        Destroy(Background);
        Background = Instantiate(ImageManager._Chaos);

        Destroy(AnimatedSprite);
        AnimatedSprite = Instantiate(ImageManager._PortalAnimation, FindObjectOfType<Canvas>().transform);
        AnimatedSprite.GetComponent<Image>().color += new Color(0f, 0f, 0f, 1f);

        TitleText.fontSize = 80;
        TitleText.text = "REPAIR THE F*CKING W RLD";

        FirstAnswerButton.GetComponentInChildren<Text>().text = "ALLONS SAUVER LE MONDE!";
        SecondAnswerButton.GetComponentInChildren<Text>().text = "SANS MOI!";

        FirstAnswerButton.gameObject.SetActive(true);
        SecondAnswerButton.gameObject.SetActive(true);

        for (float a = 1.0f; a > 0f; a -= 0.1f)
        {
            TransitionScreen.GetComponent<Image>().color -= aColor;
            yield return new WaitForSeconds(0.1f);
        }

        TransitionScreen.SetActive(false);

        MainMusic.Play();

        Awake();
    }

    private IEnumerator BigBang()
    {
        Destroy(AnimatedSprite);
        AnimatedSprite = Instantiate(ImageManager._BigBangAnimation, FindObjectOfType<Canvas>().transform);
        AnimatedSprite.GetComponent<Image>().color += new Color(0f, 0f, 0f, 1f);
        yield return new WaitForSeconds(6f);
    }

    private IEnumerator DisplayCredits()
    {
        yield return null;
    }

    private IEnumerator ChangeBackground(GameObject backgroundPrefab, bool isEnd, GameObject animationSprite)
    {
        TransitionScreen.SetActive(true);
        Color aColor = new Color(0f, 0f, 0f, 0.1f);
        for (float a = 0f; a < 1f; a += 0.1f)
        {
            TransitionScreen.GetComponent<Image>().color += aColor;
            if (isEnd)
            {
                MainMusic.volume -= 0.1f;
            }
            yield return new WaitForSeconds(0.1f);
        }

        if (isEnd)
        {
            MainMusic.Stop();
            MainMusic.volume = 1.0f;
        }

        ClearUI();
        Destroy(Background);
        Background = Instantiate(backgroundPrefab);

        Destroy(AnimatedSprite);
        if (animationSprite != null)
        {
            AnimatedSprite = Instantiate(animationSprite, FindObjectOfType<Canvas>().transform);
            AnimatedSprite.GetComponent<Image>().color += new Color(0f, 0f, 0f, 1f);
        }

        for (float a = 1.0f; a > 0f; a -= 0.1f)
        {
            TransitionScreen.GetComponent<Image>().color -= aColor;
            yield return new WaitForSeconds(0.1f);
        }

        TransitionScreen.SetActive(false);
    }

    private IEnumerator FadeOutTexts()
    {
        Color aColor = new Color(0f, 0f, 0f, 0.1f);
        for (float a = 1.0f; a > 0f; a -= 0.1f)
        {
            TitleText.color -= aColor;
            FirstAnswerButton.GetComponent<Image>().color -= aColor;
            FirstAnswerButton.GetComponentInChildren<Text>().color -= aColor;
            SecondAnswerButton.GetComponent<Image>().color -= aColor;
            SecondAnswerButton.GetComponentInChildren<Text>().color -= aColor;

            if (AnimatedSprite != null)
            {
                AnimatedSprite.GetComponent<Image>().color -= aColor;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator FadeOut()
    {
        yield return FadeOutTexts();

        TitleText.fontSize = 20;
        ClearUI();
    }

    private IEnumerator DisplayText()
    {
        yield return PrintAll();
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

        FirstAnswerButton.gameObject.SetActive(false);
        SecondAnswerButton.gameObject.SetActive(false);
    }

    public void WritingAnimation()
    {
        StartCoroutine(PrintAll());
    }

    private IEnumerator PrintAll()
    {
        KeyboardAudio.Play();
        yield return PrintText(TitleText, Title);
        yield return new WaitForSeconds(0.5f / writingSpeed);

        if (FirstAnswer != "")
        {
            FirstAnswerButton.gameObject.SetActive(true);
        }

        yield return PrintText(FirstAnswerButton.GetComponentInChildren<Text>(), FirstAnswer);
        yield return new WaitForSeconds(0.3f / writingSpeed);

        if (SecondAnswer != "")
        {
            SecondAnswerButton.gameObject.SetActive(true);
        }
        yield return PrintText(SecondAnswerButton.GetComponentInChildren<Text>(), SecondAnswer);

        KeyboardAudio.Stop();
        writingSpeed = 1f;
    }

    private void PlayRandomSFX()
    {
        ButtonAudio.clip = ButtonSFX[UnityEngine.Random.Range(0, ButtonSFX.Length)];
        ButtonAudio.Play();
    }

    private IEnumerator PrintText(Text textComponent, string text)
    {
        FirstAnswerButton.interactable = false;
        SecondAnswerButton.interactable = false;

        for (int i = 0; i < text.Length; i++)
        {
            KeyboardAudio.clip = KeyboardSFX[UnityEngine.Random.Range(0, KeyboardSFX.Length)];
            KeyboardAudio.Play();

            if (text[i] == ' ')
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 0.2f) / writingSpeed);
                textComponent.text = string.Concat(textComponent.text, text[i]);
            }
            else if (text[i] == '\\' && text[i + 1] == 'n')
            {
                yield return new WaitForSeconds(0.25f / writingSpeed);
                textComponent.text = string.Concat(textComponent.text, "\n");
                i++;
            }
            else
            {
                textComponent.text = string.Concat(textComponent.text, text[i]);
            }

            //Wait a certain amount of time, then continue with the for loop
            yield return new WaitForSeconds(0.08f / writingSpeed);
        }

        FirstAnswerButton.interactable = true;
        SecondAnswerButton.interactable = true;
    }
}
