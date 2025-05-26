using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private ResourceSpawner _resourceSpawner;

    private IEnumerator Start()
    {
        List<NavigationDepartment> navigationDepartments = FindObjectsByType<NavigationDepartment>(FindObjectsSortMode.None).ToList();

        List<Obstacle> obstacles = FindObjectsByType<Obstacle>(FindObjectsSortMode.None).ToList();

        List<SpriteRenderer> obstaclesSR = new List<SpriteRenderer>();

        foreach (var item in obstacles)
        {
            obstaclesSR.Add(item.SR);
        }

        _resourceSpawner.Inject(obstaclesSR);

        yield return StartCoroutine(_resourceSpawner.SpawnResources());

        foreach (var item in navigationDepartments)
        {
            item.Inject(_resourceSpawner);
            item.SpawnDrones();
        }
    }
}
