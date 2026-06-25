using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ColorQuiz : MonoBehaviour
{
    public Image corImage;
    public Button botao1;
    public Button botao2;
    public TMP_Text texto1;
    public TMP_Text texto2;
    public TMP_Text feedbackText;

    public CorItem[] cores;

    private string respostaBotao1;
    private string respostaBotao2;
    private string corCorreta;

    private List<int> coresUsadas = new List<int>();
    private bool moduloFinalizado = false;

    void Awake()
    {
        if (texto1 == null && botao1 != null)
            texto1 = botao1.GetComponentInChildren<TMP_Text>();

        if (texto2 == null && botao2 != null)
            texto2 = botao2.GetComponentInChildren<TMP_Text>();

        if (texto1 != null)
            texto1.color = new Color(1f, 0.5f, 0f);

        if (texto2 != null)
            texto2.color = new Color(1f, 0.5f, 0f);

        if (feedbackText != null)
            feedbackText.color = Color.white;
    }

    void Start()
    {
        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        GerarPergunta();
    }

    void GerarPergunta()
    {
        if (moduloFinalizado)
            return;

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        if (cores == null || cores.Length == 0)
        {
            Debug.LogError("Nenhuma cor configurada!");
            return;
        }

        // Final do módulo
        if (coresUsadas.Count >= cores.Length)
        {
            moduloFinalizado = true;

            feedbackText.gameObject.SetActive(true);
            feedbackText.text = "MÓDULO FINALIZADO!";

            corImage.gameObject.SetActive(false);
            botao1.gameObject.SetActive(false);
            botao2.gameObject.SetActive(false);

            return;
        }

        int indexCorreta;

        do
        {
            indexCorreta = Random.Range(0, cores.Length);
        }
        while (coresUsadas.Contains(indexCorreta));

        coresUsadas.Add(indexCorreta);

        corCorreta = cores[indexCorreta].nome;

        // Mostra imagem
        corImage.sprite = cores[indexCorreta].imagem;

        int indexErrada;

        do
        {
            indexErrada = Random.Range(0, cores.Length);
        }
        while (indexErrada == indexCorreta);

        bool corretaNoBotao1 = Random.value > 0.5f;

        botao1.onClick.RemoveAllListeners();
        botao2.onClick.RemoveAllListeners();

        if (corretaNoBotao1)
        {
            respostaBotao1 = corCorreta;
            respostaBotao2 = cores[indexErrada].nome;
        }
        else
        {
            respostaBotao1 = cores[indexErrada].nome;
            respostaBotao2 = corCorreta;
        }

        texto1.text = respostaBotao1;
        texto2.text = respostaBotao2;

        botao1.onClick.AddListener(() => Responder(respostaBotao1 == corCorreta));
        botao2.onClick.AddListener(() => Responder(respostaBotao2 == corCorreta));
    }

    void Responder(bool acertou)
    {
        if (feedbackText != null)
        {
            feedbackText.gameObject.SetActive(true);
            feedbackText.text = acertou ? "Correto!" : "Incorreto!";
        }

        if (acertou)
        {
            Invoke(nameof(GerarPergunta), 1f);
        }
    }
}