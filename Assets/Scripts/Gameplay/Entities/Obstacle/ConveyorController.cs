using System.Collections;
using System.Collections.Generic;
using Gameplay.Entities;
using Gameplay.LevelData;
using MyGame.SkewerJam.Gameplay;
using MyGame.SkewerJam.Scripts.SO.Behavior;
using Sonat.Enums;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems.EventBus;
using SonatFramework.Systems.ObjectPooling;
using UnityEngine;

public class ConveyorController : EntityBase, IPoolingObject
{
    public override EntityType entityType => EntityType.Conveyor;
    [SerializeField] protected MoveType moveType;
    protected ConveyorData conveyData;
    [SerializeField] protected Transform container;
    protected Vector3 startPosition;
    protected Vector3 endPosition;
    protected List<PrimaryGrill> grills;
    protected float spacing = 4f;
    protected Vector3 direction;
    protected Vector3 conveySpeed;
    protected Vector3 visualSpeed;
    protected Material materialVisual;
    [SerializeField] protected float visualRatio = -1.2f;

    [SerializeField] protected SpriteRenderer visual;
    //[SerializeField] private SpriteRenderer visual;
    //[SerializeField] private Sprite[] sprites;

    //private static int visualCount;
    protected bool paused;
    protected bool moving = true;
    protected EventBinding<GameStateChangeEvent> onGameStateChange;
    protected bool hasSubGrill = false;
    [SerializeField] protected Material visualMaterial;

    [Space(10)]
    [Header("Behavior SO")]
    [SerializeField] protected ConveyorControllerSO conveyorControllerSO;

    private void OnEnable()
    {
        onGameStateChange = new EventBinding<GameStateChangeEvent>(OnGameStateChangedEvent);
    }

    private void OnDisable()
    {
        EventBus<GameStateChangeEvent>.Deregister(onGameStateChange);
    }

    private void OnGameStateChangedEvent(GameStateChangeEvent eventData)
    {
        if (eventData.gameState == GameState.Paused || eventData.gameState == GameState.GameOver)
        {
            moving = false;
        }
        else
        {
            moving = true;
        }
    }

    public void SetData(ConveyorData conveyData)
    {
        if (conveyData.speed == 0) conveyData.speed = -1;
        this.conveyData = conveyData;
        transform.position = conveyData.position.ToVector3();
        materialVisual = Instantiate(visualMaterial);
        CalculateStartPosition();
        GetGrills();
        CalculateSpacing();
        paused = false;
        moving = true;
        StartMovement();
        //visual.sprite = sprites[visualCount];
        //visualCount++;
        //if(visualCount >= sprites.Length) visualCount = 0;
    }

    protected virtual void CalculateStartPosition()
    {
        startPosition = container.position;
        endPosition = container.position;
        conveyorControllerSO.GetStartEndPositions(conveyData, moveType, conveyData.speed, ref startPosition, ref endPosition);

        direction = moveType == MoveType.Horizontal
            ? (conveyData.speed > 0 ? Vector3.right : Vector3.left)
            : (conveyData.speed > 0 ? Vector3.up : Vector3.down);
    }

    public virtual void GetGrills()
    {
        grills = new List<PrimaryGrill>();
        hasSubGrill = false;
        foreach (var grillId in conveyData.grillIds)
        {
            var grill = conveyorControllerSO.GetGrill(grillId);
            if (grill != null)
            {
                grills.Add(grill);
                grill.SetMaskVisible(moveType == MoveType.Vertical);
                if (!hasSubGrill && grill.HasSubGrills())
                {
                    hasSubGrill = true;
                }
            }
        }

        grills.Sort((a, b) => Vector3.SqrMagnitude(a.transform.position - endPosition).CompareTo(Vector3.SqrMagnitude(b.transform.position - endPosition)));
    }

    protected virtual void CalculateSpacing()
    {
        if (grills.Count == 0) return;

        spacing = 0.75f;
        float totalDistance = 0;
        for (int i = 0; i < grills.Count; i++)
        {
            totalDistance += grills[0].GrillVisual.GetGrillBounds().size.x;
        }

        if (totalDistance + spacing * (grills.Count - 1) < Vector3.Distance(startPosition, endPosition))
        {
            spacing = (Vector3.Distance(startPosition, endPosition) - totalDistance) / (grills.Count - 1) + 0.2f;
        }

        //spacing = Mathf.Clamp(spacing, 4, 8f);
        grills[0].transform.position = startPosition;
        if (!hasSubGrill)
        {
            grills[0].transform.SetLocalPositionY(startPosition.y - 0.35f);
        }

        for (int i = 1; i < grills.Count; i++)
        {
            float distance = grills[i].GrillVisual.GetGrillBounds().size.x / 2 + grills[i - 1].GrillVisual.GetGrillBounds().size.x / 2 + spacing;
            grills[i].transform.position = grills[i - 1].transform.position - direction * distance;
        }
    }

    private void StartMovement()
    {
        conveySpeed = direction * Mathf.Clamp(Mathf.Abs(conveyData.speed), -1, 1);
        visualSpeed = visualRatio * conveySpeed;
        materialVisual.SetVector("_UVScrollSpeed", visualSpeed);
        visual.material = materialVisual;

        StartCoroutine(IEMovement());
    }

    IEnumerator IEMovement()
    {
        //Vector3 movementDirection = direction; // * Mathf.Abs(conveyData.speed);
        PrimaryGrill firstGrill = grills[0];
        while (true)
        {
            if (moving)
            {
                if (paused)
                {
                    paused = false;
                    materialVisual.SetVector("_UVScrollSpeed", visualSpeed);
                }

                foreach (var grill in grills)
                {
                    grill.transform.Translate(conveySpeed * Time.deltaTime);
                }

                if ((firstGrill.transform.position - endPosition).sqrMagnitude < 0.2f)
                {
                    grills.Remove(firstGrill);
                    Vector3 rePos = grills[^1].transform.position - direction * GetReDistance(firstGrill);
                    firstGrill.transform.position = GetRePosition(rePos);
                    firstGrill.OnResetConveyorCircle();
                    grills.Add(firstGrill);
                    firstGrill = grills[0];
                }
            }
            else if (!paused)
            {
                paused = true;
                materialVisual.SetVector("_UVScrollSpeed", Vector3.zero);
            }

            yield return null;
        }
    }

    protected virtual float GetReDistance(PrimaryGrill grill)
    {
        return grill.GrillVisual.GetGrillBounds().size.x / 2 + grills[^1].GrillVisual.GetGrillBounds().size.x / 2 + spacing;
    }

    private Vector3 GetRePosition(Vector3 position)
    {
        if (moveType == MoveType.Vertical) return position;
        if (conveyData.speed > 0)
        {
            if (position.x > startPosition.x) return startPosition;
        }
        else
        {
            if (position.x < startPosition.x) return startPosition;
        }

        return position;
    }

    public void Setup()
    {
    }

    public virtual void OnCreateObj(params object[] args)
    {
        paused = false;
    }

    public virtual void OnReturnObj()
    {
        StopAllCoroutines();
        paused = false;
    }
}