using DG.Tweening;
using Manager;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;

public class UIWheel : MonoBehaviour
{
    [SerializeField] private RectTransform pArrow;
    [SerializeField] private float duration = 2f;
    [SerializeField] private bool isRotate = true;
    [SerializeField] private int valueMax = 8;
    [SerializeField] private int valueMid = 5;
    [SerializeField] private int valueMin = 3;
    [SerializeField] private TMP_Text[] textValues;
    [SerializeField, ShowIf("isRotate")] private float maxAngle = 25f;
    [SerializeField, ShowIf("@!isRotate")] private RectTransform startPoint;
    [SerializeField, ShowIf("@!isRotate")] private RectTransform endPoint;
    [SerializeField, ShowIf("@!isRotate")] private AnimationCurve moveCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField, ShowIf("@!isRotate")] private float[] thresholds;


    private Tween _wheelTween;
    private Vector2 _centerPosition;
    // Start is called before the first frame update
    void Awake()
    {
        _centerPosition = (startPoint.anchoredPosition + endPoint.anchoredPosition) / 2;
    }

    private void OnDisable()
    {
        StopWheel();
    }

    private void OnEnable()
    {
        LoadConfig();
        StartWheel();
    }

    private void LoadConfig()
    {
        valueMax = GameRemoteConfigValue.GetInt("IN_GAME_win_panel_reward_multiplier_max", valueMax);
        valueMid = GameRemoteConfigValue.GetInt("IN_GAME_win_panel_reward_multiplier_mid", valueMid);
        valueMin = GameRemoteConfigValue.GetInt("IN_GAME_win_panel_reward_multiplier_min", valueMin);
    }

    private void StartWheel()
    {
        SetValues();


        _wheelTween?.Kill();
        if (isRotate)
        {
            pArrow.localRotation = Quaternion.Euler(0, 0, -maxAngle);
            _wheelTween = pArrow.DORotate(new Vector3(0, 0, maxAngle), duration / 2)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
        else
        {
            pArrow.anchoredPosition = startPoint.anchoredPosition;
            _wheelTween = pArrow.DOAnchorPos(endPoint.anchoredPosition, duration / 2)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(moveCurve);
        }
    }

    private void SetValues()
    {

        textValues[2].text = "x" + valueMax.ToString();

        textValues[1].text = "x" + valueMid.ToString();
        textValues[0].text = "x" + valueMin.ToString();

        textValues[3].text = "x" + valueMid.ToString();
        textValues[4].text = "x" + valueMin.ToString();
    }

    public void StopWheel()
    {
        _wheelTween?.Kill();
    }

    public int GetWheelValue()
    {
        if (isRotate)
        {
            return GetWheelValue_Rotate();
        }
        else
        {
            return GetWheelValue_Move();
        }
    }

    private int GetWheelValue_Move()
    {
        // Debug.Log($"pArrow.anchoredPosition: {pArrow.anchoredPosition}");
        // Debug.Log($"_originalPosition: {_centerPosition}");
        var distance = Vector3.Distance(pArrow.anchoredPosition, _centerPosition);
        if (distance < thresholds[0])
        {
            return valueMax;
        }
        else if (distance < thresholds[1])
        {
            return valueMid;
        }
        else
        {
            return valueMin;
        }
    }

    public int GetWheelValue_Rotate()
    {
        var rotation = pArrow.localRotation.eulerAngles.z;
        rotation = (rotation > 180) ? rotation - 360 : rotation; // Normalize to [-180, 180]
        if (rotation < -18.5f || rotation > 18.5f)
        {
            return valueMin;
        }
        else if (rotation < -6.5f || rotation > 6.5f)
        {
            return valueMid;
        }
        else
        {
            return valueMax;
        }
    }
}
