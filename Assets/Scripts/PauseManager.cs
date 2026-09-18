using UnityEngine;
using UnityEngine.UI;


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
    [Tooltip("Outras telas intermediárias que precisam ser escondidas ao voltar pro início (ex: a tela de 'Módulos'/seleção)")]
    public GameObject[] outrasTelasParaEsconder;

    [Header("Opções")]
    [Tooltip("O script OpcoesMenu do painel de Opções COMPARTILHADO (o mesmo que já existe na tela inicial — não crie um novo)")]
    public OpcoesMenu opcoesMenu;
    [Tooltip("Slider de volume da música, dentro do painel de pausa")]
    public Slider sliderMusica;
    [Tooltip("Slider de volume dos efeitos sonoros, dentro do painel de pausa")]
    public Slider sliderEfeitos;
    [Tooltip("Opcional: ícone do botão de mudo, dentro do painel de pausa")]
    public Image iconeMudo;
    public Sprite spriteSomLigado;
    public Sprite spriteSomMutado;

    private bool pausado = false;

    void OnEnable()
    {
        
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

        AtualizarControlesDeAudio();
    }

    
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

   
    public void SairDoJogo()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    
    void OnDisable()
    {
        Time.timeScale = 1f;
    }

    
    public void DefinirVolumeMusica(float volume)
    {
        if (ControladorSom.Instance != null)
            ControladorSom.Instance.VolumeMusical(volume);
    }

    
    public void DefinirVolumeEfeitos(float volume)
    {
        if (ControladorSom.Instance != null)
            ControladorSom.Instance.VolumeEfeito(volume);
    }

    
    public void AlternarMudo()
    {
        if (ControladorSom.Instance == null) return;

        ControladorSom.Instance.LigarDesligarSom();
        AtualizarIconeMudo();
    }

   
    void AtualizarControlesDeAudio()
    {
        if (ControladorSom.Instance == null) return;

        if (sliderMusica != null)
            sliderMusica.SetValueWithoutNotify(ControladorSom.Instance.VolumeMusicaAtual);

        if (sliderEfeitos != null)
            sliderEfeitos.SetValueWithoutNotify(ControladorSom.Instance.VolumeEfeitoAtual);

        AtualizarIconeMudo();
    }

    void AtualizarIconeMudo()
    {
        if (iconeMudo == null || ControladorSom.Instance == null) return;

        iconeMudo.sprite = ControladorSom.Instance.SomLigado ? spriteSomLigado : spriteSomMutado;
    }

    
    public void AbrirOpcoes()
    {
        if (opcoesMenu == null)
        {
            Debug.LogWarning("PauseManager: 'opcoesMenu' não foi atribuído no Inspector.");
            return;
        }

        opcoesMenu.Abrir(painelDePausa);
        AtualizarControlesDeAudio();
    }
}