using UnityEngine;

[CreateAssetMenu(fileName = "Weapon+", menuName = "Weapons/Weapon")]
public class Weapon : ScriptableObject
{
    public string weaponName;

    public float length = 1f;
    public float width = 0.25f;
    public float weight = 1f;
}
