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

    [Header("Áudio")]
    public AudioSource audioSource;
    public AudioClip somAcerto;
    public AudioClip somErro;

    [Header("Botão de Reiniciar")]
    [Tooltip("Botão de reiniciar — fica escondido durante o jogo, só aparece ao finalizar o módulo")]
    public GameObject botaoReiniciar;

    [Header("Sistema de Estrelas")]
    [Tooltip("Componente EstrelasUI que mostra o resultado final")]
    public EstrelasUI telaDeEstrelas;

    private int currentQuestion = 0;
    private bool acertouSemErrarNestaPergunta = true;
    private int estrelasConquistadas = 0;

    void Start()
    {
        if (feedbackText != null)
            feedbackText.text = "";

        if (botaoReiniciar != null)
            botaoReiniciar.SetActive(false);

        LoadQuestion();
    }

    public void CorrectAnswer()
    {
        if (feedbackText != null)
            feedbackText.text = "Correto!";

        TocarSom(true);

        if (acertouSemErrarNestaPergunta)
            estrelasConquistadas++;

        currentQuestion++;

        Debug.Log($"CorrectAnswer chamado. currentQuestion agora é {currentQuestion} de {images.Length}.");

        if (currentQuestion < images.Length)
        {
            Invoke(nameof(LoadQuestion), 1f);
        }
        else
        {
            Debug.Log("Chegou na última pergunta! Tentando mostrar a tela de estrelas...");

            if (questionImage != null)
                questionImage.gameObject.SetActive(false);

            if (feedbackText != null)
                feedbackText.text = "";

            if (botaoReiniciar != null)
            {
                botaoReiniciar.SetActive(true);
                Debug.Log("botaoReiniciar ativado.");
            }
            else
            {
                Debug.LogWarning("botaoReiniciar está NULO — não foi arrastado no Inspector do GameManager.");
            }

            if (telaDeEstrelas != null)
            {
                Debug.Log($"Chamando MostrarResultado({estrelasConquistadas}, {images.Length})");
                telaDeEstrelas.MostrarResultado(estrelasConquistadas, images.Length);
            }
            else
            {
                Debug.LogWarning("telaDeEstrelas está NULA — não foi arrastada no Inspector do GameManager.");
            }

            Debug.Log("Fim do jogo!");
        }
    }

    public void WrongAnswer()
    {
        if (feedbackText != null)
            feedbackText.text = "Incorreto!";

        TocarSom(false);

        acertouSemErrarNestaPergunta = false;
    }

    void LoadQuestion()
    {
        if (questionImage == null)
        {
            Debug.LogError("questionImage não foi atribuído no Inspector do GameManager!");
            return;
        }

        if (images == null || images.Length == 0)
        {
            Debug.LogError("O array 'images' está vazio! Arraste as imagens no Inspector do GameManager.");
            return;
        }

        if (answers == null || answers.Length == 0)
        {
            Debug.LogError("O array 'answers' está vazio! Preencha as respostas no Inspector do GameManager.");
            return;
        }

        if (currentQuestion >= images.Length || currentQuestion >= answers.Length)
        {
            Debug.LogError($"currentQuestion ({currentQuestion}) está fora dos limites dos arrays. " +
                            $"images tem {images.Length} itens, answers tem {answers.Length} itens. Eles precisam ter o mesmo tamanho.");
            return;
        }

        if (dropZone == null)
        {
            Debug.LogError("dropZone não foi atribuído no Inspector do GameManager!");
            return;
        }

        questionImage.sprite = images[currentQuestion];

        dropZone.correctLetter = answers[currentQuestion];

        acertouSemErrarNestaPergunta = true;
    }

    /// <summary>
    /// Toca o som de acerto ou erro, se o AudioSource e o clip estiverem configurados.
    /// </summary>
    void TocarSom(bool acertou)
    {
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource não foi atribuído no Inspector do GameManager.");
            return;
        }

        AudioClip clipEscolhido = acertou ? somAcerto : somErro;

        if (clipEscolhido != null)
        {
            audioSource.PlayOneShot(clipEscolhido);
        }
        else
        {
            Debug.LogWarning($"Som de {(acertou ? "acerto" : "erro")} não foi atribuído no Inspector do GameManager.");
        }
    }

    /// <summary>
    /// Reinicia o módulo do zero: volta pra primeira pergunta, reativa a imagem
    /// e limpa o texto de feedback.
    /// Ligue essa função ao OnClick do botão "Reiniciar" no Inspector.
    /// </summary>
    public void ReiniciarModulo()
    {
        CancelInvoke();

        currentQuestion = 0;
        estrelasConquistadas = 0;
        acertouSemErrarNestaPergunta = true;

        if (questionImage != null)
            questionImage.gameObject.SetActive(true);

        if (feedbackText != null)
            feedbackText.text = "";

        if (botaoReiniciar != null)
            botaoReiniciar.SetActive(false);

        if (telaDeEstrelas != null)
            telaDeEstrelas.Esconder();

        LoadQuestion();
    }
}