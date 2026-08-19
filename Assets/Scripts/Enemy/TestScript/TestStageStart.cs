using UnityEngine;

public class StageTestStarter : MonoBehaviour
{
    [SerializeField] private StageManager stageManager;
    [SerializeField] private int stageNumber = 1;

    private void Start()
    {
        stageManager.StartStage(stageNumber);
    }
}