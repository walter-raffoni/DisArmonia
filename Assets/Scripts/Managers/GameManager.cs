using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Player")]
    public Player player;
    [SerializeField] TextMeshProUGUI stateText;

    [SerializeField] GameObject pauseObject;

    public static GameManager instance;

    #region Campi visibili ma non modificabili
    public bool IsPaused => isPaused;
    #endregion

    #region Campi privati
    private bool isPaused;
    #endregion

    private void Awake() => instance = this;

    private void Start() => Time.timeScale = 1;

    private void Update()
    {
        if (stateText != null) stateText.text = player.stateMachine.currentState.ToString();
    }

    public void PauseGame()
    {
        if (!isPaused)
        {
            isPaused = true;
            pauseObject.SetActive(true);
            player.BarraVita.gameObject.SetActive(true);
            player.BarraStackDiSangue.gameObject.SetActive(true);
            stateText.gameObject.SetActive(false);
            Time.timeScale = 0;
        }
        else
        {
            isPaused = false;
            pauseObject.SetActive(false);
            player.BarraVita.gameObject.SetActive(false);
            player.BarraStackDiSangue.gameObject.SetActive(false);
            stateText.gameObject.SetActive(true);
            Time.timeScale = 1;
        }
    }
}