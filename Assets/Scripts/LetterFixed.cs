using UnityEngine;
using TMPro;

public class LetterFixed : MonoBehaviour
{
    private TextMeshProUGUI texto;

    void Awake()
    {
        texto = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void DefinirLetra(char letra)
    {
        texto.text = letra.ToString();
    }
}