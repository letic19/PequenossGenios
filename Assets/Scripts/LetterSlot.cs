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
        Debug.Log("Inicializando Slot");

        letraCorreta = letra.ToString();
        gameManager = manager;

        Debug.Log("LetterText é nulo? " + (letterText == null));
        Debug.Log("GameManager é nulo? " + (gameManager == null));

        letterText.text = "_";
        preenchido = false;
}

    public void OnDrop(PointerEventData eventData)
    {
        if (preenchido)
            return;

        DragItem item = eventData.pointerDrag.GetComponent<DragItem>();

        if (item == null)
            return;

        string letraArrastada = item.gameObject.name.Replace("(Clone)", "").Trim().ToUpper();

        if (letraArrastada == letraCorreta)
        {
            preenchido = true;

            letterText.text = letraCorreta;
            Debug.Log("Mostrando letra: " + letraCorreta);

            item.foiColocada = true;
            item.gameObject.SetActive(false);

            gameManager.LetraCorreta();
        }
    }
}