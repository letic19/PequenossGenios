using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private Slider barra;
    [SerializeField] private TMP_Text textoProgresso;
    [SerializeField] private int totalAtividades = 20;

    private int atividadesConcluidas = 0;

    private void Start()
    {
        barra.minValue = 0;
        barra.maxValue = totalAtividades;

        AtualizarProgresso();
    }

    public void AvancarProgresso()
    {
        if (atividadesConcluidas < totalAtividades)
        {
            atividadesConcluidas++;
            AtualizarProgresso();
        }
    }

    private void AtualizarProgresso()
    {
        barra.value = atividadesConcluidas;
        textoProgresso.text = atividadesConcluidas + "/" + totalAtividades;
    }

    public void ReiniciarProgresso()
    {
        atividadesConcluidas = 0;
        AtualizarProgresso();
    }
}