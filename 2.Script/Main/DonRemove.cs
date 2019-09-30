using UnityEngine;

public class DonRemove : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}