using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class LetterSlot : MonoBehaviour, IDropHandler
{
    private string letraCorreta;
    private bool preenchido = false;

    private TextMeshProUGUI letterText;

    private VogaisGameManager gameManager;

    void Awake()
    {
        letterText = GetComponentInChildren<TextMeshProUGUI>(true);

        if (letterText == null)
        {
            Debug.LogError("Não encontrei o LetterText dentro do prefab LetterSlot.");
        }
    }

    public void Inicializar(char letra, VogaisGameManager manager)
    {
        letraCorreta = letra.ToString();
        gameManager = manager;

        letterText.text = "_";
        preenchido = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (preenchido)
        {
            Debug.Log($"OnDrop ignorado: slot da letra '{letraCorreta}' já estava preenchido.");
            return;
        }

        DragItem item = eventData.pointerDrag.GetComponent<DragItem>();

        if (item == null)
        {
            Debug.LogWarning("OnDrop: o objeto solto não tem componente DragItem. Ignorando.");
            return;
        }

        string letraArrastada = item.gameObject.name.Replace("(Clone)", "").Trim().ToUpper();

        Debug.Log($"OnDrop no slot '{letraCorreta}': letra arrastada = '{letraArrastada}'");

        if (letraArrastada == letraCorreta)
        {
            preenchido = true;

            letterText.text = letraCorreta;

            item.VoltarAoInicio();
            //item.gameObject.SetActive(false);

            gameManager.LetraCorreta();
        }
        else
        {
            // Letra errada: avisa o GameManager (feedback + conta como erro pra essa palavra)
            gameManager.LetraErrada();

            item.VoltarAoInicio();
        }
    }
}