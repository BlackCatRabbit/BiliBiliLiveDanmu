using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject HudParent;
    public GameObject GamePoint;
    Dictionary<string, GameObject> playerNpcNameDic = new Dictionary<string, GameObject>();
    //随机产生的物体
    private static GameObject sphere;
    private static GameObject cube;
    private static GameObject cylinder;
    private static GameObject capsule;
    public GameObject[] RandomGameObject =
    {
        sphere,
        cube,
        cylinder,
        capsule
    };

    public GameObject CreateGameObject(string UserName,string danmu)
    {
        GameObject go;
        if (playerNpcNameDic.ContainsKey(UserName))
        {
            if(playerNpcNameDic.TryGetValue(UserName, out go))
            {
                if(go.activeSelf==true)
                {
                    go.GetComponent<NpcFloowPlayer>().HUDParent = HudParent;
                    go.GetComponent<NpcFloowPlayer>().nameUI.SetActive(true);
                    go.GetComponent<NpcFloowPlayer>().delog.SetActive(true);
                    go.GetComponent<NpcFloowPlayer>().nameUIText.text = UserName;
                    go.GetComponent<NpcFloowPlayer>().delogText.text = danmu;
                    go.GetComponent<NpcFloowPlayer>().timer = 0;
                    return go;
                }
            }
        }
        go = ObjectPool.GetInstance().GetObj(RandomGameObject[Random.Range(0, 4)].name);
        go.transform.position = GamePoint.transform.position;
        go.GetComponent<NpcFloowPlayer>().HUDParent = HudParent;
        go.GetComponent<NpcFloowPlayer>().nameUI.SetActive(true);
        go.GetComponent<NpcFloowPlayer>().delog.SetActive(true);
        go.GetComponent<NpcFloowPlayer>().nameUIText.text = UserName;
        go.GetComponent<NpcFloowPlayer>().delogText.text = danmu;
        go.GetComponent<NpcFloowPlayer>().timer = 0;
        if (playerNpcNameDic.ContainsKey(UserName))
        {
            playerNpcNameDic[UserName] = go;
        }
        else
        {
            playerNpcNameDic.Add(UserName, go);
        }
        return go;
    }
    public void RecycleGameObject(GameObject obj)
    {
        ObjectPool.GetInstance().RecycleObj(obj);
    }
}
