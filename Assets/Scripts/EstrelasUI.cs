using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class EstrelasUI : MonoBehaviour
{
    [Header("Estrelas (arraste as 5 imagens de estrela, na ordem)")]
    public Image[] estrelas;

    [Tooltip("Sprite da estrela preenchida (conquistada)")]
    public Sprite spriteEstrelaPreenchida;

    [Tooltip("Sprite da estrela vazia (não conquistada)")]
    public Sprite spriteEstrelaVazia;

    [Header("Texto de resultado")]
    public TMP_Text textoResultado;

    [Header("Mensagens")]
    [TextArea]
    public string mensagemTodasEstrelas = "Parabéns! Você conseguiu todas as estrelas!";

    [TextArea]
    public string mensagemParcial = "Você foi muito bem! Vamos tentar novamente para conseguir mais estrelas?";

    [Header("Som de Finalização")]
    public AudioSource audioSource;
    public AudioClip somFinalizacao;

    
    public void MostrarResultado(int acertosDePrimeira, int totalDePerguntas)
    {
        Debug.Log($"EstrelasUI.MostrarResultado chamado com acertosDePrimeira={acertosDePrimeira}, totalDePerguntas={totalDePerguntas}. Ativando {gameObject.name}...");

        gameObject.SetActive(true);

       
        if (audioSource != null && somFinalizacao != null)
        {
            audioSource.PlayOneShot(somFinalizacao);
        }

        if (estrelas == null || estrelas.Length == 0)
        {
            Debug.LogWarning("O array 'estrelas' está vazio no EstrelasUI! Arraste as 5 imagens no Inspector.");
        }

        int estrelasParaMostrar = CalcularEstrelas(acertosDePrimeira, totalDePerguntas);

        Debug.Log($"Estrelas calculadas para mostrar: {estrelasParaMostrar} de {estrelas.Length}");

        for (int i = 0; i < estrelas.Length; i++)
        {
            if (estrelas[i] == null)
            {
                Debug.LogWarning($"Elemento {i} do array 'estrelas' está vazio (None) no Inspector.");
                continue;
            }

            estrelas[i].sprite = (i < estrelasParaMostrar)
                ? spriteEstrelaPreenchida
                : spriteEstrelaVazia;
        }

        if (textoResultado != null)
        {
            textoResultado.gameObject.SetActive(true);

            if (estrelasParaMostrar == estrelas.Length)
            {
                textoResultado.text = mensagemTodasEstrelas;
            }
            else
            {
                textoResultado.text = mensagemParcial;
            }
        }
        else
        {
            Debug.LogWarning("textoResultado está NULO no EstrelasUI — não foi arrastado no Inspector.");
        }
    }

    
    int CalcularEstrelas(int acertosDePrimeira, int totalDePerguntas)
    {
        if (totalDePerguntas <= 0) return 0;

        float proporcao = (float)acertosDePrimeira / totalDePerguntas;

        int estrelasCalculadas = Mathf.RoundToInt(proporcao * estrelas.Length);

        return Mathf.Clamp(estrelasCalculadas, 0, estrelas.Length);
    }

    
    public void Esconder()
    {
        gameObject.SetActive(false);
    }
}