using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
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

    [Header("Áudio")]
    public AudioSource audioSource;
    public AudioClip somAcerto;
    public AudioClip somErro;

    [Header("Botão de Reiniciar")]
    [Tooltip("Botão de reiniciar — fica escondido durante o jogo, só aparece ao finalizar o módulo")]
    public GameObject botaoReiniciar;

    [Header("Painéis de Feedback (iguais aos do módulo de Números)")]
    [Tooltip("Painel que aparece quando a resposta certa é escolhida")]
    public GameObject painelAcerto;
    [Tooltip("Painel que aparece quando a resposta errada é escolhida (some sozinho depois de um tempo)")]
    public GameObject painelErro;
    [Tooltip("Quanto tempo os painéis de acerto/erro ficam visíveis antes de sumir")]
    public float duracaoPainelFeedback = 1f;

    [Header("Sistema de Estrelas")]
    [Tooltip("Componente EstrelasUI que mostra o resultado final")]
    public EstrelasUI telaDeEstrelas;

    private string respostaBotao1;
    private string respostaBotao2;
    private string corCorreta;

    private List<int> coresUsadas = new List<int>();
    private bool moduloFinalizado = false;
    private bool acertouSemErrarNestaPergunta = true;
    private int estrelasConquistadas = 0;

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

        if (botaoReiniciar != null)
            botaoReiniciar.SetActive(false);

        GerarPergunta();
    }

    void GerarPergunta()
    {
        if (moduloFinalizado)
            return;

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        if (painelAcerto != null) painelAcerto.SetActive(false);
        if (painelErro != null) painelErro.SetActive(false);

        if (cores == null || cores.Length == 0)
        {
            Debug.LogError("Nenhuma cor configurada!");
            return;
        }

        // Final do módulo
        if (coresUsadas.Count >= cores.Length)
        {
            moduloFinalizado = true;

            feedbackText.gameObject.SetActive(false);

            if (painelAcerto != null) painelAcerto.SetActive(false);
            if (painelErro != null) painelErro.SetActive(false);

            corImage.gameObject.SetActive(false);
            botao1.gameObject.SetActive(false);
            botao2.gameObject.SetActive(false);

            if (botaoReiniciar != null)
                botaoReiniciar.SetActive(true);

            if (telaDeEstrelas != null)
                telaDeEstrelas.MostrarResultado(estrelasConquistadas, cores.Length);

            return;
        }

        acertouSemErrarNestaPergunta = true;

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
        int tentativas = 0;

        do
        {
            indexErrada = Random.Range(0, cores.Length);
            tentativas++;

            if (tentativas > 100)
            {
                Debug.LogError("Não consegui achar uma cor com nome diferente da correta. Confira se o array 'cores' tem pelo menos 2 nomes diferentes (sem espaços extras).");
                return;
            }
        }
        while (NomesIguais(cores[indexErrada].nome, corCorreta));

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

        Debug.Log($"Pergunta gerada — Correta: '{corCorreta}' | Botão1: '{respostaBotao1}' | Botão2: '{respostaBotao2}'");

        botao1.onClick.AddListener(() => Responder(respostaBotao1 == corCorreta));
        botao2.onClick.AddListener(() => Responder(respostaBotao2 == corCorreta));
    }

    /// <summary>
    /// Compara dois nomes de cor ignorando espaços extras e diferença de maiúsculas/minúsculas,
    /// pra evitar bugs bobos tipo "Azul " != "azul" fazendo os botões repetirem.
    /// </summary>
    bool NomesIguais(string nomeA, string nomeB)
    {
        if (nomeA == null || nomeB == null) return nomeA == nomeB;
        return nomeA.Trim().Equals(nomeB.Trim(), System.StringComparison.OrdinalIgnoreCase);
    }

    void Responder(bool acertou)
    {
        if (feedbackText != null)
        {
            feedbackText.gameObject.SetActive(true);
            feedbackText.text = acertou ? "Correto!" : "Incorreto!";
        }

        TocarSom(acertou);

        if (acertou)
        {
            if (painelAcerto != null)
                painelAcerto.SetActive(true);

            if (acertouSemErrarNestaPergunta)
                estrelasConquistadas++;

            Invoke(nameof(GerarPergunta), 1f);
        }
        else
        {
            acertouSemErrarNestaPergunta = false;

            if (painelErro != null)
            {
                painelErro.SetActive(true);
                StartCoroutine(EsconderPainelErro());
            }
        }
    }

    IEnumerator EsconderPainelErro()
    {
        yield return new WaitForSeconds(duracaoPainelFeedback);

        if (painelErro != null)
            painelErro.SetActive(false);
    }

    /// <summary>
    /// Toca o som de acerto ou erro, se o AudioSource e o clip estiverem configurados.
    /// </summary>
    void TocarSom(bool acertou)
    {
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource não foi atribuído no Inspector do ColorQuiz.");
            return;
        }

        AudioClip clipEscolhido = acertou ? somAcerto : somErro;

        if (clipEscolhido != null)
        {
            audioSource.PlayOneShot(clipEscolhido);
        }
        else
        {
            Debug.LogWarning($"Som de {(acertou ? "acerto" : "erro")} não foi atribuído no Inspector do ColorQuiz.");
        }
    }

    /// <summary>
    /// Reinicia o módulo do zero: zera as cores já usadas, reativa a imagem
    /// e os botões, e sorteia a primeira pergunta de novo.
    /// Ligue essa função ao OnClick do botão "Reiniciar" no Inspector.
    /// </summary>
    public void ReiniciarModulo()
    {
        CancelInvoke();

        moduloFinalizado = false;
        coresUsadas.Clear();
        estrelasConquistadas = 0;
        acertouSemErrarNestaPergunta = true;

        if (corImage != null) corImage.gameObject.SetActive(true);
        if (botao1 != null) botao1.gameObject.SetActive(true);
        if (botao2 != null) botao2.gameObject.SetActive(true);

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        if (painelAcerto != null) painelAcerto.SetActive(false);
        if (painelErro != null) painelErro.SetActive(false);

        if (botaoReiniciar != null)
            botaoReiniciar.SetActive(false);

        if (telaDeEstrelas != null)
            telaDeEstrelas.Esconder();

        GerarPergunta();
    }
}