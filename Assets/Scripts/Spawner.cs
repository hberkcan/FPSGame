using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject enemy;
    [SerializeField] private GameObject ammoObj;
    [SerializeField] private GameObject healthObj;
    [SerializeField] private float timeBetweenSpawns = 5;
    private CountdownTimer enemyTimer;
    private CountdownTimer ammoTimer;
    private CountdownTimer healthTimer;
    [SerializeField] private int maxEnemyCount = 5;
    private int enemyCount;
    [SerializeField] private int maxAmmoObjCount = 3;
    private int ammoObjCount;
    [SerializeField] private int maxHealthObjtCount = 1;
    private int healthObjCount;

    private float[] sizes;
    private float[] cumulativeSizes;
    private float total;

    private Mesh mesh;
    NavMeshPath path;

    private void OnEnable()
    {
        EnemyController.OnAnyEnemyDie += EnemyController_OnAnyEnemyDie;
    }

    private void OnDisable()
    {
        EnemyController.OnAnyEnemyDie -= EnemyController_OnAnyEnemyDie;
    }

    private void EnemyController_OnAnyEnemyDie(EnemyConfig obj)
    {
        enemyCount--;
    }

    private void Start()
    {
        enemyTimer = new CountdownTimer(timeBetweenSpawns);
        enemyTimer.Start();
        enemyCount = maxEnemyCount; // there are already 5 enemies on scene
        ammoTimer = new CountdownTimer(timeBetweenSpawns);
        ammoTimer.Start();
        healthTimer = new CountdownTimer(timeBetweenSpawns);
        healthTimer.Start();
        
        path = new NavMeshPath();

        InitializeMesh();
    }

    private void Update()
    {
        if (!enemyTimer.IsRunning) 
        {
            if(enemyCount < maxEnemyCount) 
            {
                enemyTimer.Start();
                SpawnObject(enemy);
                enemyCount++;
            }
        }

        if(!ammoTimer.IsRunning) 
        {
            if(ammoObjCount < maxAmmoObjCount) 
            {
                ammoTimer.Start();
                SpawnObject(ammoObj);
                ammoObjCount++;
            }
        }

        if (!healthTimer.IsRunning) 
        {
            if(healthObjCount < maxHealthObjtCount)
            {
                healthTimer.Start();
                SpawnObject(healthObj);
                healthObjCount++;
            }
        }

        enemyTimer.Tick(Time.deltaTime);
        ammoTimer.Tick(Time.deltaTime);
        healthTimer.Tick(Time.deltaTime);
    }

    private void SpawnObject(GameObject go) 
    {
        Vector3 randomPos = GetRandomPointOnMesh(mesh);
        NavMesh.CalculatePath(Vector3.zero, randomPos, NavMesh.AllAreas, path);
        bool canReachPoint = path.status == NavMeshPathStatus.PathComplete;

        Vector3 spawnPos = Vector3.zero;

        if (canReachPoint)
            spawnPos = randomPos;

        GameObject obj = Instantiate(go, spawnPos, Quaternion.identity);

        if (obj.TryGetComponent(out ICollectable collectible))
        {
            collectible.OnDestroy = OnDestroyObj;
        }
    }

    private void OnDestroyObj(CollectibleType type) 
    {
        if(type == CollectibleType.Ammo) 
        {
            ammoObjCount--;
        }
        else if(type == CollectibleType.Health) 
        {
            healthObjCount--;
        }
    }

    private void InitializeMesh() 
    {
        NavMeshTriangulation triangles = NavMesh.CalculateTriangulation();
        mesh = new Mesh();
        mesh.vertices = triangles.vertices;
        mesh.triangles = triangles.indices;

        sizes = GetTriSizes(mesh.triangles, mesh.vertices);
        cumulativeSizes = new float[sizes.Length];
        total = 0;

        for (int i = 0; i < sizes.Length; i++)
        {
            total += sizes[i];
            cumulativeSizes[i] = total;
        }
    }

    private Vector3 GetRandomPointOnMesh(Mesh mesh)
    {
        float randomsample = Random.value * total;

        int triIndex = -1;

        for (int i = 0; i < sizes.Length; i++)
        {
            if (randomsample <= cumulativeSizes[i])
            {
                triIndex = i;
                break;
            }
        }

        if (triIndex == -1) Debug.LogError("triIndex should never be -1");

        Vector3 a = mesh.vertices[mesh.triangles[triIndex * 3]];
        Vector3 b = mesh.vertices[mesh.triangles[triIndex * 3 + 1]];
        Vector3 c = mesh.vertices[mesh.triangles[triIndex * 3 + 2]];

        //generate random barycentric coordinates

        float r = Random.value;
        float s = Random.value;

        if (r + s >= 1)
        {
            r = 1 - r;
            s = 1 - s;
        }
        //and then turn them back to a Vector3
        Vector3 pointOnMesh = a + r * (b - a) + s * (c - a);
        return pointOnMesh;

    }

    private float[] GetTriSizes(int[] tris, Vector3[] verts)
    {
        int triCount = tris.Length / 3;
        float[] sizes = new float[triCount];
        for (int i = 0; i < triCount; i++)
        {
            sizes[i] = .5f * Vector3.Cross(verts[tris[i * 3 + 1]] - verts[tris[i * 3]], verts[tris[i * 3 + 2]] - verts[tris[i * 3]]).magnitude;
        }
        return sizes;
    }
}
