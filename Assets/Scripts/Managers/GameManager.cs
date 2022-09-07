using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Player System")]
    [SerializeField] Player player;
    [SerializeField] Slider barraVita;
    [SerializeField] Slider barraStackDiSangue;
    [SerializeField] TextMeshProUGUI dashCooldownText;
    [SerializeField] TextMeshProUGUI stanceChangeCooldownText;
    [SerializeField] TextMeshProUGUI attaccoPotenteCooldownText;

    [Header("Pause System")]
    [SerializeField] GameObject pauseObject;

    #region Campi pubblici
    public static GameManager instance;
    public bool IsPaused => isPaused;
    public Slider BarraVita => barraVita;
    public Slider BarraStackDiSangue => barraStackDiSangue;
    #endregion

    #region Campi privati
    private bool isPaused;
    private ExtendedInputActions input;
    #endregion

    private void Awake()
    {
        instance = this;
        input = instance.GetComponent<ExtendedInputActions>();
    }

    private void Start()
    {
        Time.timeScale = 0.25f;//1

        int cooldownStance = (int)player.CooldownStanceAttuale;
        int cooldownDash = (int)player.CooldownDashAttuale;
        int cooldownAttaccoPotente = (int)player.CooldownAttaccoPotenteAttuale;

        stanceChangeCooldownText.text = "Cooldown Stance: " + cooldownStance.ToString() + "s";
        dashCooldownText.text = "Cooldown Dash: " + cooldownDash.ToString() + "s";
        attaccoPotenteCooldownText.text = "Cooldown Attacco Potente: " + cooldownAttaccoPotente.ToString() + "s";
    }

    private void Update()
    {
        int cooldownStance = (int)player.CooldownStanceAttuale;
        int cooldownDash = (int)player.CooldownDashAttuale;
        int cooldownAttaccoPotente = (int)player.CooldownAttaccoPotenteAttuale;

        stanceChangeCooldownText.text = "Cooldown Stance: " + cooldownStance.ToString() + "s";
        dashCooldownText.text = "Cooldown Dash: " + cooldownDash.ToString() + "s";
        attaccoPotenteCooldownText.text = "Cooldown Attacco Potente: " + cooldownAttaccoPotente.ToString() + "s";

        if (input.HandleInput().PauseDown) instance.PauseGame();

        if (input.HandleInput().ReloadGameDown) SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void PauseGame()
    {
        if (!isPaused)
        {
            isPaused = true;
            pauseObject.SetActive(true);
            barraVita.gameObject.SetActive(false);
            barraStackDiSangue.gameObject.SetActive(false);
            stanceChangeCooldownText.gameObject.SetActive(false);
            attaccoPotenteCooldownText.gameObject.SetActive(false);
            Time.timeScale = 0;
        }
        else
        {
            isPaused = false;
            pauseObject.SetActive(false);
            barraVita.gameObject.SetActive(true);
            barraStackDiSangue.gameObject.SetActive(true);
            stanceChangeCooldownText.gameObject.SetActive(true);
            attaccoPotenteCooldownText.gameObject.SetActive(true);
            Time.timeScale = 1;
        }
    }
}