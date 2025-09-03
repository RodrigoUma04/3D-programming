using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.FPS.Game
{
    public class GameFlowManager : MonoBehaviour
    {
        [Header("Parameters")]
        [Tooltip("Duration of the fade-to-black at the end of the game")]
        public float EndSceneLoadDelay = 3f;

        [Tooltip("The canvas group of the fade-to-black screen")]
        public CanvasGroup EndGameFadeCanvasGroup;

        [Header("Lose")]
        [Tooltip("This string has to be the name of the scene you want to load when losing")]
        public string LoseSceneName = "LoseScene";

        [Header("Wave UI")]
        [Tooltip("Panel shown when a wave is completed")]
        public GameObject WaveCompletePanel;

        [Tooltip("Sound played on wave complete")]
        public AudioClip VictorySound;

        [Tooltip("Win game message (optional text in HUD)")]
        public string WinGameMessage;

        [Tooltip("Delay before showing the win message")]
        public float DelayBeforeWinMessage = 2f;

        public bool GameIsEnding { get; private set; }

        float m_TimeLoadEndGameScene;
        string m_SceneToLoad;

        void Awake()
        {
            EventManager.AddListener<PlayerDeathEvent>(OnPlayerDeath);
            EventManager.AddListener<WaveStartedEvent>(OnWaveStarted);
            EventManager.AddListener<WaveCompletedEvent>(OnWaveCompleted);
        }

        void Start()
        {
            AudioUtility.SetMasterVolume(1);
        }

        void Update()
        {
            if (GameIsEnding)
            {
                float timeRatio = 1 - (m_TimeLoadEndGameScene - Time.time) / EndSceneLoadDelay;
                EndGameFadeCanvasGroup.alpha = timeRatio;

                AudioUtility.SetMasterVolume(1 - timeRatio);

                if (Time.time >= m_TimeLoadEndGameScene)
                {
                    SceneManager.LoadScene(m_SceneToLoad);
                    GameIsEnding = false;
                }
            }
        }

        void OnPlayerDeath(PlayerDeathEvent evt) => HandleGameOver();

        void OnWaveStarted(WaveStartedEvent evt)
        {
            Debug.Log("Wave " + evt.WaveNumber + " started");
            // Could hide panel here if needed
            if (WaveCompletePanel != null)
            {
                WaveCompletePanel.SetActive(false);
                Time.timeScale = 1f;
            }
        }

        void OnWaveCompleted(WaveCompletedEvent evt)
        {
            Debug.Log("Wave " + evt.WaveNumber + " completed");

            // Show the upgrade / wave complete panel
            if (WaveCompletePanel != null)
            {
                WaveCompletePanel.SetActive(true);
                Time.timeScale = 0f;
            }

            // Play victory sound
            if (VictorySound)
            {
                var audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = VictorySound;
                audioSource.playOnAwake = false;
                audioSource.outputAudioMixerGroup = AudioUtility.GetAudioGroup(AudioUtility.AudioGroups.HUDVictory);
                audioSource.Play();
            }
        }

        void HandleGameOver()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            GameIsEnding = true;
            EndGameFadeCanvasGroup.gameObject.SetActive(true);

            m_SceneToLoad = LoseSceneName;
            m_TimeLoadEndGameScene = Time.time + EndSceneLoadDelay;
        }

        void OnDestroy()
        {
            EventManager.RemoveListener<PlayerDeathEvent>(OnPlayerDeath);
            EventManager.AddListener<WaveStartedEvent>(OnWaveStarted);
            EventManager.AddListener<WaveCompletedEvent>(OnWaveCompleted);
        }
    }
}