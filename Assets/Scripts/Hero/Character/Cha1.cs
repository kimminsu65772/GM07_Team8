using UnityEngine;

public class Cha1 : Hero
{
    private void Awake()
    {
        Init(100f, 20f, 10f, 2f, HeroLocationEnum.Front);
    }
}
