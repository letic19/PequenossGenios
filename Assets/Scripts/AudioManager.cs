using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gerenciador central de áudio do jogo. Controla o VOLUME da música de fundo e dos
/// efeitos sonoros (acerto/erro) através de Sliders, e lembra a escolha do jogador
/// entre sessões (usando PlayerPrefs). Volume 0 funciona como "desligado".
///
/// COMO USAR:
/// 1. Coloque este script em UM objeto só, na tela inicial/menu (ex: "AudioManager").
/// 2. Arraste o clipe de música de fundo no campo "Musica De Fundo".
/// 3. Em cada módulo (ColorQuiz, GameManager, VogaisGameManager, NumeroContagemManager),
///    troque a chamada "audioSource.PlayOneShot(clip)" por "AudioManager.Instance.TocarEfeito(audioSource, clip)".
/// 4. Crie dois Sliders (UI > Slider) na tela de Opções: um para "Música", outro para "Efeitos Sonoros".
///    Configure o Min Value = 0 e Max Value = 1 em cada um.
///    No "On Value Changed" de cada Slider, arraste o AudioManager e escolha
///    DefinirVolumeMusica / DefinirVolumeEfeitos.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Música de fundo")]
    public AudioClip musicaDeFundo;

    [Header("UI (opcional — arraste se quiser os sliders já sincronizados automaticamente)")]
    public Slider sliderMusica;
    public Slider sliderEfeitos;

    private AudioSource fonteMusica;

    private const string CHAVE_VOLUME_MUSICA = "volumeMusica";
    private const string CHAVE_VOLUME_EFEITOS = "volumeEfeitos";

    public float VolumeMusica { get; private set; } = 0.5f;
    public float VolumeEfeitos { get; private set; } = 1f;

    void Awake()
    {
        // Garante que só existe um AudioManager, mesmo se essa cena for recarregada.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        fonteMusica = gameObject.AddComponent<AudioSource>();
        fonteMusica.loop = true;
        fonteMusica.playOnAwake = false;

        // Carrega a preferência salva (padrão: música em 50%, efeitos em 100%, se nunca configurado antes)
        VolumeMusica = PlayerPrefs.GetFloat(CHAVE_VOLUME_MUSICA, 0.5f);
        VolumeEfeitos = PlayerPrefs.GetFloat(CHAVE_VOLUME_EFEITOS, 1f);
    }

    void Start()
    {
        AtualizarMusica();
        AtualizarSliders();
    }

    /// <summary>
    /// Chame este método de qualquer módulo para tocar um som de efeito (acerto/erro),
    /// aplicando o volume configurado pelo jogador. Se o volume estiver em 0, o som
    /// simplesmente não é tocado.
    /// </summary>
    public void TocarEfeito(AudioSource fonte, AudioClip clip)
    {
        if (VolumeEfeitos <= 0f)
        {
            Debug.Log("AudioManager: volume de efeitos está em 0, som ignorado.");
            return;
        }

        if (fonte == null || clip == null)
            return;

        fonte.PlayOneShot(clip, VolumeEfeitos);
    }

    /// <summary>
    /// Ligue ao "On Value Changed (Single)" do Slider de Música na tela de Opções.
    /// </summary>
    public void DefinirVolumeMusica(float volume)
    {
        VolumeMusica = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(CHAVE_VOLUME_MUSICA, VolumeMusica);
        PlayerPrefs.Save();

        AtualizarMusica();
    }

    /// <summary>
    /// Ligue ao "On Value Changed (Single)" do Slider de Efeitos Sonoros na tela de Opções.
    /// </summary>
    public void DefinirVolumeEfeitos(float volume)
    {
        VolumeEfeitos = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(CHAVE_VOLUME_EFEITOS, VolumeEfeitos);
        PlayerPrefs.Save();
    }

    void AtualizarMusica()
    {
        if (fonteMusica == null) return;

        fonteMusica.volume = VolumeMusica;

        if (VolumeMusica > 0f)
        {
            if (musicaDeFundo != null && fonteMusica.clip != musicaDeFundo)
                fonteMusica.clip = musicaDeFundo;

            if (!fonteMusica.isPlaying && fonteMusica.clip != null)
                fonteMusica.Play();
        }
        else
        {
            fonteMusica.Pause();
        }
    }

    /// <summary>
    /// Sincroniza os Sliders da UI com o volume atual salvo, caso você tenha arrastado
    /// eles no Inspector — assim, ao abrir a tela de Opções, os sliders já aparecem
    /// na posição da última escolha do jogador.
    /// </summary>
    void AtualizarSliders()
    {
        if (sliderMusica != null)
            sliderMusica.SetValueWithoutNotify(VolumeMusica);

        if (sliderEfeitos != null)
            sliderEfeitos.SetValueWithoutNotify(VolumeEfeitos);
    }
}