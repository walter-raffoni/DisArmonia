using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Objects")]
    public Player player;
    [SerializeField] GameObject pauseObject;
    [SerializeField] Slider barraVita;
    public Slider stackDiSangue;

    [Header("Texts")]
    [SerializeField] TextMeshProUGUI stateText;

    #region Campi vari
    public static GameManager instance;
    [HideInInspector] public bool isPaused;
    #endregion

    private void Awake() => instance = this;

    private void Start() => Time.timeScale = 1;

    private void Update()
    {
        if (stateText != null) stateText.text = player.stateMachine.currentState.ToString();

        barraVita.value = player.currentHP;
        stackDiSangue.value = player.stackDiSangue;
    }

    public void PauseGame()
    {
        if (!isPaused)
        {
            isPaused = true;
            pauseObject.SetActive(true);
            stateText.gameObject.SetActive(false);
            Time.timeScale = 0;
        }
        else
        {
            isPaused = false;
            pauseObject.SetActive(false);
            stateText.gameObject.SetActive(true);
            Time.timeScale = 1;
        }
    }
}