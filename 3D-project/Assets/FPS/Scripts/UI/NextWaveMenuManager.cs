using Unity.FPS.AI;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;

public class NextWaveMenuManager : MonoBehaviour
{
    [Tooltip("Root GameObject of the menu used to toggle its activation")]
    public GameObject MenuRoot;

    [Tooltip("Master volume when menu is open")]
    [Range(0.001f, 1f)]
    public float VolumeWhenMenuOpen = 0.5f;

    [Tooltip("Upgrade buttons parent to hide when chosen.")]
    public GameObject UpgradeButtons;

    private WaveManager m_WaveManager;

    private bool hasChosen = false;

    void Start()
    {
        m_WaveManager = FindObjectOfType<WaveManager>();
        if (!m_WaveManager)
        {
            Debug.LogError("Could not find WaveManager in scene");
            return;
        }

        MenuRoot.SetActive(false);

        EventManager.AddListener<WaveCompletedEvent>(OnWaveCompleted);
    }

    void Update()
    {
        if (!MenuRoot.activeSelf && Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void OnWaveCompleted(WaveCompletedEvent evt)
    {
        SetMenuActive(true);
    }

    void SetMenuActive(bool active)
    {
        MenuRoot.SetActive(active);

        if (active)
        {
            hasChosen = false;
            UpgradeButtons.SetActive(true);
            Debug.Log("Setting menu active");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
            AudioUtility.SetMasterVolume(VolumeWhenMenuOpen);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
            AudioUtility.SetMasterVolume(1f);
        }
    }

    public void OnNextWaveButtonPressed()
    {
        if (hasChosen)
        {
            SetMenuActive(false);
            EventManager.Broadcast(Events.WaveStartedEvent);
        }
    }

    public void OnHealthButtonPressed()
    {
        if (!hasChosen)
        {
            Debug.Log("Upgraded health by 1");
            Health health = FindObjectOfType<Health>();
            if (health != null)
                health.MaxHealth++;
            hasChosen = true;
            UpgradeButtons.SetActive(false);
        }
    }

    public void OnSpeedButtonPressed()
    {
        if (!hasChosen)
        {
            Debug.Log("Upgraded speed by 2");
            PlayerCharacterController playerCharacterController = FindObjectOfType<PlayerCharacterController>();
            if (playerCharacterController != null)
            {
                playerCharacterController.MaxSpeedOnGround += 2;
            }
            hasChosen = true;
            UpgradeButtons.SetActive(false);
        }
    }

    public void OnWeaponButtonPressed()
    {
        if (!hasChosen)
        {
            Debug.Log("Upgraded weapon max ammo by 4");
 
            WeaponController weaponController = FindObjectOfType<WeaponController>();
            if (weaponController != null)
            {
                weaponController.MaxAmmo += 4;
                weaponController.AmmoReloadRate += 4;
            }
            hasChosen = true;
            UpgradeButtons.SetActive(false);
        }
    }


    private void OnDestroy()
    {
        EventManager.RemoveListener<WaveCompletedEvent>(OnWaveCompleted);
    }
}
