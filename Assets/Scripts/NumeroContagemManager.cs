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

    [Header("Tela de Módulo Finalizado")]
    [Tooltip("Painel que aparece quando o jogador termina o módulo (antes de ir pro parque)")]
    public GameObject painelModuloFinalizado;
    [Tooltip("Texto dentro do painel de finalizado (ex: 'MÓDULO FINALIZADO!')")]
    public TextMeshProUGUI textoModuloFinalizado;
    [Tooltip("Quanto tempo o painel de finalizado fica na tela antes de seguir pro parque")]
    public float duracaoTelaFinalizado = 3f;

    private int quantidadeCorreta;
    private GameObject prefabDaRodadaAtual;
    private int acertosAtuais = 0;
    private List<GameObject> objetosNaTela = new List<GameObject>();
    private bool aguardandoProximaRodada = false;

    void Start()
    {
        ConfigurarBotoes();
        NovaRodada();
    }

    /// <summary>
    /// Liga o evento de clique de cada botão de número automaticamente.
    /// O valor do botão é lido do TEXTO dele (ex: o botão com o texto "6" vale 6),
    /// então não importa a ordem em que os botões foram arrastados no Inspector.
    /// </summary>
    void ConfigurarBotoes()
    {
        for (int i = 0; i < botoesNumero.Length; i++)
        {
            Button botao = botoesNumero[i];
            int valorDoBotao;

            if (!TentarLerNumeroDoBotao(botao, out valorDoBotao))
            {
                Debug.LogError($"Não consegui ler um número no texto do botão '{botao.name}'. " +
                                $"Confira se o texto dentro dele é só o número (ex: '6'), sem espaços ou outros caracteres.");
                continue;
            }

            botao.onClick.RemoveAllListeners();
            botao.onClick.AddListener(() => VerificarResposta(valorDoBotao));
        }
    }

    /// <summary>
    /// Procura um texto (TMP ou UI Text normal) dentro do botão e tenta converter para número.
    /// </summary>
    bool TentarLerNumeroDoBotao(Button botao, out int valor)
    {
        valor = -1;

        TMP_Text textoTMP = botao.GetComponentInChildren<TMP_Text>();
        if (textoTMP != null && int.TryParse(textoTMP.text.Trim(), out valor))
            return true;

        Text textoUI = botao.GetComponentInChildren<Text>();
        if (textoUI != null && int.TryParse(textoUI.text.Trim(), out valor))
            return true;

        return false;
    }

    /// <summary>
    /// Limpa a rodada anterior e sorteia uma nova quantidade de objetos.
    /// </summary>
    void NovaRodada()
    {
        aguardandoProximaRodada = false;
        LimparObjetos();

        quantidadeCorreta = Random.Range(quantidadeMinima, quantidadeMaxima + 1);

        // Sorteia UM tipo de objeto para a rodada inteira (ex: só maçã, ou só estrela)
        if (objetoPrefabs != null && objetoPrefabs.Length > 0)
        {
            prefabDaRodadaAtual = objetoPrefabs[Random.Range(0, objetoPrefabs.Length)];
        }

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
        if (objetoPrefabs == null || objetoPrefabs.Length == 0)
        {
            Debug.LogError("objetoPrefabs está vazio! Arraste os prefabs no Inspector.");
            return;
        }

        if (areaDeSpawn == null)
        {
            Debug.LogError("areaDeSpawn não foi atribuído no Inspector!");
            return;
        }

        GameObject prefabEscolhido = prefabDaRodadaAtual;

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

        Debug.Log($"Objeto criado: {novoObjeto.name} | Posição: {posicao} | Ativo: {novoObjeto.activeInHierarchy} | Pai: {novoObjeto.transform.parent.name}");

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
            textoInstrucao.text = "Muito bem! Você terminou o módulo!";

        yield return new WaitForSeconds(1.5f);

        // Esconde o painel de "Acerto!" antes de trocar de tela
        if (painelAcerto != null) painelAcerto.SetActive(false);

        // Esconde o jogo de contagem e mostra a tela de "Módulo Finalizado"
        if (moduloContagem != null) moduloContagem.SetActive(false);

        if (textoModuloFinalizado != null)
            textoModuloFinalizado.text = "MÓDULO FINALIZADO!";

        if (painelModuloFinalizado != null) painelModuloFinalizado.SetActive(true);

        yield return new WaitForSeconds(duracaoTelaFinalizado);

        // Depois da tela de finalizado, segue para o parque
        if (painelModuloFinalizado != null) painelModuloFinalizado.SetActive(false);
        if (moduloParque != null) moduloParque.SetActive(true);
    }
}