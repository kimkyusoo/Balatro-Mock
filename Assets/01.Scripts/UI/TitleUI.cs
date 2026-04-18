using UnityEngine;

public class TitleUI : MonoBehaviour
{
    public AudioClip titleMusic;

    private void Start()
    {
        if (SoundManager.Instance != null && titleMusic != null)
        {
            SoundManager.Instance.PlayBgm(titleMusic, 0.3f);
        }
    }
}
