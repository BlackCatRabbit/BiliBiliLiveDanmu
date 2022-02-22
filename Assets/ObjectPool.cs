using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool 
{
    /// <summary>
    /// �����
    /// </summary>
    private Dictionary<string, List<GameObject>> pool;
    /// <summary>
    /// Ԥ����
    /// </summary>
    private Dictionary<string, GameObject> prefabs;

    #region ����
    private static ObjectPool _instance;
    private ObjectPool()
    {
        pool = new Dictionary<string, List<GameObject>>();
        prefabs = new Dictionary<string, GameObject>();
    }
    public static ObjectPool GetInstance()
    {
        if (_instance == null)
        {
            _instance = new ObjectPool();
        }
        return _instance;
    }
    #endregion
    /// <summary>
    /// �Ӷ�����л�ȡ����
    /// </summary>
    /// <param name="objName"></param>
    /// <returns></returns>
    public GameObject GetObj(string objName)
    {
        //�������
        GameObject result = null;
        //�ж��Ƿ��и����ֵĶ����
        if (pool.ContainsKey(objName))
        {
            Debug.Log(pool[objName].Count);
            //��������ж���
            if (pool[objName].Count>0)
            {
                result = pool[objName][0];
                result.SetActive(true);
                pool[objName].Remove(result);
                //���ؽ��
                return result;
            }
        }

        //���û�и����ֵĶ���ػ��߸����ֶ����û�ж���
        GameObject prefab = null;
        //����Ѿ����ع���Ԥ����
        if (prefabs.ContainsKey(objName))
        {
            prefab = prefabs[objName];
        }
        else
        {
            //����Ԥ����
            prefab = Resources.Load<GameObject>("Prefabs/" + objName);
            //�����ֵ�
            prefabs.Add(objName, prefab);
        }
        //����
        result = UnityEngine.Object.Instantiate(prefab);
        //������ȥ�� Clone��
        result.name = objName;
        return result;
    }
    /// <summary>
    /// ���ն��󵽶����
    /// </summary>
    /// <param name="objName"></param>
    public void RecycleObj(GameObject obj)
    {
        //����Ϊ�Ǽ���
        obj.SetActive(false);
        //�ж��Ƿ��иö���Ķ����
        if (pool.ContainsKey(obj.name))
        {
            //���õ��ö����
            pool[obj.name].Add(obj);
        }
        else
        {
            Debug.Log(obj.name);
            //���������͵ĳ��ӣ������������
            pool.Add(obj.name, new List<GameObject>() { obj });
        }
    }
}
