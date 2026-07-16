using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Módulo "Eu sei contar" (Números)
/// O jogador vê uma quantidade aleatória de objetos na tela e deve
/// clicar no número correspondente. Após um número de acertos,
/// o jogo carrega a cena do Parque do Escola Games (coleta de estrelas).
/// </summary>
public class NumeroContagemManager : MonoBehaviour
{
    [Header("Configuração da Rodada")]
    [Tooltip("Prefabs dos objetos que podem ser contados (ex: maçã, estrela, bola)")]
    public GameObject[] objetoPrefabs;

    [Tooltip("Área retangular onde os objetos serão espalhados")]
    public RectTransform areaDeSpawn;

    [Tooltip("Quantidade mínima e máxima de objetos por rodada")]
    public int quantidadeMinima = 1;
    public int quantidadeMaxima = 10;

    [Tooltip("Distância mínima entre os objetos para não se sobreporem")]
    public float distanciaMinimaEntreObjetos = 90f;

    [Header("Botões de Resposta (0 a 10)")]
    public Button[] botoesNumero; // arraste os botões 0-10 na ordem no Inspector

    [Header("UI de Feedback")]
    public TextMeshProUGUI textoInstrucao;
    public GameObject painelAcerto;
    public GameObject painelErro;

    [Header("Áudio")]
    public AudioSource audioSource;
    public AudioClip somAcerto;
    public AudioClip somErro;

    [Header("Progresso")]
    [Tooltip("Quantos acertos são necessários para liberar o passeio no parque")]
    public int acertosParaVencer = 5;

    [Header("Transição para o Parque (mesma cena)")]
    [Tooltip("Objeto pai que contém toda a UI/lógica do jogo de contagem")]
    public GameObject moduloContagem;
    [Tooltip("Objeto pai que contém toda a UI/lógica do Parque do Escola Games")]
    public GameObject moduloParque;

    private int quantidadeCorreta;
    private int acertosAtuais = 0;
    private List<GameObject> objetosNaTela = new List<GameObject>();
    private bool aguardandoProximaRodada = false;

    void Start()
    {
        ConfigurarBotoes();
        NovaRodada();
    }

    /// <summary>
    /// Liga o evento de clique de cada botão de número automaticamente,
    /// usando o índice do botão no array como o valor numérico dele.
    /// </summary>
    void ConfigurarBotoes()
    {
        for (int i = 0; i < botoesNumero.Length; i++)
        {
            int valorDoBotao = i; // 0,1,2...10 — cópia local para evitar bug de closure
            botoesNumero[i].onClick.RemoveAllListeners();
            botoesNumero[i].onClick.AddListener(() => VerificarResposta(valorDoBotao));
        }
    }

    /// <summary>
    /// Limpa a rodada anterior e sorteia uma nova quantidade de objetos.
    /// </summary>
    void NovaRodada()
    {
        aguardandoProximaRodada = false;
        LimparObjetos();

        quantidadeCorreta = Random.Range(quantidadeMinima, quantidadeMaxima + 1);

        for (int i = 0; i < quantidadeCorreta; i++)
        {
            SpawnObjeto();
        }

        if (textoInstrucao != null)
            textoInstrucao.text = "Conte quantos objetos aparecem e clique no número certo!";

        if (painelAcerto != null) painelAcerto.SetActive(false);
        if (painelErro != null) painelErro.SetActive(false);
    }

    void LimparObjetos()
    {
        foreach (var obj in objetosNaTela)
        {
            if (obj != null) Destroy(obj);
        }
        objetosNaTela.Clear();
    }

    /// <summary>
    /// Instancia um objeto aleatório dentro da área de spawn,
    /// tentando evitar sobreposição com objetos já colocados.
    /// </summary>
    void SpawnObjeto()
    {
        GameObject prefabEscolhido = objetoPrefabs[Random.Range(0, objetoPrefabs.Length)];

        Vector2 posicao = Vector2.zero;
        int tentativas = 0;
        bool posicaoValida = false;

        while (!posicaoValida && tentativas < 30)
        {
            posicao = PosicaoAleatoriaNaArea();
            posicaoValida = true;

            foreach (var obj in objetosNaTela)
            {
                if (obj == null) continue;
                RectTransform rectExistente = obj.GetComponent<RectTransform>();
                Vector2 posicaoExistente = rectExistente != null ? rectExistente.anchoredPosition : (Vector2)obj.transform.localPosition;
                float distancia = Vector2.Distance(posicaoExistente, posicao);
                if (distancia < distanciaMinimaEntreObjetos)
                {
                    posicaoValida = false;
                    break;
                }
            }
            tentativas++;
        }

        GameObject novoObjeto = Instantiate(prefabEscolhido, areaDeSpawn);

        RectTransform rectDoObjeto = novoObjeto.GetComponent<RectTransform>();
        if (rectDoObjeto != null)
        {
            // anchoredPosition é o correto para elementos de UI
            // (localPosition só funciona igual quando o anchor está centralizado)
            rectDoObjeto.anchoredPosition = posicao;
            rectDoObjeto.localScale = Vector3.one;
        }
        else
        {
            novoObjeto.transform.localPosition = posicao;
        }

        novoObjeto.transform.SetAsLastSibling(); // garante que fique na frente do fundo/painel

        objetosNaTela.Add(novoObjeto);
    }

    Vector2 PosicaoAleatoriaNaArea()
    {
        float largura = areaDeSpawn.rect.width;
        float altura = areaDeSpawn.rect.height;

        float x = Random.Range(-largura / 2f, largura / 2f);
        float y = Random.Range(-altura / 2f, altura / 2f);

        return new Vector2(x, y);
    }

    /// <summary>
    /// Chamado quando o jogador clica em um botão de número.
    /// </summary>
    void VerificarResposta(int numeroEscolhido)
    {
        if (aguardandoProximaRodada) return; // evita clique duplo durante o feedback

        if (numeroEscolhido == quantidadeCorreta)
        {
            AcertoResposta();
        }
        else
        {
            ErroResposta();
        }
    }

    void AcertoResposta()
    {
        aguardandoProximaRodada = true;
        acertosAtuais++;

        if (painelAcerto != null) painelAcerto.SetActive(true);
        if (audioSource != null && somAcerto != null) audioSource.PlayOneShot(somAcerto);

        if (acertosAtuais >= acertosParaVencer)
        {
            StartCoroutine(IrParaOParque());
        }
        else
        {
            StartCoroutine(ProximaRodadaComAtraso(1.5f));
        }
    }

    void ErroResposta()
    {
        if (painelErro != null) painelErro.SetActive(true);
        if (audioSource != null && somErro != null) audioSource.PlayOneShot(somErro);

        StartCoroutine(EsconderPainelErro(1f));
        // Aqui o jogador pode tentar novamente, não avança de rodada
    }

    IEnumerator EsconderPainelErro(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        if (painelErro != null) painelErro.SetActive(false);
    }

    IEnumerator ProximaRodadaComAtraso(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        NovaRodada();
    }

    IEnumerator IrParaOParque()
    {
        if (textoInstrucao != null)
            textoInstrucao.text = "Muito bem! Vamos passear no Parque do Escola Games!";

        yield return new WaitForSeconds(2f);

        // Em vez de trocar de cena, escondemos o módulo de contagem
        // e mostramos o módulo do parque, dentro da mesma cena.
        if (moduloContagem != null) moduloContagem.SetActive(false);
        if (moduloParque != null) moduloParque.SetActive(true);
    }
}
