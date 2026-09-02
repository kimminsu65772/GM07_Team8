using UnityEngine;

public enum AirshipCannonType
{
    Normal,
    Freeze,
    Rapid,
    Heal,
}

[CreateAssetMenu(
    fileName = "AirshipCannon_",
    menuName = "Airship/Equipment/Cannon")]
public class AirshipCannonData : ScriptableObject
{
    [SerializeField] private string displayName;
    [SerializeField] private Sprite uiImage;
    [SerializeField] private Sprite barrelImage;
    [SerializeField] private AirshipCannonType cannonType;
    [SerializeField] private AirshipProjectileBase projectilePrefab;
    [SerializeField] private AirshipBuffData buffData;
    [SerializeField] private AudioClip fireSfxClip;
    [SerializeField, Range(0f, 1f)]
    private float fireSfxVolume = 1f;

    

    public string DisplayName => displayName;
    public Sprite UIImage => uiImage;
    public Sprite BarrelImage => barrelImage;
    public AirshipCannonType CannonType => cannonType;
    public AirshipProjectileBase ProjectilePrefab => projectilePrefab;
    public AudioClip FireSfxClip => fireSfxClip;
    public float FireSfxVolume => fireSfxVolume;

    public AirshipBuff CreateBuff(object owner)
    {
        return buffData == null
            ? null
            : buffData.CreateBuff(owner);
    }
}