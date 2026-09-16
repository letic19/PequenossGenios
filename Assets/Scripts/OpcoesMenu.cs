using UnityEngine;

/// <summary>
/// Coloque este script no MESMO painel de Opções que já existe na tela inicial
/// (não crie um painel novo — é pra ser um único objeto compartilhado, pra ficar
/// visualmente idêntico não importa de onde foi aberto).
///
/// Ele lembra qual tela pediu pra abrir as Opções (Menu Principal ou a Pausa de um
/// módulo), e o botão "Voltar" retorna pra essa tela certa automaticamente.
/// </summary>
public class OpcoesMenu : MonoBehaviour
{
    private GameObject telaQueChamou;

    /// <summary>
    /// Chame isso a partir de QUALQUER lugar que precise abrir as Opções,
    /// passando a tela atual (que deve ser escondida e reaberta depois).
    /// </summary>
    public void Abrir(GameObject telaAtual)
    {
        telaQueChamou = telaAtual;

        if (telaAtual != null)
            telaAtual.SetActive(false);

        gameObject.SetActive(true);
    }

    /// <summary>
    /// Ligue essa função ao OnClick do botão "Voltar" dentro da tela de Opções.
    /// Funciona tanto vindo do Menu Principal quanto da Pausa de qualquer módulo.
    /// </summary>
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
