using MyGame.SkewerJam.Gameplay;
using Sonat.Enums;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    public static GameplayController instance;
    public GameState GameState => GameController.Instance.GameState;

}