using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    const string terrainTag = "Ground"; //터렌인 청크를 확인하는데 사용한다 
    public float maxDistance = 1000f; //얼만큼 깊이로 RayCast 를 할건지 정한다 

    [System.NonSerialized]
    public static bool genreatingTerrainObjectCompleted;

    private int totalNumber;

    public DynamicTerrainObjectType terrainObjectType;
    public bool staticSpwan;

    //밀도 관련 변수들
    float totalDensity;

    // 구 관련 정보
    public Vector3 planetCenter;

    LinkedList<GameObject> dynamicObjectList = new LinkedList<GameObject>();

    public void Start()
    {
        StartCoroutine(tempCorutine());
        if(terrainObjectType.spwanIntervalTime == 0)
        {
            staticSpwan = true;
        }
    }

    IEnumerator tempCorutine()
    {
        yield return new WaitForSecondsRealtime(3);
        GenerateInitialDynamicObject();
    }

    IEnumerator SpwanObjectCorutine()
    {
        while (true)
        {
            GenerateInitialDynamicObject();
            totalNumber = dynamicObjectList.Count;
            yield return new WaitForSeconds(terrainObjectType.spwanIntervalTime);
        }
    }

    void GenerateInitialDynamicObject()
    {
        Queue<MeshFilter> meshFilters = new Queue<MeshFilter>();

        for (int i=0;i<terrainObjectType.spwanNumber;i++)
        {
            GameObject gameObject = SpwanTerrainObject(Random.onUnitSphere, terrainObjectType);
            if(gameObject != null && terrainObjectType.mergeMesh && staticSpwan)
            {
                meshFilters.Enqueue(gameObject.GetComponent<MeshFilter>());
            }
        }

        if (terrainObjectType.mergeMesh && staticSpwan)
        {
            /*
            int subMeshNumber = terrainObjectType.prefab.GetComponent<MeshFilter>().sharedMesh.subMeshCount;
            GameObject[] subMeshes = new GameObject[subMeshNumber];
            CombineInstance[,] combines = new CombineInstance[subMeshNumber,meshFilters.Count];

            int meshCount = 0;
            while(meshFilters.Count > 0)
            {
                MeshFilter tempMesh = meshFilters.Dequeue();
                for(int index = 0; index < subMeshNumber; index ++)
                {
                    combines[index, meshCount].mesh = tempMesh.sharedMesh.GetSubMesh(index);
                }
            }
            */

            int totalMeshFiliterCount = meshFilters.Count;
            int subMeshNumber = terrainObjectType.prefab.GetComponent<MeshFilter>().sharedMesh.subMeshCount;
            int verticesCount = 0;
            int verticesLimit = 30000;
            GameObject[] meshes = new GameObject[subMeshNumber];
            CombineInstance[] combines = new CombineInstance[meshFilters.Count];

            int currentCombineIndex = 0;
            int prevCombineIndex = 0;
            while (meshFilters.Count > 0)
            {
                MeshFilter tempMesh = meshFilters.Dequeue();
                combines[currentCombineIndex].mesh = tempMesh.sharedMesh;
                combines[currentCombineIndex].transform = tempMesh.transform.localToWorldMatrix;
                Destroy(tempMesh.gameObject);

                currentCombineIndex++;
                verticesCount += tempMesh.mesh.vertexCount;

                if (verticesCount > verticesLimit || meshFilters.Count == 0)
                {
                    verticesCount = 0;
                    CombineInstance[] tempCombine = new CombineInstance[currentCombineIndex - prevCombineIndex + 1];

                    for (int i = prevCombineIndex; i < currentCombineIndex; i++)
                    {
                        tempCombine[i - prevCombineIndex].mesh = combines[i].mesh;
                        tempCombine[i - prevCombineIndex].transform = combines[i].transform;
                    }
                    for (int index = 0; index < subMeshNumber; index++)
                    {
                        for (int i = prevCombineIndex; i <= currentCombineIndex; i++)
                        {                   
                            tempCombine[i - prevCombineIndex].subMeshIndex = index;
                        }

                        meshes[index] = new GameObject();
                        meshes[index].transform.parent = gameObject.transform;
                        meshes[index].name = "Mesh Object Convex " + index;
                        meshes[index].AddComponent<MeshFilter>();
                        meshes[index].AddComponent<MeshRenderer>();

                        meshes[index].GetComponent<MeshFilter>().mesh = new Mesh();
                        meshes[index].GetComponent<MeshFilter>().mesh.CombineMeshes(tempCombine);
                        meshes[index].GetComponent<MeshRenderer>().sharedMaterial = terrainObjectType.prefab.GetComponent<MeshRenderer>().sharedMaterials[index];

                        if (terrainObjectType.useCollider)
                        {
                            var object_meshCollider = meshes[index].AddComponent<MeshCollider>();
                        }
                    }

                    prevCombineIndex = currentCombineIndex;
                }
            }



            meshFilters.Clear();
        }

        genreatingTerrainObjectCompleted = true;
        if (staticSpwan == false)
            StartCoroutine(SpwanObjectCorutine());
    }

    public bool RemoveObject(GameObject gameObject)
    {
        try
        {
            dynamicObjectList.Remove(gameObject);
        }
        catch (System.InvalidOperationException)
        {
            return false;
        }

        return true;
    }

    public GameObject SpwanTerrainObject(Vector3 onUnitSphere, DynamicTerrainObjectType terrainObjectType)
    {
        //스폰되면 게임 오브젝트를 아니면 null 을 반환한다 
        RaycastHit raycastHit;
        Vector3 origin = onUnitSphere * maxDistance;
        Vector3 direction = planetCenter - origin;

        Vector3 spwanPosition;
        Vector3 spwanNormal;
        Quaternion spwanRotation;

        bool isHit = Physics.Raycast(origin, direction, out raycastHit, maxDistance);

        if (isHit == true)
        {
            if (raycastHit.collider.tag == "Water")
            {
                if (terrainObjectType.spwanPosition == DynamicTerrainObjectType.SpwanPositionInfo.All)
                {
                    Physics.Raycast(origin, direction, out raycastHit, maxDistance, 1 << LayerMask.NameToLayer("Ground"));
                }
                else if (terrainObjectType.spwanPosition == DynamicTerrainObjectType.SpwanPositionInfo.InsideWater)
                {
                    Physics.Raycast(origin, direction, out raycastHit, maxDistance, 1 << LayerMask.NameToLayer("Ground"));
                }
                else if (terrainObjectType.spwanPosition == DynamicTerrainObjectType.SpwanPositionInfo.SurfaceWater)
                {
                    //do nothing
                }
                else if (terrainObjectType.spwanPosition == DynamicTerrainObjectType.SpwanPositionInfo.OutsideWater)
                {
                    return null;
                }
            } else if(raycastHit.collider.tag == "Ground")
            {
                if (terrainObjectType.spwanPosition == DynamicTerrainObjectType.SpwanPositionInfo.All)
                {
                    //do nothing
                }
                else if (terrainObjectType.spwanPosition == DynamicTerrainObjectType.SpwanPositionInfo.InsideWater)
                {
                    return null;
                }
                else if (terrainObjectType.spwanPosition == DynamicTerrainObjectType.SpwanPositionInfo.SurfaceWater)
                {
                    return null;
                }
                else if (terrainObjectType.spwanPosition == DynamicTerrainObjectType.SpwanPositionInfo.OutsideWater)
                {
                    //do nothing
                }
            } else
            {
                return null;
            }

            spwanPosition = raycastHit.point;
            spwanNormal = raycastHit.normal;

            if (!terrainObjectType.isErect)
                spwanRotation = Quaternion.FromToRotation(Vector3.up, 360 * spwanNormal) * terrainObjectType.prefab.transform.rotation;
            else
                spwanRotation = Quaternion.FromToRotation(Vector3.down, 360 * direction.normalized) * terrainObjectType.prefab.transform.rotation;

            if(terrainObjectType.randomRotation == true)
            {
                spwanRotation = spwanRotation * Quaternion.Euler(0, Random.Range(0, 360), 0);
            }
        }
        else
        {
            return null;
        }

        MeshCollider object_meshCollider;
        Rigidbody object_rigidbody;

        GameObject spwanObject = Instantiate(terrainObjectType.prefab);
        spwanObject.tag = this.tag;
        spwanObject.transform.position = new Vector3(spwanPosition.x, spwanPosition.y, spwanPosition.z);
        spwanObject.transform.position = Vector3.MoveTowards(spwanObject.transform.position, planetCenter, terrainObjectType.insertDepth);

        spwanObject.transform.rotation = spwanRotation;
        spwanObject.transform.parent = this.transform;

        if (terrainObjectType.sizeMultiplier > 0.001)
            spwanObject.transform.localScale *= terrainObjectType.sizeMultiplier;
        else
            spwanObject.transform.localScale *= 1;

        if (terrainObjectType.useRigidbody)
        {
            object_rigidbody = spwanObject.AddComponent<Rigidbody>();
            object_meshCollider = spwanObject.AddComponent<MeshCollider>();
            if(terrainObjectType.useConvex)
            {
                object_meshCollider.convex = true;
            }
            if (terrainObjectType.isKnematic)
            {
                object_rigidbody.isKinematic = true;
            }
        }
        else if (terrainObjectType.useCollider)
        {
            object_meshCollider = spwanObject.AddComponent<MeshCollider>();
        }

        return spwanObject;
    }
}

[System.Serializable]
public class DynamicTerrainObjectType
{
    public bool useRigidbody;
    public bool useCollider;
    public bool isKnematic;
    public bool isErect;
    public bool useConvex;
    public bool mergeMesh;

    public GameObject prefab;

    public int spwanNumber;
    public float sizeMultiplier; //0 means no change to object size
    public float insertDepth;
    public float spwanIntervalTime;
    public int spawnIntervalNumber;
    public bool randomRotation;

    public enum SpwanPositionInfo { InsideWater, SurfaceWater, OutsideWater, All };
    public SpwanPositionInfo spwanPosition;
}
