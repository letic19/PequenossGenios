using UnityEngine;


public class OpcoesMenu : MonoBehaviour
{
    private GameObject telaQueChamou;

    
    public void Abrir(GameObject telaAtual)
    {
        telaQueChamou = telaAtual;

        if (telaAtual != null)
            telaAtual.SetActive(false);

        gameObject.SetActive(true);
    }

   
    public void Voltar()
    {
        gameObject.SetActive(false);

        if (telaQueChamou != null)
        {
            telaQueChamou.SetActive(true);
        }
        else
        {
            Debug.LogWarning("OpcoesMenu: não sei pra qual tela voltar (Abrir() nunca foi chamado com uma tela válida).");
        }

        telaQueChamou = null;
    }
}
