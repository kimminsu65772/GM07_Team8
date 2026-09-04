using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using Newtonsoft.Json;

public class SaveScheduler : MonoBehaviour
{
    private static SaveScheduler instance;

    public static SaveScheduler Instance
    {
        get
        {
            if (instance == null)
            {
                SaveScheduler scheduler = FindFirstObjectByType<SaveScheduler>();
                if (scheduler == null)
                {
                    GameObject obj = new GameObject("SaveScheduler");
                    instance = obj.AddComponent<SaveScheduler>();
                }
                else
                {
                    instance = scheduler;
                }
            }

            return instance;
        }
    }

    [SerializeField, Min(1f)] private float autoSaveInterval = 30;
    [SerializeField, Min(0f)] private float soonToSaveDelay = 5;


    private PlayerSaveData saveData;
    private SaveDataWriter saveDataWriter;

    // 현재 진행 데이터의 변경 여부를 나타내는 플래그 변수
    private bool isDirty;
    private bool isSaving;
    // 저장 중에 저장 요청이 들어왔는가?
    private bool saveRequestedWhileSaving;
    // Soon 타입의 저장이 예약되어 있는가?
    private bool isSoonSaveScheduled;

    private CancellationTokenSource schedulerCTS;

    private void Awake()
    {
        schedulerCTS = new CancellationTokenSource();

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        AutoSaveAsync(schedulerCTS.Token).Forget();
    }

    public void Initialize(PlayerSaveData saveData, SaveDataWriter saveDataWriter)
    {
        if (saveData == null)
        {
            throw new ArgumentNullException(nameof(saveData), "PlayerSaveData가 초기화되지 않았습니다.");
        }
        if (saveDataWriter == null)
        {
            throw new ArgumentNullException(nameof(saveDataWriter), "SaveDataWriter가 초기화되지 않았습니다.");
        }

        this.saveData = saveData;
        this.saveDataWriter = saveDataWriter;
    }
    private void OnDestroy()
    {
        schedulerCTS?.Cancel();
        schedulerCTS?.Dispose();
    }

    // 자동 저장 메서드
    private async UniTaskVoid AutoSaveAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(autoSaveInterval), cancellationToken: cancellationToken);

                if (isDirty)
                {
                    await SaveAsync();
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    // Soon 타입의 저장 예약 메서드
    private async UniTaskVoid SoonToSaveAsync(CancellationToken cancellationToken)
    {
        // 이미 Soon 타입의 저장이 예약되어 있다면, 중복 예약을 방지한다.
        if (isSoonSaveScheduled)
        {
            return;
        }
        isSoonSaveScheduled = true;
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(soonToSaveDelay), cancellationToken: cancellationToken);
            if (isDirty)
            {
                await SaveAsync();
            }
        }
        catch (OperationCanceledException)
        {
            // 취소된 경우, 아무 작업도 수행하지 않는다.
        }
        finally
        {
            isSoonSaveScheduled = false;
        }
    }

    public void RequestSave(SavePolicy policy)
    {
        isDirty = true;

        if (isSaving)
        {
            saveRequestedWhileSaving = true;
        }

        switch (policy)
        {
            case SavePolicy.Deferred:
                // Defferred 정책은 자동 저장 주기 내에서 처리되므로, 별도의 작업 진행 X
                break;
            case SavePolicy.Soon:
                SoonToSaveAsync(schedulerCTS.Token).Forget();
                break;
            case SavePolicy.Immediate:
                SaveAsync().Forget();
                break;
            default:
                // 정의되지 않은 정책 타입이 들어오면 예외를 발생시킨다.
                throw new ArgumentOutOfRangeException(nameof(policy), policy, null);
        }
    }

    // FlushAsync 메서드는 isDirty 여부를 true로 설정하여, 다음 저장 작업에서 반드시 저장이 수행되도록 보장하는 메서드이다.
    // 주로 백그라운드 전환 시점이나 씬 전환 시점에서 호출되어, 현재까지의 데이터를 반드시 저장하도록 보장한다.
    public async UniTask FlushAsync()
    {
        isDirty = true;

        // 현재 저장 작업이 진행 중이라면 끝날 때까지 대기
        if (isSaving)
        {
            saveRequestedWhileSaving = true;
            await UniTask.WaitUntil(() => !isSaving);
        }

        await SaveAsync();
    }

    // 실질적인 저장 요청 메서드
    private async UniTask SaveAsync()
    {
        if (!isDirty)
        {
            return;
        }

        if (saveData == null || saveDataWriter == null)
        {
            return;
        }
        if (isSaving)
        {
            saveRequestedWhileSaving = true;
            return;
        }

        isSaving = true;

        try
        {
            // 데이터를 JSON으로 직렬화하고 저장할 것을 Writer에게 요청한다.
            // 그 후 saveRequestedWhileSaving 플래그를 확인하여, 추가 저장 요청이 있었는지 확인하고, 있다면 다시 저장을 수행한다.
            do
            {
                // 현재 저장 시도는 곧 이전까지의 변경 사항을 모드 저장하는 것이므로 saveRequestedWhileSaving 플래그를 초기화한다.
                saveRequestedWhileSaving = false;

                saveData.LastSavedAtUtc = DateTime.UtcNow.ToString("o");

                string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);

                isDirty = false;

                await saveDataWriter.SaveFileAsync(json);
            } while (saveRequestedWhileSaving);
        }
        catch (Exception)
        {
            // 저장 중 에러가 발생하면 현재 파일이 결국 저장되지 않았음을 의미하므로, isDirty를 true로 설정하여 다음 저장 시도를 보장한다.
            isDirty = true;
        }
        finally
        {
            isSaving = false;
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            FlushAsync().Forget();
        }
    }

    private void OnApplicationQuit()
    {
        // 강제 저장은 Writer에서 즉시 이루어지므로 이곳에서 유효성 검사를 수행해야 한다.
        if (saveData != null && saveDataWriter != null)
        {
            saveDataWriter.ForceSave(saveData);
        }
    }
}
