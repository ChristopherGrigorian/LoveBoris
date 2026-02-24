using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonNoises : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField] private Button thisButton;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip hoverClip;
    [SerializeField] private AudioClip hoverExitClip;
    [SerializeField] private AudioClip pressedClip;

    private void Start()
    {
        thisButton.onClick.AddListener(() => PlayClickNoise());
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        audioSource.PlayOneShot(hoverClip, 0.5f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        audioSource.PlayOneShot(hoverExitClip, 0.5f);
    }

    public void PlayClickNoise()
    {
        if (audioSource != null && audioSource.enabled && audioSource.gameObject.activeInHierarchy)
        {
            audioSource.PlayOneShot(pressedClip, 0.5f);
        }
    }

    public void AddAudioSource(AudioSource audioSource)
    {
        this.audioSource = audioSource;
    }

    public void AddHoverClip(AudioClip audioClip)
    {
        this.hoverClip = audioClip;
    }

    public void AddPressedClip(AudioClip audioClip)
    {
        this.pressedClip = audioClip;
    }

    public void AssignSelf(Button self)
    {
        thisButton = self;
    }
}
