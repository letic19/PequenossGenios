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

    [Header("Config")]
    public float tempoParaProximo = 1.2f;
    public AudioClip somAcerto;
    public AudioClip somErro;

    private AudioSource audioSource;

    private List<int> palavrasUsadas = new List<int>();

    private bool moduloFinalizado = false;
    private bool bloqueado = false;

    private int totalLacunas;
    private int lacunasPreenchidas;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        textoFeedback.text = "";
        CarregarNovoObjeto();
    }

    public void CarregarNovoObjeto()
    {
        if (moduloFinalizado)
            return;

        bloqueado = false;
        lacunasPreenchidas = 0;

        if (palavrasUsadas.Count >= database.palavras.Length)
        {
            moduloFinalizado = true;

            textoFeedback.text = "🎉 MÓDULO FINALIZADO!";

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

        WordData palavra = database.palavras[indice];

        imagemObjeto.sprite = palavra.imagem;

        totalLacunas = ContarVogais(palavra.palavraCompleta);

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

        if (somAcerto != null)
            audioSource.PlayOneShot(somAcerto);

        if (lacunasPreenchidas >= totalLacunas)
        {
            textoFeedback.text = "✔ Correto!";

            StartCoroutine(ProximaPalavra());
        }
    }

    public void LetraErrada()
    {
        textoFeedback.text = "✖ Incorreto!";

        if (somErro != null)
            audioSource.PlayOneShot(somErro);
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
}