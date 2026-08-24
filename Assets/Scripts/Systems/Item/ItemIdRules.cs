
// TODO: 아이템 타입이 추가될 경우, ItemType 열거형에 추가하고, ItemSO에서 해당 타입에 대한 유효성 검사를 추가해야 함.
public enum ItemType
{
    Material,
}

public static class ItemIdRules
{
    public const int MaterialMin = 10000;
    public const int MaterialMax = 19999;

    // TODO: 아이템 타입이 추가될 경우, Max 값을 더 늘려야 함.
    public const int ItemIdMin = 10000;
    public const int ItemIdMax = 19999;


    // TODO: 아이템 타입이 추가될 경우, 유효 값과 해당 아이템 타입에 대한 유효 검사 메서드 추가 필요.
    public static bool IsValidItemId(int itemId)
    {
        return itemId >= MaterialMin && itemId <= MaterialMax;
    }

    public static bool IsMaterialId(int itemId)
    {
        return itemId >= MaterialMin && itemId <= MaterialMax;
    }
}

