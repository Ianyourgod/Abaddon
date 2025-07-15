using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Optional ID for the key. If it's less than 0, it's not a special key.")]
    private int keyID = -1;

    public bool isSpecialKey()
    {
        return keyID >= 0;
    }

    public int GetKeyID()
    {
        return keyID;
    }
}
