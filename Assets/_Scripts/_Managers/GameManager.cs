using UnityEngine;

public sealed class GameManager : MonoBehaviour
{
    #region Instance Method / GameState
    public static GameManager Instance;
    public LevelSpecial currentLevel;
    public PlayerSpecial playerSpecial;
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
    
    public enum GameState
    {
        Play,
        Pause,
        Win,
        Lose,
        StartMenu,
    }
    public GameState gameState;
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
    
    private void Update()
    {
        if (gameState == GameState.Play)
        {
           
        }
    }

    #region Win/Lose/CoinUpdate
    
    public void GameWin()
    {
        gameState = GameState.Win;
        //////////////////////////
        UIManager.Instance._GameWin();
    }

    public void GameLose()
    {
        gameState = GameState.Lose;
        ///////////////////////////
        UIManager.Instance._GameLose();
    }
    #endregion
    
    #region Constant Methods
    
    public static float ClampAngle(float angle, float min, float max)
    {
        angle = Mathf.Repeat(angle, 360);
        min = Mathf.Repeat(min, 360);
        max = Mathf.Repeat(max, 360);
        var inverse = false;
        var timing = min;
        var tangle = angle;
        if (min > 180)
        {
            inverse = true;
            timing -= 180;
        }
        if (angle > 180)
        {
            inverse = !inverse;
            tangle -= 180;
        }
        var result = !inverse ? tangle > timing : tangle < timing;
        if (!result)
            angle = min;
        inverse = false;
        tangle = angle;
        var tax = max;
        if (angle > 180)
        {
            inverse = true;
            tangle -= 180;
        }
        if (max > 180)
        {
            inverse = !inverse;
            tax -= 180;
        }
        result = !inverse ? tangle < tax : tangle > tax;
        if (!result)
            angle = max;
        return angle;
    }
    
    public Vector2 GetMousePosition()
    {
        var pos = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);

        return pos;
    }
    
    #endregion
}
