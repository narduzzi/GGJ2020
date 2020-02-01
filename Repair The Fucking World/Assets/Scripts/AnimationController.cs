using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AnimationController : MonoBehaviour
{
    public Text Title;
    public Button FirstAnswerButton;
    public Button SecondAnswerButton;
    public Image Sprite;

    private void Awake()
    {
        TextController.Play = SceneManager.GetActiveScene().name != "MainMenuScene";
    }
    public void FadeOutUI()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        Color aColor = new Color(0f, 0f, 0f, 0.1f);
        for (float a = 1.0f; a > 0f; a -= 0.1f)
        {
            Title.color -= aColor;
            FirstAnswerButton.GetComponent<Image>().color -= aColor;
            FirstAnswerButton.GetComponentInChildren<Text>().color -= aColor;
            SecondAnswerButton.GetComponent<Image>().color -= aColor;
            SecondAnswerButton.GetComponentInChildren<Text>().color -= aColor;
            Sprite.color -= aColor;
            yield return new WaitForSeconds(0.1f);
        }

        ClearUI();

        Title.fontSize = 20;

        TextController.Title = "WORLD WAR III HAS BEEN DECLARED !\n\nYOU HAVE BEEN CHOSEN TO CHANGE THE COURSE OF TIME\n\nMAKE IT QUICK, THERE IS NO TIME FOR DREAMING !";
        TextController.FirstAnswer = "LET'S GO !";
        TextController.SecondAnswer = "";
        TextController.Play = true;
        FirstAnswerButton.onClick.RemoveAllListeners();

        FirstAnswerButton.onClick.AddListener(() =>
        {
            TextController.Title = "LOOK ! THE SPARK OF LIFE !\n\nWHAT SHOULD WE DO ?";
            TextController.FirstAnswer = "WATCH IT AS IF IT WAS A MOVIE";
            TextController.SecondAnswer = "BLOW IT LIKE A CANDLE !";
            SceneManager.LoadScene("GameScene");
        });
    }

    private void ClearUI()
    {
        Color aColor = new Color(0f, 0f, 0f, 1f);
        Title.text = "";
        Title.color += aColor;
        FirstAnswerButton.GetComponentInChildren<Text>().text = "";
        FirstAnswerButton.GetComponent<Image>().color += aColor;
        FirstAnswerButton.GetComponentInChildren<Text>().color += aColor;
        SecondAnswerButton.GetComponentInChildren<Text>().text = "";
        SecondAnswerButton.GetComponent<Image>().color += aColor;
        SecondAnswerButton.GetComponentInChildren<Text>().color += aColor;
    }
}
