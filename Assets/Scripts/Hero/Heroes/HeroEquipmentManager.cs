using UnityEngine;

public class HeroEquipmentManager : MonoBehaviour
{
    [SerializeField] private Equipment currentWeaponEquip;
    [SerializeField] private Equipment currentBodyEquip;
    [SerializeField] private Equipment currentAccEquip;
    [SerializeField] private Equipment selectedEquip;

    private Hero hero;

    public Equipment CurrentWeaponEquip => currentWeaponEquip;
    public Equipment CurrentBodyEquip => currentBodyEquip;
    public Equipment CurrentAccEquip => currentAccEquip;

    private void Awake()
    {
        hero = GetComponent<Hero>();
    }

    public void SelecteEquip(Equipment equip)
    {
        if (selectedEquip == equip) return;

        selectedEquip = equip;
    }

    public void GetEquip()
    {
        if (selectedEquip == currentWeaponEquip || selectedEquip == currentBodyEquip || selectedEquip == currentAccEquip) return;

        switch (selectedEquip.EquipPart)
        {
            case EquipPartEnum.Weapon:
                GetWeapon(selectedEquip);
                break;
            case EquipPartEnum.Body:
                GetBody(selectedEquip);
                break;
            case EquipPartEnum.Acc:
                GetAcc(selectedEquip);
                break;
            default:
                break;
        }
    }

    public void GetWeapon(Equipment weapon)
    {
        if (weapon.EquipPart != EquipPartEnum.Weapon) return;

        currentWeaponEquip = weapon;
        hero.EquipStatApply(weapon);
    }

    public void GetBody(Equipment body)
    {
        if (body.EquipPart != EquipPartEnum.Body) return;

        currentBodyEquip = body;
        hero.EquipStatApply(body);
    }

    public void GetAcc(Equipment acc)
    {
        if (acc.EquipPart != EquipPartEnum.Acc) return;

        currentAccEquip = acc;
        hero.EquipStatApply(acc);
    }

    public void ClearSelectEquip()
    {
        selectedEquip = null;
    }
}
