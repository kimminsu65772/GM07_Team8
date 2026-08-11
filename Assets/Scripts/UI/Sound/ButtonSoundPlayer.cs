using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSoundPlayer : MonoBehaviour, IPointerEnterHandler
{
    [Header("효과음 설정")]
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;
    [Range(0f, 1f)]
    [SerializeField] private float soundVolume = 1f;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(PlayClickSound);
        }  
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSound != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(hoverSound, soundVolume);
        }
    }
    private void PlayClickSound()
    {
        if (clickSound != null && SoundManager.Instance != null) 
        {
            SoundManager.Instance.PlaySound(clickSound, soundVolume);
        }
    }
}
