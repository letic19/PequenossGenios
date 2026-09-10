using UnityEngine;
using UnityEngine.UI;

public class ControladorSom : MonoBehaviour
{
    public static ControladorSom Instance { get; private set; }

    private bool estadoSom = true;
    [SerializeField] private AudioSource FundoMusical;
    [SerializeField] private Sprite somLigadoSprite;
    [SerializeField] private Sprite somDesligadoSprite;

    [SerializeField] private Image MuteImage;

    [Header("Efeitos sonoros (Correto/Incorreto)")]
    [SerializeField] private float volumeEfeitos = 1f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void LigarDesligarSom()
    {
        estadoSom = !estadoSom;
        FundoMusical.enabled = estadoSom;

        if (estadoSom)
        {
            MuteImage.sprite = somLigadoSprite;
        }
        else
        {
            MuteImage.sprite = somDesligadoSprite;
        }
    }

    public void VolumeMusical(float value)
    {
        FundoMusical.volume = value;
    }

    /// <summary>
    /// Ligue ao "On Value Changed (Single)" do Slider de Efeitos Sonoros.
    /// </summary>
    public void VolumeEfeito(float value)
    {
        volumeEfeitos = value;
        Debug.Log($"VolumeEfeito() chamado. Novo volumeEfeitos = {volumeEfeitos}");
    }

    /// <summary>
    /// Chame de qualquer módulo para tocar um efeito (acerto/erro), respeitando
    /// o botão de mudo geral e o volume de efeitos configurado.
    /// </summary>
    public void TocarEfeito(AudioSource fonte, AudioClip clip)
    {
        Debug.Log($"TocarEfeito() chamado. estadoSom={estadoSom} | volumeEfeitos={volumeEfeitos} | fonte.volume={(fonte != null ? fonte.volume.ToString() : "fonte nula")}");

        if (!estadoSom || volumeEfeitos <= 0f)
        {
            Debug.Log("TocarEfeito: som ignorado (mudo ou volume 0).");
            return;
        }

        if (fonte == null || clip == null)
            return;

        fonte.PlayOneShot(clip, volumeEfeitos);
    }

    // Getters para outras telas (ex: painel de pausa) sincronizarem sliders/ícones
    public bool SomLigado => estadoSom;
    public float VolumeMusicaAtual => FundoMusical != null ? FundoMusical.volume : 0f;
    public float VolumeEfeitoAtual => volumeEfeitos;
}