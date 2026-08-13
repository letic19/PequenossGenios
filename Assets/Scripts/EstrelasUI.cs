using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Componente reutilizável para mostrar o resultado final de um módulo
/// como estrelas preenchidas/vazias, junto com uma mensagem.
/// Use o MESMO script em todos os módulos (Números, Cores, Letras).
/// </summary>
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

    /// <summary>
    /// Mostra o resultado: quantas perguntas o jogador acertou de primeira (sem errar)
    /// em relação ao total de perguntas do módulo. O número de estrelas exibidas (de 0 a 5)
    /// é calculado proporcionalmente, mesmo que o módulo tenha mais ou menos que 5 perguntas.
    /// </summary>
    public void MostrarResultado(int acertosDePrimeira, int totalDePerguntas)
    {
        Debug.Log($"EstrelasUI.MostrarResultado chamado com acertosDePrimeira={acertosDePrimeira}, totalDePerguntas={totalDePerguntas}. Ativando {gameObject.name}...");

        gameObject.SetActive(true);

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
            estrelas[i].sprite = (i < estrelasParaMostrar) ? spriteEstrelaPreenchida : spriteEstrelaVazia;
        }

        if (textoResultado != null)
        {
            textoResultado.text = (estrelasParaMostrar >= estrelas.Length)
                ? mensagemTodasEstrelas
                : mensagemParcial;
        }
        else
        {
            Debug.LogWarning("textoResultado está NULO no EstrelasUI — não foi arrastado no Inspector.");
        }
    }

    /// <summary>
    /// Converte "acertos de primeira / total de perguntas" numa quantidade de 0 a 5 estrelas.
    /// </summary>
    int CalcularEstrelas(int acertosDePrimeira, int totalDePerguntas)
    {
        if (totalDePerguntas <= 0) return 0;

        float proporcao = (float)acertosDePrimeira / totalDePerguntas;
        int estrelasCalculadas = Mathf.RoundToInt(proporcao * estrelas.Length);

        return Mathf.Clamp(estrelasCalculadas, 0, estrelas.Length);
    }

    /// <summary>
    /// Esconde a tela de resultado (chame ao reiniciar o módulo).
    /// </summary>
    public void Esconder()
    {
        gameObject.SetActive(false);
    }
}