using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageRepeatModeButton : MonoBehaviour
{
    [SerializeField] private Button modeButton;
    [SerializeField] private RectTransform rotationArrow;
    [SerializeField] private Image buttonStateText;
    [SerializeField] private Sprite[] textImages;

    private Tween rotationTween;

    private void Awake()
    {
        if (modeButton == null)
        {
            modeButton = GetComponent<Button>();
        }
    }

    private void OnEnable()
    {
        if (modeButton != null)
        {
            modeButton.onClick.AddListener(ToggleMode);
        }

        Refresh();
    }

    private void OnDisable()
    {
        if (modeButton != null)
        {
            modeButton.onClick.RemoveListener(ToggleMode);
        }

        if (rotationTween != null && rotationTween.IsActive())
        {
            rotationTween.Kill();
            rotationTween = null;
        }

        if (rotationArrow != null)
        {
            rotationArrow.localRotation = Quaternion.identity;
        }
    }

    private void ToggleMode()
    {
        PlayerInfo playerInfo = PlayerInfo.Instance;
        if (playerInfo == null || !playerInfo.IsInitialized)
        {
            return;
        }

        bool nextRepeatMode = !playerInfo.RepeatClearedStage;

        if (playerInfo.SetRepeatClearedStage(nextRepeatMode))
        {
            Refresh();
        }
    }

    private void Refresh()
    {
        PlayerInfo playerInfo = PlayerInfo.Instance;
        if (buttonStateText == null || playerInfo == null || !playerInfo.IsInitialized)
        {
            return;
        }

        if (textImages == null || textImages.Length < 2)
        {
            Debug.LogWarning("StageRepeatModeButton: 텍스트 이미지가 할당되지 않았습니다.");
            return;
        }

        buttonStateText.sprite = !playerInfo.RepeatClearedStage ? textImages[0] : textImages[1];

        if (playerInfo.RepeatClearedStage)
        {
            RotateArrowDO();
        }
        else
        {
            if (rotationArrow != null)
            {
                rotationArrow.DOKill();
                rotationArrow.localRotation = Quaternion.identity;
            }
        }
    }

    private void RotateArrowDO()
    {
        if (rotationArrow == null)
        {
            return;
        }

        if (rotationTween != null && rotationTween.IsActive())
        {
            rotationTween.Kill();
            rotationTween = null;
        }

        rotationTween = rotationArrow.DOLocalRotate(new Vector3(0, 0, -360), 1, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
    }
}
