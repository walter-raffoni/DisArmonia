using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Player System")]
    public Player player;
    [SerializeField] TextMeshProUGUI stanceChangeCooldownText;

    [Header("Pause System")]
    [SerializeField] GameObject pauseObject;

    #region Campi pubblici
    public static GameManager instance;
    public bool IsPaused => isPaused;
    #endregion

    #region Campi privati
    private bool isPaused;
    #endregion

    private void Awake() => instance = this;

    private void Start()
    {
        Time.timeScale = 1;

        int cooldown = (int)player.CooldownStanceAttuale;
        stanceChangeCooldownText.text = "Cooldown Stance: " + cooldown.ToString() + "s";
    }

    private void Update()
    {
        int cooldown = (int)player.CooldownStanceAttuale;
        stanceChangeCooldownText.text = "Cooldown Stance: " + cooldown.ToString() + "s";
    }

    public void PauseGame()
    {
        if (!isPaused)
        {
            isPaused = true;
            pauseObject.SetActive(true);
            player.BarraVita.gameObject.SetActive(false);
            player.BarraStackDiSangue.gameObject.SetActive(false);
            stanceChangeCooldownText.gameObject.SetActive(false);
            Time.timeScale = 0;
        }
        else
        {
            isPaused = false;
            pauseObject.SetActive(false);
            player.BarraVita.gameObject.SetActive(true);
            player.BarraStackDiSangue.gameObject.SetActive(true);
            stanceChangeCooldownText.gameObject.SetActive(true);
            Time.timeScale = 1;
        }
    }
}