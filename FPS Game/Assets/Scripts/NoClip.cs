using UnityEngine;

public sealed class NoClip : MonoBehaviour
{
    [SerializeField] float distance;
    [SerializeField] float radius;

    [SerializeField] AnimationCurve offsetCurve;

    [SerializeField] LayerMask clippingLayerMask;

    Vector3 originalLocalPosition;

    private void Start()
    {
        originalLocalPosition = transform.localPosition;
    }

    private void Update()
    {
        if (Physics.SphereCast(Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f)), radius, out var hit, distance, clippingLayerMask))
        {
            transform.localPosition = originalLocalPosition - new Vector3(0.0f, 0.0f, offsetCurve.Evaluate(hit.distance / distance));
        }
        else
        {
            transform.localPosition = originalLocalPosition;
        }
    }
}
