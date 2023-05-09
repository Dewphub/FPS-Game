using System;
using System.Collections.Generic;
using UnityEngine;

public class DamageIndicatorSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] DamageIndicator indicatorPrefab = null;
    [SerializeField] RectTransform holder = null;
    [SerializeField] new Camera camera = null;
    [SerializeField] Transform player = null;

    Dictionary<Transform, DamageIndicator> Indicators = new();

    #region Delegates
    public static Action<Transform> CreateIndicator = delegate { };
    public static Func<Transform, bool> CheckIfObjectInSight = null;
    #endregion

    void OnEnable()
    {
        CreateIndicator += Create;
        CheckIfObjectInSight += InSight;
    }

    void OnDisable()
    {
        CreateIndicator -= Create;
        CheckIfObjectInSight -= InSight;
    }

    void Create(Transform target)
    {
        if(Indicators.ContainsKey(target))
        {
            Indicators[target].Restart();
            return;
        }
        DamageIndicator newIndicator = Instantiate(indicatorPrefab, holder);
        newIndicator.Register(target, player, new Action( () => { Indicators.Remove(target); }));

        Indicators.Add(target, newIndicator);
    }

    bool InSight(Transform target)
    {
        Vector3 screenPoint = camera.WorldToViewportPoint(target.position);
        return screenPoint.z > 0 && 
            screenPoint.x > 0 && 
            screenPoint.x < 1 && 
            screenPoint.y > 0 && 
            screenPoint.y < 1;
    }
}
