using UnityEngine;
using System.Collections;

public class GameObjectPool : MonoBehaviour, iPool<GameObject>
{
    protected static GameObjectDummyPool dummypool = null;

    [SerializeField]
    string pooledPrefabName;

    ObjectPool<GameObject> goPool;

    GameObject CreateBlock()
    {
        GameObject obj = Instantiate(Resources.Load(pooledPrefabName) as GameObject);
        obj.transform.SetParent(transform);

        return obj;
    }

    protected virtual void Awake()
    {
        goPool = new ObjectPool<GameObject>(100, 20, CreateBlock);
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

