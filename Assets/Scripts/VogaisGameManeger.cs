using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class VogaisGameManager : MonoBehaviour
{
    [Header("Database")]
    public WordDatabase database;

    [Header("UI")]
    public Image imagemObjeto;
    public TextMeshProUGUI textoFeedback;
    public PalavraBuilder palavraBuilder;

    [Header("Painéis de Feedback (iguais aos do módulo de Números)")]
    [Tooltip("Painel que aparece quando a palavra inteira é completada corretamente")]
    public GameObject painelAcerto;
    [Tooltip("Painel que aparece quando uma letra errada é solta (some sozinho depois de um tempo)")]
    public GameObject painelErro;
    [Tooltip("Quanto tempo o painel de erro fica visível antes de sumir sozinho")]
    public float duracaoPainelErro = 1f;

    [Header("Config")]
    public float tempoParaProximo = 1.2f;
    public AudioClip somAcerto;
    public AudioClip somErro;

    [Header("Botão de Reiniciar")]
    [Tooltip("Botão de reiniciar — o ideal é ele estar DENTRO do painel de estrelas (TelaDeEstrelas), como filho, pra aparecer/sumir sozinho")]
    public GameObject botaoReiniciar;

    [Header("Sistema de Estrelas")]
    [Tooltip("Componente EstrelasUI que mostra o resultado final")]
    public EstrelasUI telaDeEstrelas;

    private AudioSource audioSource;

    private List<int> palavrasUsadas = new List<int>();

    private bool moduloFinalizado = false;
    private bool bloqueado = false;

    private int totalLacunas;
    private int lacunasPreenchidas;

    private bool acertouSemErrarNestaPalavra = true;
    private int estrelasConquistadas = 0;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        textoFeedback.text = "";

        if (botaoReiniciar != null)
            botaoReiniciar.SetActive(false);

        CarregarNovoObjeto();
    }

    public void CarregarNovoObjeto()
    {
        if (moduloFinalizado)
        {
            Debug.Log("CarregarNovoObjeto chamado, mas moduloFinalizado já é true. Ignorando.");
            return;
        }

        bloqueado = false;
        lacunasPreenchidas = 0;
        acertouSemErrarNestaPalavra = true;

        if (painelAcerto != null) painelAcerto.SetActive(false);
        if (painelErro != null) painelErro.SetActive(false);

        Debug.Log($"CarregarNovoObjeto: palavrasUsadas.Count={palavrasUsadas.Count} / database.palavras.Length={database.palavras.Length}");

        if (palavrasUsadas.Count >= database.palavras.Length)
        {
            moduloFinalizado = true;

            Debug.Log("MÓDULO FINALIZADO detectado dentro de CarregarNovoObjeto.");

            textoFeedback.text = "";

            imagemObjeto.gameObject.SetActive(false);

            if (botaoReiniciar != null)
                botaoReiniciar.SetActive(true);

            if (telaDeEstrelas != null)
                telaDeEstrelas.MostrarResultado(estrelasConquistadas, database.palavras.Length);

            return;
        }

        int indice;

        do
        {
            indice = Random.Range(0, database.palavras.Length);
        }
        while (palavrasUsadas.Contains(indice));

        palavrasUsadas.Add(indice);

        WordData palavra = database.palavras[indice];

        imagemObjeto.sprite = palavra.imagem;

        totalLacunas = ContarVogais(palavra.palavraCompleta);

        Debug.Log($"Nova palavra carregada: '{palavra.palavraCompleta}' | totalLacunas (vogais) = {totalLacunas}");

        palavraBuilder.MontarPalavra(
            palavra.palavraCompleta.ToUpper(),
            this
        );

        textoFeedback.text = "";
    }

    int ContarVogais(string palavra)
    {
        int total = 0;

        foreach (char letra in palavra.ToUpper())
        {
            if ("AEIOU".Contains(letra.ToString()))
                total++;
        }

        return total;
    }

    public void LetraCorreta()
    {
        lacunasPreenchidas++;

        Debug.Log($"LetraCorreta() chamado. lacunasPreenchidas={lacunasPreenchidas} / totalLacunas={totalLacunas}");

        if (somAcerto != null)
            audioSource.PlayOneShot(somAcerto);

        if (lacunasPreenchidas >= totalLacunas)
        {
            Debug.Log("Palavra completa! Chamando ProximaPalavra...");

            textoFeedback.text = "Correto!";

            if (painelAcerto != null)
                painelAcerto.SetActive(true);

            if (acertouSemErrarNestaPalavra)
                estrelasConquistadas++;

            StartCoroutine(ProximaPalavra());
        }
    }

    public void LetraErrada()
    {
        textoFeedback.text = "Incorreto!";

        acertouSemErrarNestaPalavra = false;

        if (somErro != null)
            audioSource.PlayOneShot(somErro);

        if (painelErro != null)
        {
            painelErro.SetActive(true);
            StartCoroutine(EsconderPainelErro());
        }
    }

    IEnumerator EsconderPainelErro()
    {
        yield return new WaitForSeconds(duracaoPainelErro);

        if (painelErro != null)
            painelErro.SetActive(false);
    }

    IEnumerator ProximaPalavra()
    {
        bloqueado = true;

        yield return new WaitForSeconds(tempoParaProximo);

        CarregarNovoObjeto();
    }

    public bool PodeJogar()
    {
        return !bloqueado;
    }

    /// <summary>
    /// Reinicia o módulo do zero: zera as palavras usadas e as estrelas,
    /// reativa a imagem e sorteia a primeira palavra de novo.
    /// Ligue essa função ao OnClick do botão "Reiniciar" no Inspector.
    /// </summary>
    public void ReiniciarModulo()
    {
        Debug.Log("ReiniciarModulo() chamado no VogaisGameManager.");

        StopAllCoroutines();

        moduloFinalizado = false;
        bloqueado = false;
        palavrasUsadas.Clear();
        estrelasConquistadas = 0;
        acertouSemErrarNestaPalavra = true;

        if (imagemObjeto != null)
            imagemObjeto.gameObject.SetActive(true);
        else
            Debug.LogWarning("imagemObjeto está nulo no ReiniciarModulo.");

        if (textoFeedback != null)
            textoFeedback.text = "";

        if (painelAcerto != null) painelAcerto.SetActive(false);
        if (painelErro != null) painelErro.SetActive(false);

        if (botaoReiniciar != null)
            botaoReiniciar.SetActive(false);

        if (telaDeEstrelas != null)
            telaDeEstrelas.Esconder();

        if (palavraBuilder != null)
            Debug.Log($"palavraBuilder OK. Filhos atuais antes de montar nova palavra: {palavraBuilder.transform.childCount}");
        else
            Debug.LogWarning("palavraBuilder está nulo no ReiniciarModulo!");

        Debug.Log($"Chamando CarregarNovoObjeto(). moduloFinalizado={moduloFinalizado}, palavrasUsadas.Count={palavrasUsadas.Count}, database.palavras.Length={(database != null ? database.palavras.Length : -1)}");

        CarregarNovoObjeto();
    }
}