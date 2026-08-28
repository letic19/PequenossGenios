using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Sistema de pausa genérico, reutilizável em qualquer módulo (Números, Cores, Letras...).
/// Coloque este script em UM objeto por módulo (ou um só compartilhado, se os módulos
/// estiverem sempre na mesma cena e só um ficar ativo por vez).
///
/// Como funciona:
/// - O botão de pausa do módulo chama Pausar().
/// - O painel de pausa (que você monta na Hierarchy) aparece por cima de tudo,
///   bloqueando cliques no jogo por trás (contanto que ele cubra a tela toda
///   e tenha um Image com Raycast Target ligado).
/// - Time.timeScale = 0 congela qualquer coroutine que use WaitForSeconds
///   (ex: "próxima pergunta em 1 segundo"), então nada avança escondido atrás do painel.
/// - O botão "Continuar" dentro do painel de pausa deve chamar Continuar().
/// </summary>
public class PauseManager : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Painel de pausa — deve cobrir a tela toda e ter Raycast Target ligado no Image de fundo")]
    public GameObject painelDePausa;

    [Tooltip("Opcional: botão de pausa em si, pra esconder ele enquanto o jogo já está pausado")]
    public GameObject botaoDePausa;

    [Header("Navegação")]
    [Tooltip("O painel/objeto raiz DESTE módulo (ex: 'Números', 'Cores', 'Vogais') — será escondido ao ir para a tela inicial")]
    public GameObject moduloAtual;
    [Tooltip("O painel do menu principal / tela inicial — será mostrado ao clicar em 'Tela Inicial'")]
    public GameObject telaInicial;

    private bool pausado = false;

    void OnEnable()
    {
        // OnEnable roda toda vez que o objeto do módulo é reativado (ex: voltando do menu),
        // não só na primeira vez — assim o estado de pausa sempre começa limpo.
        pausado = false;
        Time.timeScale = 1f;

        if (painelDePausa != null)
            painelDePausa.SetActive(false);

        if (botaoDePausa != null)
            botaoDePausa.SetActive(true);
    }

    /// <summary>
    /// Ligue essa função ao OnClick do botão de pausa (o ícone ⏸ na tela do jogo).
    /// </summary>
    public void Pausar()
    {
        if (pausado) return;

        pausado = true;

        Time.timeScale = 0f;

        if (painelDePausa != null)
            painelDePausa.SetActive(true);

        if (botaoDePausa != null)
            botaoDePausa.SetActive(false);
    }

    /// <summary>
    /// Ligue essa função ao OnClick do botão "Continuar" dentro do painel de pausa.
    /// </summary>
    public void Continuar()
    {
        if (!pausado) return;

        pausado = false;

        Time.timeScale = 1f;

        if (painelDePausa != null)
            painelDePausa.SetActive(false);

        if (botaoDePausa != null)
            botaoDePausa.SetActive(true);
    }

    /// <summary>
    /// Alterna entre pausado/despausado — útil se quiser um único botão que funcione como toggle.
    /// </summary>
    public void AlternarPausa()
    {
        if (pausado)
            Continuar();
        else
            Pausar();
    }

    public bool EstaPausado()
    {
        return pausado;
    }

    /// <summary>
    /// Ligue essa função ao OnClick do botão "Ir para Tela Inicial" dentro do painel de pausa.
    /// Esconde o módulo atual e mostra o menu principal, restaurando o tempo normal do jogo.
    /// </summary>
    public void IrParaTelaInicial()
    {
        Time.timeScale = 1f;
        pausado = false;

        if (painelDePausa != null)
            painelDePausa.SetActive(false);

        if (botaoDePausa != null)
            botaoDePausa.SetActive(true);

        if (moduloAtual != null)
            moduloAtual.SetActive(false);
        else
            Debug.LogWarning("PauseManager: 'moduloAtual' não foi atribuído no Inspector — não sei qual painel esconder.");

        if (telaInicial != null)
            telaInicial.SetActive(true);
        else
            Debug.LogWarning("PauseManager: 'telaInicial' não foi atribuído no Inspector — não sei qual tela mostrar.");
    }

    /// <summary>
    /// Ligue essa função ao OnClick do botão "Sair do Jogo" dentro do painel de pausa.
    /// Fecha a aplicação. No Editor do Unity, apenas para o modo Play (Application.Quit não funciona lá).
    /// </summary>
    public void SairDoJogo()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// IMPORTANTE: sempre restaure o timeScale ao sair da cena/módulo,
    /// senão o jogo continua "pausado" mesmo depois de trocar de tela.
    /// </summary>
    void OnDisable()
    {
        Time.timeScale = 1f;
    }
}