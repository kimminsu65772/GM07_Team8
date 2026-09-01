using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SoundTableSO", menuName = "Game/Sound/SoundTable")]
public class SoundTableSO : ScriptableObject
{
    [SerializeField] private List<SoundData> sounds;

    public IReadOnlyList<SoundData> Sounds => sounds;
}


