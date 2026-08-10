using UnityEngine;

public enum AirshipProjectileType
{
    Normal,
    Freeze,
}

[CreateAssetMenu(
    fileName = "AirshipCannon_",
    menuName = "Airship/Equipment/Cannon")]
public class AirshipCannonData : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string displayName;
    [SerializeField] private Sprite uiImage;
    [SerializeField] private Sprite barrelImage;
    [SerializeField] private AirshipProjectileType projectileType;
    [SerializeField] private AirshipProjectileBase projectilePrefab;

    public string Id => id;
    public string DisplayName => displayName;
    public Sprite UIImage => uiImage;
    public Sprite BarrelImage => barrelImage;
    public AirshipProjectileType ProjectileType => projectileType;
    public AirshipProjectileBase ProjectilePrefab => projectilePrefab;
}