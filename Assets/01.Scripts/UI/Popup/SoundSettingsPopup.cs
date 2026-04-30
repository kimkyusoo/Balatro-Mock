using UnityEngine;
using UnityEngine.UI;

public class SoundSettingsPopup : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(() => gameObject.SetActive(false));
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    public void Open()
    {
        gameObject.SetActive(true);

        if (SoundManager.Instance != null)
        {
            volumeSlider.value = SoundManager.Instance.currentVolume;
        }
    }

    private void OnVolumeChanged(float value)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetMasterVolume(value);
        }
    }
}