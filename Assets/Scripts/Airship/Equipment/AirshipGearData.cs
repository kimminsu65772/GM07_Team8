using UnityEngine;

public enum AirshipGearType
{
    MaxHealth,
    MoveSpeed,
}


[CreateAssetMenu(
    fileName = "AirshipGear_",
    menuName = "Airship/Equipment/Gear")]
public class AirshipGearData : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string displayName;
    [SerializeField] private Sprite uiImage;
    [SerializeField] private Sprite gearImage;
    [SerializeField] private AirshipGearType gearType;
    [SerializeField] private AirshipBuffData buffData;

    public string Id => id;
    public string DisplayName => displayName;
    public Sprite UIImage => uiImage;
    public Sprite GearImage => gearImage;
    public AirshipGearType GearType => gearType;

    public AirshipBuff CreateBuff(object owner)
    {
        return buffData == null
            ? null
            : buffData.CreateBuff(owner);
    }
}
