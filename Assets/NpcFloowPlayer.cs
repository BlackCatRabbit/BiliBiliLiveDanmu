using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcFloowPlayer : MonoBehaviour
{
	private GameObject HUDdelog;
	private GameObject HUDnameUI;

	public GameObject HUDParent;
	public GameObject delog;
	public GameObject nameUI;
	public Text delogText;
	public Text nameUIText;
	public Vector3 delogoffset = new Vector3(0, 1, 0);
	public Vector3 nameUIoffset = new Vector3(0, 1, 0);

	public float timer=0;

	private void Awake()
	{
		HUDdelog = Resources.Load<GameObject>("Delog");
		HUDnameUI = Resources.Load<GameObject>("NameUI");
		delog = GameObject.Instantiate(HUDdelog);
		nameUI = GameObject.Instantiate(HUDnameUI);
		delogText = delog.transform.Find("danmu").GetComponent<Text>();
		nameUIText = nameUI.GetComponent<Text>();
	}
	// Start is called before the first frame update
	void Start()
    {

		//delogText.text = "wsfsdfsfsf";
	}

    // Update is called once per frame
    void Update()
    {
		if(HUDParent!=null)
        {
			delog.transform.parent = HUDParent.transform;
			nameUI.transform.parent = HUDParent.transform;
		}
		delog.transform.position = this.transform.position+ delogoffset;
		nameUI.transform.position = this.transform.position+ nameUIoffset;

		delog.transform.forward = Camera.main.transform.forward;
		nameUI.transform.forward = Camera.main.transform.forward;

		timer += Time.deltaTime;
		if(timer>=30)
		{
			nameUI.SetActive(false);
			delog.SetActive(false);
			ObjectPool.GetInstance().RecycleObj(gameObject);
		}
		//delog.transform.position = Camera.main.WorldToScreenPoint(this.transform.position+ delogoffset);
		//nameUI.transform.position = Camera.main.WorldToScreenPoint(this.transform.position + nameUIoffset);

	}
}
