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
    public TextMeshProUGUI textoPalavra;
    public TextMeshProUGUI textoFeedback;

    [Header("Config")]
    public float tempoParaProximo = 1.2f;
    public bool usarAudioAoCompletar = true;
    public AudioClip somAcerto;
    public AudioClip somErro;

    private AudioSource audioSource;

    private string palavraCompleta;
    private char[] palavraComLacunas;
    private bool bloqueado = false;

    private List<int> palavrasUsadas = new List<int>();
    private bool moduloFinalizado = false;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        if (textoFeedback != null)
            textoFeedback.text = "";

        CarregarNovoObjeto();
    }

    public void CarregarNovoObjeto()
    {
        if (moduloFinalizado)
            return;

        bloqueado = false;

        if (database == null || database.palavras == null || database.palavras.Length == 0)
        {
            if (textoPalavra != null)
                textoPalavra.text = "ERRO: Sem dados";

            Debug.LogError("Nenhum database configurado.");
            return;
        }

        // Terminou todas as palavras
        if (palavrasUsadas.Count >= database.palavras.Length)
        {
            moduloFinalizado = true;

            if (textoPalavra != null)
                textoPalavra.text = "🎉 MÓDULO FINALIZADO!";

            if (textoFeedback != null)
                textoFeedback.text = "";

            if (imagemObjeto != null)
                imagemObjeto.gameObject.SetActive(false);

            return;
        }

        int indice;

        do
        {
            indice = Random.Range(0, database.palavras.Length);
        }
        while (palavrasUsadas.Contains(indice));

        palavrasUsadas.Add(indice);

        WordData item = database.palavras[indice];

        palavraCompleta = item.palavraCompleta.ToUpper().Trim();

        if (imagemObjeto != null)
            imagemObjeto.sprite = item.imagem;

        palavraComLacunas = palavraCompleta.ToCharArray();

        for (int i = 0; i < palavraComLacunas.Length; i++)
        {
            if ("AEIOU".IndexOf(palavraComLacunas[i]) >= 0)
                palavraComLacunas[i] = '_';
        }

        if (textoPalavra != null)
            textoPalavra.text = FormatarComEspacos(new string(palavraComLacunas));

        if (textoFeedback != null)
            textoFeedback.text = "";
    }

    string FormatarComEspacos(string texto)
    {
        return string.Join(" ", texto.ToCharArray());
    }

    public void EscolherVogal(string letra)
    {
        if (bloqueado || moduloFinalizado)
            return;

        letra = letra.ToUpper();

        bool acertou = false;

        for (int i = 0; i < palavraComLacunas.Length; i++)
        {
            if (palavraComLacunas[i] == '_' &&
                palavraCompleta[i].ToString() == letra)
            {
                palavraComLacunas[i] = letra[0];
                acertou = true;
            }
        }

        if (textoPalavra != null)
            textoPalavra.text = FormatarComEspacos(new string(palavraComLacunas));

        if (acertou)
        {
            if (textoFeedback != null)
                textoFeedback.text = "✔ Correto!";

            if (somAcerto != null && usarAudioAoCompletar)
                audioSource.PlayOneShot(somAcerto);

            if (!new string(palavraComLacunas).Contains("_"))
            {
                StartCoroutine(EsperarEAvancar());
            }
        }
        else
        {
            if (textoFeedback != null)
                textoFeedback.text = "✖ Incorreto!";

            if (somErro != null && usarAudioAoCompletar)
                audioSource.PlayOneShot(somErro);
        }
    }

    IEnumerator EsperarEAvancar()
    {
        bloqueado = true;

        yield return new WaitForSeconds(tempoParaProximo);

        CarregarNovoObjeto();
    }

    public void Reiniciar()
    {
        palavrasUsadas.Clear();
        moduloFinalizado = false;

        if (imagemObjeto != null)
            imagemObjeto.gameObject.SetActive(true);

        CarregarNovoObjeto();
    }
}
