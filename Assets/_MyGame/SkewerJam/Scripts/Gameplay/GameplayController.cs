using Sonat.Enums;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    public static GameplayController instance;
    public GameState gameState;

    public LevelGenerator levelGenerator;

    private void Awake()
    {
        // instance = this;
    }

    public void Start()
    {
    }
}