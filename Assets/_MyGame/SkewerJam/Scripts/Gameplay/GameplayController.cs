using Sonat.Enums;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    public static GameplayController instance;
    public GameState gameState;

    private void Awake()
    {
        instance = this;
    }

    public void Start()
    {
    }
}