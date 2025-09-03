using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.EventSystems;

public class NextWaveButton : MonoBehaviour
{
    void Start()
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject
          && Input.GetButtonDown(GameConstants.k_ButtonNameSubmit))
        {
            OnNextWaveButtonPressed();
        }
    }

    public void OnNextWaveButtonPressed()
    {
        Debug.Log("Next wave button pressed");
        EventManager.Broadcast(Events.WaveStartedEvent);
    }
}
