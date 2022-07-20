using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] GameObject canvasToEnable;
    [SerializeField] GameObject canvasToDisable;

    public void GoToGame() => SceneManager.LoadScene(1, LoadSceneMode.Single);//1: Menù principale

    public void ToggleCanvases()
    {
        canvasToDisable.SetActive(false);
        canvasToEnable.SetActive(true);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void AddStack() => GameManager.instance.player.StackDiSangue++;

    public void RemoveStack() => GameManager.instance.player.StackDiSangue--;
}