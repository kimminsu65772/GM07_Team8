using System.Collections.Generic;
using UnityEngine;

// Project 창의 Create 메뉴에서
// StageData 타입의 SO 파일을 생성할 수 있도록 설정한다.
[CreateAssetMenu(
    fileName = "NewStageData",
    menuName = "Game Data/Stage/Stage Data"
)]
public class StageData : ScriptableObject
{
    // 스테이지를 구분하기 위한 번호
    [SerializeField, Min(1)]
    private int stageNumber = 1;

    // 리스트에 등록된 순서대로 웨이브가 진행된다.
    [SerializeField]
    private List<WaveData> waves =
        new List<WaveData>();

    public int StageNumber => stageNumber;

    // IReadOnlyList를 사용해 외부에서 리스트를 직접 수정하지 못하게 한다.
    public IReadOnlyList<WaveData> Waves =>
        waves;
}