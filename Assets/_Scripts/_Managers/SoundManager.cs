using UnityEngine;

public class SoundManager : MonoBehaviour
{
    #region Instance Method
    public static SoundManager Instance;
    private void InstanceMethod()
    {
        if (Instance)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion
    
    private void Awake()
    {
        #region Instance Method
        InstanceMethod();
        #endregion
    }

    private void Start()
    {
        
    }
}
