using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class Resource : MonoBehaviour
{
    private bool _isReserved;
    public bool IsReserved => _isReserved;

    public UnityAction<Resource> Collected;

    [SerializeField] private Transform _pointToStop;

    public Transform PointToStop => _pointToStop;

    public void Reserve()
    {
        _isReserved = true;
    }

    public void Unreserve()
    {
        _isReserved = false;
    }

    public void Collect()
    {
        Unreserve();
        Collected?.Invoke(this);
    }
}
