using Unity.FPS.AI;
using Unity.FPS.Game;
using UnityEngine;

public class NextWaveMenuManager : MonoBehaviour
{
    [Tooltip("Root GameObject of the menu used to toggle its activation")]
    public GameObject MenuRoot;

    [Tooltip("Master volume when menu is open")]
    [Range(0.001f, 1f)]
    public float VolumeWhenMenuOpen = 0.5f;

    private WaveManager m_WaveManager;

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
        SetMenuActive(false);
        EventManager.Broadcast(Events.WaveStartedEvent);
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener<WaveCompletedEvent>(OnWaveCompleted);
    }
}
