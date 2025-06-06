using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ResourceSpawner : MonoBehaviour
{
    [SerializeField] private Resource _resourcePrefab;

    [SerializeField] private int SourceCount;

    [SerializeField] private SpriteRenderer _floor;

    [SerializeField] private float _spawnDelay;

    private List<SpriteRenderer> _obstacles;

    private List<Resource> _reservedResources = new List<Resource>();

    private List<Resource> _freeResources = new List<Resource>();

    private Stack<Resource> _pooledResources = new Stack<Resource>();

    public List<Resource> FreeResources => _freeResources;

    private int _spawnRate;

    private Vector3 _max;
    private Vector3 _min;
    private Vector3 _offset;

    private void Awake()
    {
        SpriteRenderer spriteRenderer = _resourcePrefab.GetComponent<SpriteRenderer>();
        _offset = spriteRenderer.bounds.extents;

        _max = _floor.bounds.max;
        _min = _floor.bounds.min;
    }

    public void Inject(List<SpriteRenderer> obstacles)
    {
        _obstacles = obstacles;
    }

    public IEnumerator SpawnResources()
    {
        for (int i = 0; i < SourceCount; i++)
        {
            Resource resource = Instantiate(_resourcePrefab, transform);
            _pooledResources.Push(resource);

            resource.gameObject.SetActive(false);

            yield return StartCoroutine(ResetResource(0));
        }
    }

    public void ReserveResource(Resource resource)
    {
        if (_freeResources.Contains(resource) == false) return;

        resource.Reserve();

        _freeResources.Remove(resource);

        _reservedResources.Add(resource);
    }

    private IEnumerator ResetResource(float time)
    {
        yield return new WaitForSeconds(time);

        if (_pooledResources.Count > 0)
        {
            Resource newResource = _pooledResources.Pop();

            SpriteRenderer newResourceSR = newResource.GetComponent<SpriteRenderer>();

            newResource.gameObject.SetActive(true);

            newResource.Collected += OnResourceCollected;

            if (_obstacles.Contains(newResourceSR))
            {
                _obstacles.Remove(newResourceSR);
            }

            yield return StartCoroutine(SetPosition(newResource, newResourceSR));

            _obstacles.Add(newResourceSR);

            _freeResources.Add(newResource);
        }
    }

    private IEnumerator SetPosition(Resource resource, SpriteRenderer resourceSR)
    {
        yield return null;

        bool isEmpty;

        Vector3 point;

        do
        {
            isEmpty = true;

            float x = Random.Range(_min.x + _offset.x, _max.x - _offset.x);
            float y = Random.Range(_min.y + _offset.y, _max.y - _offset.y - resource.PointToStop.localPosition.y);

            point = new Vector3(x, y, 0);

            resource.transform.position = point;

            foreach (var obstacle in _obstacles)
            {
                if (resourceSR.bounds.Intersects(obstacle.bounds) || NavMesh.SamplePosition(point, out NavMeshHit hit, 0.1f, NavMesh.AllAreas) == false)
                {
                    isEmpty = false;
                    break;
                }
            }

        } while (isEmpty == false);
    }

    private void OnResourceCollected(Resource resource)
    {
        if (resource is null) return;

        _reservedResources.Remove(resource);
        _pooledResources.Push(resource);

        resource.Collected -= OnResourceCollected;

        resource.gameObject.SetActive(false);
        StartCoroutine(ResetResource(_spawnDelay * _spawnRate * _spawnRate));
    }

    private void OnEnable()
    {
        GameSettings.ResourceRateChanged += (int value) => _spawnRate = value;
    }

    private void OnDisable()
    {
        GameSettings.ResourceRateChanged -= (int value) => _spawnRate = value;
    }
}
