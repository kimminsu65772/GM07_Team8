using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StageTransitionController : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private BattleManager battleManager;

    [SerializeField] private float fadeDuration;

    private Coroutine transitionRoutine;


    public bool StartTransition(int nextStageNumber)
    {
        if (transitionRoutine != null)
        {
            return false;
        }

        transitionRoutine =
            StartCoroutine(TransitionToNextStage(nextStageNumber));

        return true;
    }

    private IEnumerator TransitionToNextStage(int nextStageNumber)
    {
        battleManager.StopStage();

        //TODO: 화면 암전 완료 대기
        yield return FadeOutScreen();

        // 배틀 매니저에게 다음 스테이지 세팅 요청
        battleManager.SetUpStage(nextStageNumber);
        yield return new WaitForSeconds(0.5f); // 스테이지 세팅 완료 대기
        yield return FadeInScreen();
        battleManager.StartStage();

        transitionRoutine = null;
    }

    private IEnumerator FadeOutScreen()
    {
        fadeImage.gameObject.SetActive(true);
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }
    }

    private IEnumerator FadeInScreen()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(1f - (elapsedTime / fadeDuration));
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }
        fadeImage.gameObject.SetActive(false);
    }
}
