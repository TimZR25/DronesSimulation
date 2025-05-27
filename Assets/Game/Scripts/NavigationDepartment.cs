using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class NavigationDepartment : MonoBehaviour
{
    [SerializeField] private Drone _dronePrefab;

    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Color _color;

    [SerializeField] private Text _scoreText;

    [SerializeField] private ParticleSystem _particleSystem;

    private ResourceSpawner _resourceSpawner;

    private List<Drone> _drones = new List<Drone>();
    private Stack<Drone> _waitingDrones = new Stack<Drone>();
    private Stack<Drone> _removedDrones = new Stack<Drone>();

    private int _dronesCount;

    private IReadOnlyList<Resource> _resources => _resourceSpawner.FreeResources;

    private int _score = 0;

    public void Inject(ResourceSpawner resourceSpawner)
    {
        _resourceSpawner = resourceSpawner;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"A:{_drones.Count} : W:{_waitingDrones.Count} : R:{_removedDrones.Count}");
        }
    }

    private IEnumerator DistributeDrones()
    {
        if (_waitingDrones.Count > 0)
        {
            if (_drones.Count + _waitingDrones.Count != _dronesCount)
            {
                StartCoroutine(DistributeDrones(_dronesCount));
            }
        }

        if (_waitingDrones.Count > 0 && _resources.Count > 0)
        {
            Drone drone = _waitingDrones.Pop();

            drone.gameObject.SetActive(true);

            drone.State = DroneState.Wait;

            drone.ResourceCollected += OnResourceCollected;
            drone.ResourceUnloaded += OnResourceUnloaded;

            yield return StartCoroutine(SetClosestResource(drone));

            if (drone.State == DroneState.Wait)
            {
                WaitDrone(drone);
                DisableDrone(drone);
            }
            else
            {
                _drones.Add(drone);
            }
        }

        yield return new WaitForSeconds(1);

        StartCoroutine(DistributeDrones());
    }

    private IEnumerator SetClosestResource(Drone drone)
    {
        NavMeshAgent agent = drone.Agent;

        float tmpDist = float.MaxValue;

        Resource currentTarget = null;

        List<Resource> resources = new List<Resource>(_resources);

        foreach (var resource in resources)
        {
            if (resource.IsReserved == true) continue;

            if (agent.SetDestination(resource.PointToStop.position))
            {
                while (agent.pathPending)
                    yield return null;

                if (agent.pathStatus != NavMeshPathStatus.PathInvalid)
                {
                    float pathDistance = 0;

                    pathDistance += Vector3.Distance(drone.transform.position, agent.path.corners[0]);
                    for (int i = 1; i < agent.path.corners.Length; i++)
                    {
                        pathDistance += Vector3.Distance(agent.path.corners[i - 1], agent.path.corners[i]);
                    }

                    if (tmpDist > pathDistance && resource.IsReserved == false)
                    {
                        tmpDist = pathDistance;
                        currentTarget = resource;
                        agent.ResetPath();
                    }
                }
            }
        }

        if (currentTarget != null && currentTarget.gameObject.activeSelf)
        {
            _resourceSpawner.ReserveResource(currentTarget);

            drone.SetTarget(currentTarget.transform, DroneState.Seek);
        }
    }

    private void OnResourceCollected(Drone drone, Resource resource)
    {
        if (resource is null)
        {
            WaitDrone(drone);
        }
        else
        {
            drone.SetTarget(transform, DroneState.Return);
        }
    }

    private void WaitDrone(Drone drone)
    {
        drone.SetTarget(null, DroneState.Wait);

        _drones.Remove(drone);

        _waitingDrones.Push(drone);
    }

    private void DisableDrone(Drone drone)
    {
        drone.ResourceCollected -= OnResourceCollected;
        drone.ResourceUnloaded -= OnResourceUnloaded;

        drone.gameObject.SetActive(false);
    }

    private void OnResourceUnloaded(Drone drone)
    {
        _particleSystem.Play();

        _score++;
        _scoreText.text = _score.ToString();

        WaitDrone(drone);
        DisableDrone(drone);
    }

    private void SpawnDrones(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Drone drone = Instantiate(_dronePrefab, transform);

            drone.SetTeamColor(_color);

            _removedDrones.Push(drone);

            drone.gameObject.SetActive(false);
        }

        StartCoroutine(DistributeDrones());
    }

    private void OnDroneCountChanged(float value)
    {
        _dronesCount = (int)value;

        if (_drones.Count + _waitingDrones.Count + _removedDrones.Count <= 0)
        {
            SpawnDrones((int)value);
        }

        StartCoroutine(DistributeDrones(value));
    }

    private IEnumerator DistributeDrones(float count)
    {
        int dronesCount = _drones.Count + _waitingDrones.Count;

        if (dronesCount < count)
        {
            if (_removedDrones.Count > 0)
            {
                Drone drone = _removedDrones.Pop();

                WaitDrone(drone);
                DisableDrone(drone);
            }
        }
        else if (dronesCount > count)
        {
            if (_waitingDrones.Count > 0)
            {
                _removedDrones.Push(_waitingDrones.Pop());
            }
        }

        if (dronesCount != count)
        {
            yield return new WaitForSeconds(1);
            yield return StartCoroutine(DistributeDrones(count));
        }
    }

    private void OnEnable()
    {
        GameSettings.DroneCountChanged += OnDroneCountChanged;
    }

    private void OnDisable()
    {
        GameSettings.DroneCountChanged -= OnDroneCountChanged;
    }

    private void OnValidate()
    {
        _spriteRenderer.color = _color;
    }
}
