using UnityEngine;
using System.Collections;

public class GameObjectPool : MonoBehaviour, iPool<GameObject>
{
    protected static GameObjectDummyPool dummypool = null;

    [SerializeField]
    string pooledPrefabName;

    ObjectPool<GameObject> goPool;

    GameObject CreateObj()
    {
        GameObject obj = Instantiate(Resources.Load(pooledPrefabName) as GameObject);
        obj.transform.SetParent(transform);
        obj.SetActive(false);
        return obj;
    }

    protected virtual void Awake()
    {
        goPool = new ObjectPool<GameObject>(100, 20, CreateObj);
    }

    public GameObject Pop()
    {
        return goPool.Pop();
    }

    public void Push(GameObject target)
    {
        goPool.Push(target);
    }
}

public class GameObjectDummyPool : iPool<GameObject>
{
    public GameObject Pop()
    {
        return new GameObject();
    }

    public void Push(GameObject target)
    {
    }
}

