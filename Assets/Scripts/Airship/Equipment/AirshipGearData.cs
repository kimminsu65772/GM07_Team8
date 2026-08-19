using UnityEngine;

public enum AirshipGearType
{
    Normal,
    MaxHealth,
    MoveSpeed,
    Shield,
}


[CreateAssetMenu(
    fileName = "AirshipGear_",
    menuName = "Airship/Equipment/Gear")]
public class AirshipGearData : ScriptableObject
{
    [SerializeField] private string displayName;
    [SerializeField] private Sprite uiImage;
    [SerializeField] private Sprite gearImage;
    [SerializeField] private AirshipGearType gearType;
    [SerializeField] private AirshipBuffData buffData;

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
