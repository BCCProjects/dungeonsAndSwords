using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

// Interface das fases: HUD do jogador, barra do chefe, level up, pause e fim de fase.
public class UIManager : MonoBehaviour
{
    [Header("HUD do jogador")]
    [SerializeField] private TMP_Text classLevelText;
    [SerializeField] private RectTransform healthFill;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private RectTransform xpFill;
    [SerializeField] private TMP_Text xpText;
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private TMP_Text effectsText;

    [Header("Chefe")]
    [SerializeField] private GameObject bossBar;
    [SerializeField] private TMP_Text bossNameText;
    [SerializeField] private RectTransform bossFill;

    [Header("Level up")]
    [SerializeField] private GameObject levelUpPanel;
    [SerializeField] private TMP_Text levelUpTitle;

    [Header("Fim de fase")]
    [SerializeField] private TMP_Text levelMessage;
    [SerializeField] private GameObject levelCompletePanel;

    [Header("Pause")]
    [SerializeField] private GameObject pausePanel;

    private LevelSystem levelSystem;
    private PlayerEffects playerEffects;
    private Health playerHealth;
    private Health bossHealth;

    private void Start()
    {
        levelUpPanel.SetActive(false);
        levelCompletePanel.SetActive(false);
        levelMessage.gameObject.SetActive(false);
        bossBar.SetActive(false);
        pausePanel.SetActive(false);

        levelSystem = FindFirstObjectByType<LevelSystem>();
        if (levelSystem == null) return;
        playerEffects = levelSystem.GetComponent<PlayerEffects>();
        playerHealth = levelSystem.GetComponent<Health>();

        levelSystem.LevelUpReady += ShowLevelUp;
        levelSystem.XPChanged += UpdateXP;
        levelSystem.StatsChanged += UpdateStats;
        playerHealth.HealthChanged += UpdateHealth;

        UpdateHealth(playerHealth.Current, playerHealth.Max);
        UpdateXP(GameManager.Instance.Progress.xp, levelSystem.XpToNextLevel, levelSystem.Level);
        UpdateStats();
    }

    private void OnDestroy()
    {
        if (levelSystem != null)
        {
            levelSystem.LevelUpReady -= ShowLevelUp;
            levelSystem.XPChanged -= UpdateXP;
            levelSystem.StatsChanged -= UpdateStats;
        }
        if (playerHealth != null) playerHealth.HealthChanged -= UpdateHealth;
        if (bossHealth != null) bossHealth.HealthChanged -= UpdateBoss;
    }

    private void Update()
    {
        UpdateEffects();

        bool pausePressed = (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
                         || (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame);
        if (pausePressed) TogglePause();
    }

    // ---------- Pause ----------

    // Não pausa por cima do level up nem do painel de fase concluída.
    public void TogglePause()
    {
        if (levelUpPanel.activeSelf || levelCompletePanel.activeSelf) return;
        bool pause = !pausePanel.activeSelf;
        pausePanel.SetActive(pause);
        Time.timeScale = pause ? 0f : 1f;
    }

    public void Resume()
    {
        if (pausePanel.activeSelf) TogglePause();
    }

    // ---------- HUD ----------

    private void UpdateHealth(int current, int max)
    {
        SetFill(healthFill, current / (float)max);
        healthText.text = $"{current}/{max}";
    }

    private void UpdateXP(int xp, int needed, int level)
    {
        SetFill(xpFill, xp / (float)needed);
        xpText.text = $"XP {xp}/{needed}";
        classLevelText.text = $"{levelSystem.ClassName}  -  Nível {level}";
    }

    private void UpdateStats()
    {
        statsText.text = $"Dano {levelSystem.Damage}   Velocidade {levelSystem.Speed:0.00}x   Resistência {levelSystem.Resistance * 100f:0}%";
    }

    // Poções ativas com o tempo que falta.
    private void UpdateEffects()
    {
        if (playerEffects == null) return;
        string text = "";
        foreach (PlayerEffects.ActiveEffect e in playerEffects.Active)
            text += $"<color=#{ColorUtility.ToHtmlStringRGB(e.item.color)}>{EffectLabel(e.item)}</color>  {Mathf.CeilToInt(e.remaining)}s\n";
        effectsText.text = text;
    }

    private static string EffectLabel(ItemData item)
    {
        switch (item.type)
        {
            case ItemData.ItemType.Strength: return $"Força +{item.amount:0} dano";
            case ItemData.ItemType.Agility: return $"Agilidade +{item.amount * 100f:0}% velocidade";
            case ItemData.ItemType.Resistance: return $"Resistência -{item.amount * 100f:0}% dano";
            default: return item.displayName;
        }
    }

    private static void SetFill(RectTransform fill, float fraction)
    {
        fill.anchorMax = new Vector2(Mathf.Clamp01(fraction), 1f);
    }

    // ---------- Chefe ----------

    public void ShowBossBar(Health boss, string bossName)
    {
        bossHealth = boss;
        bossNameText.text = bossName;
        bossBar.SetActive(true);
        boss.HealthChanged += UpdateBoss;
        UpdateBoss(boss.Current, boss.Max);
    }

    public void HideBossBar() => bossBar.SetActive(false);

    private void UpdateBoss(int current, int max) => SetFill(bossFill, current / (float)max);

    // ---------- Level up ----------

    private void ShowLevelUp(int level)
    {
        levelUpTitle.text = $"Nível {level}!\nEscolha um atributo";
        levelUpPanel.SetActive(true);
    }

    // Botões do painel (ligados pelo OnClick no Inspector).
    public void ChooseHealth() => Choose(LevelSystem.Attribute.Health);
    public void ChooseStrength() => Choose(LevelSystem.Attribute.Strength);
    public void ChooseAgility() => Choose(LevelSystem.Attribute.Agility);
    public void ChooseResistance() => Choose(LevelSystem.Attribute.Resistance);

    private void Choose(LevelSystem.Attribute attribute)
    {
        levelUpPanel.SetActive(false);
        levelSystem.ChooseAttribute(attribute);
    }

    // ---------- Fim de fase ----------

    public void ShowLevelMessage(string message)
    {
        levelMessage.gameObject.SetActive(true);
        levelMessage.text = message;
    }

    public void ShowLevelComplete()
    {
        levelMessage.gameObject.SetActive(false);
        levelCompletePanel.SetActive(true);
        Time.timeScale = 0f;
        AudioManager.Play(AudioManager.Sfx.LevelComplete);
    }

    // Botões do painel de fase concluída.
    public void NextLevel() => SceneLoader.LoadNextLevel();
    public void BackToMenu() => SceneLoader.LoadMainMenu();
}
