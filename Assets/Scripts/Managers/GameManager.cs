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
        Time.timeScale = 1;

        float cooldownStance = player.CooldownStanceAttuale;
        float cooldownDash = player.CooldownDashAttuale;
        float cooldownAttaccoPotente = player.CooldownAttaccoPotenteAttuale;

        cooldownStance = Mathf.Round(cooldownStance);

        if (cooldownStance < 0) cooldownStance = 0;
        if (cooldownDash < 0) cooldownDash = 0;
        if (cooldownAttaccoPotente < 0) cooldownAttaccoPotente = 0;

        stanceChangeCooldownText.text = "Cooldown Stance: " + cooldownStance.ToString("F2") + "s";
        dashCooldownText.text = "Cooldown Dash: " + cooldownDash.ToString("F2") + "s";
        attaccoPotenteCooldownText.text = "Cooldown Attacco Potente: " + cooldownAttaccoPotente.ToString("F2") + "s";
    }

    private void Update()
    {
        float cooldownStance = player.CooldownStanceAttuale;
        float cooldownDash = player.CooldownDashAttuale;
        float cooldownAttaccoPotente = player.CooldownAttaccoPotenteAttuale;

        if (cooldownStance < 0) cooldownStance = 0;
        if (cooldownDash < 0) cooldownDash = 0;
        if (cooldownAttaccoPotente < 0) cooldownAttaccoPotente = 0;

        stanceChangeCooldownText.text = "Cooldown Stance: " + cooldownStance.ToString("F2") + "s";
        dashCooldownText.text = "Cooldown Dash: " + cooldownDash.ToString("F2") + "s";
        attaccoPotenteCooldownText.text = "Cooldown Attacco Potente: " + cooldownAttaccoPotente.ToString("F2") + "s";

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