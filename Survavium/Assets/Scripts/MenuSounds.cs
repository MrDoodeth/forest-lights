using UnityEngine;

public class MenuSounds : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioHover;
    [SerializeField] private AudioClip audioClick;
    public void HoverSound()
    {
        audioSource.PlayOneShot(audioHover, 1f);
    }

    public void ClickSound()
    {
        audioSource.PlayOneShot(audioClick, 1f);
    }
}
