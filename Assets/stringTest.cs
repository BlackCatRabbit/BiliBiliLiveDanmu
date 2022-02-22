using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class stringTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		JsonData jsonData = new JsonData();
		jsonData["uid"] = 0;
		jsonData["roomid"] = 1000111;
		jsonData["protover"] = 3;
		jsonData["platform"] = "web";
		jsonData["clientver"] = "1.6.3";
		jsonData["type"] = 2;
		jsonData["key"] = "token";
		string json1 = jsonData.ToJson();
		jsonData["uid"] = 111;
		jsonData["roomid"] = 125651456;
		jsonData["protover"] = 3;
		jsonData["platform"] = "web";
		jsonData["clientver"] = "1.5.3";
		jsonData["type"] = 2;
		jsonData["key"] = "token";
		string json2 = jsonData.ToJson();


		DtrSplitToJson(json2+"e"+json1);


	}

	public List<JsonData> DtrSplitToJson(string str)
	{
		string item = str;
		List<JsonData> msgJsonStr = new List<JsonData>();

		print(item.Length);
		int count = -1;
		int Startcharindex = -1;
		int Endcharindex = -1;
		for (int i = 0; i < item.Length; i++)
		{
			if (item[i] == '{')
			{
				if (Startcharindex == -1)
				{
					count = 0;
					Startcharindex = i;
				}
				count++;
			}
			if (item[i] == '}')
			{
				count--;

			}
			if (count == 0)
			{
				Endcharindex = i;
				int lengh = Endcharindex - Startcharindex + 1;
				//print(item.Substring(Startcharindex, lengh));
				JsonData msgJsonData = JsonMapper.ToObject(item.Substring(Startcharindex, lengh));
				msgJsonStr.Add(msgJsonData);
				count = -1;
				Startcharindex = -1;
				Endcharindex = -1;
			}

		}
		//foreach(var msgItem in msgJsonStr)
		//{
		//	print(msgItem["uid"]);

		//}

		return msgJsonStr;
	}
    // Update is called once per frame
    void Update()
    {
        
    }
}
