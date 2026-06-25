using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Image questionImage;

    public Sprite[] images;

    public string[] answers;

    public DropZone dropZone;

    public TMP_Text feedbackText;

    private int currentQuestion = 0;

    void Start()
    {
        if (feedbackText != null)
            feedbackText.text = "";

        LoadQuestion();
    }

    public void CorrectAnswer()
    {
        if (feedbackText != null)
            feedbackText.text = "Correto!";

        currentQuestion++;

        if (currentQuestion < images.Length)
        {
            Invoke(nameof(LoadQuestion), 1f);
        }
        else
        {
            if (questionImage != null)
                questionImage.gameObject.SetActive(false);

            if (feedbackText != null)
                feedbackText.text = "MÓDULO FINALIZADO!";

            Debug.Log("Fim do jogo!");
        }
    }

    public void WrongAnswer()
    {
        if (feedbackText != null)
            feedbackText.text = "Incorreto!";
    }

    void LoadQuestion()
    {
        questionImage.sprite = images[currentQuestion];

        dropZone.correctLetter = answers[currentQuestion];
    }
}