using UnityEngine;

public class PalavraBuilder : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject letterFixedPrefab;
    public GameObject letterSlotPrefab;

    public void MontarPalavra(string palavra, VogaisGameManager gameManager)
    {
        Limpar();

        palavra = palavra.ToUpper();

        foreach (char letra in palavra)
        {
            if ("AEIOU".Contains(letra.ToString()))
            {
                GameObject slot = Instantiate(letterSlotPrefab, transform);

                Debug.Log("Objeto criado: " + slot.name);

                LetterSlot letterSlot = slot.GetComponent<LetterSlot>();

                Debug.Log("Script encontrado? " + (letterSlot != null));

                letterSlot.Inicializar(letra, gameManager);
            }
            else
            {
                GameObject letraObj = Instantiate(letterFixedPrefab, transform);

                LetterFixed letraScript = letraObj.GetComponent<LetterFixed>();

                letraScript.DefinirLetra(letra);
            }
        }
    }

    public void Limpar()
    {
        foreach (Transform filho in transform)
        {
            Destroy(filho.gameObject);
        }
    }
}