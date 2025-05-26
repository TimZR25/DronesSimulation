using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent), typeof(LineRenderer))]
public class Drone : MonoBehaviour
{
    [SerializeField] private float _collectRadius;

    [SerializeField] private SpriteRenderer _spriteRenderer;

    [SerializeField] private SpriteRenderer _teamSR;

    private NavMeshAgent _agent;
    public NavMeshAgent Agent => _agent;

    private DroneState _state;
    public DroneState State
    {
        get { return _state; }
        set
        {
            if (_state == value) return;

            _state = value;

            UpdateSkin();
        }
    }

    public UnityAction<Drone, Resource> ResourceCollected;
    public UnityAction<Drone> ResourceUnloaded;

    public UnityAction<Drone> Disabled;
    public UnityAction<Drone> Enabled;

    private Resource _resource;
    private Transform _target;

    private LineRenderer _lineRenderer;

    private bool _pathView;

    private void Awake()
    {
        if (_agent is null)
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        if (_lineRenderer is null)
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        _lineRenderer.startWidth = 0.05f;
        _lineRenderer.endWidth = 0.05f;
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default")) { color = Color.yellow };

        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }

    private void Update()
    {
        if (_agent.hasPath && _pathView)
        {
            _lineRenderer.positionCount = _agent.path.corners.Length;
            _lineRenderer.SetPositions(_agent.path.corners);
        }
    }

    public void SetTeamColor(Color color)
    {
        _teamSR.color = color;
    }

    public void MoveTo()
    {
        _agent.SetDestination(_target.position);
    }

    public void SetTarget(Transform target, DroneState droneState)
    {
        _target = target;

        State = droneState;
    }

    public IEnumerator Collect()
    {
        State = DroneState.Collect;

        if (_target.TryGetComponent(out Resource resource))
        {
            _resource = resource;
        }

        if (_resource is not null)
        {
            yield return new WaitForSeconds(2);

            _resource.Collect();
        }

        ResourceCollected?.Invoke(this, _resource);
    }

    public void Unload()
    {
        _resource = null;

        ResourceUnloaded?.Invoke(this);
    }

    private void OnEnable()
    {
        GameSettings.DroneSpeedChanged += (float value) => _agent.speed = value;
        GameSettings.PathViewChanged += (bool value) => _pathView = value;

        StopAllCoroutines();

        StartCoroutine(CheckState());
    }

    private void OnDisable()
    {
        GameSettings.DroneSpeedChanged -= (float value) => _agent.speed = value;
        GameSettings.PathViewChanged -= (bool value) => _pathView = value;

        StopAllCoroutines();
    }

    private void UpdateSkin()
    {
        switch (State)
        {
            case DroneState.Wait:
                _spriteRenderer.color = Color.white;
                break;
            case DroneState.Seek:
                _spriteRenderer.color = Color.yellow;
                break;
            case DroneState.Collect:
                _spriteRenderer.color = Color.green;
                break;
            case DroneState.Return:
                _spriteRenderer.color = Color.cyan;
                break;
            default:
                break;
        }
    }

    private IEnumerator CheckState()
    {
        if (_target is not null)
        {
            if (Vector3.Distance(transform.position, _target.position) <= _collectRadius)
            {
                if (State == DroneState.Seek)
                {
                    yield return StartCoroutine(Collect());
                }
                else if (State == DroneState.Return)
                {
                    Unload();
                }
            }
            else
            {
                MoveTo();
            }
        }

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(CheckState());
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, _collectRadius);
    }
}
