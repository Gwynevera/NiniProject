using UnityEngine;

[CreateAssetMenu(fileName = "Prop", menuName = "Props/Prop")]
public class Prop : ScriptableObject
{
    public string propName;

    public float weight = 1f;
}
