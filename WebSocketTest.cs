using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading;
using UnityEngine;
using System.Text;
using LitJson;
using System.Net.WebSockets;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using Brotli;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using UnityEditor;
using UnityEngine.UI;

public class WebSocketTest : MonoBehaviour
{
	public Text testText;

	public int roomID;

	public bool isTextInput;
	public float MaxCount = 10;//�������
	public GameObject HudParent;
	public GameObject PlayerNcpPos;
	public bool IsLink=true;

	ClientWebSocket ws = new ClientWebSocket();
	CancellationToken ct = new CancellationToken();
	WebSocketState webSocketState = new WebSocketState();

	List<JsonData> danmuJsonList = new List<JsonData>();

	Dictionary<GameObject, string> playerNpcSDic = new Dictionary<GameObject, string>();
	public List<GameObject> playeNpcList = new List<GameObject>();

	private GameController gameController;
	private void Awake()
	{
		gameController = GetComponent<GameController>();
	}

	Thread receiveThread2;

	// Start is called before the first frame update
	void Start()
    {
		roomID = int.Parse(InputUIController._instance.text);
		for (int i = 0; i < playeNpcList.Count; i++)
		{
			playeNpcList[i].GetComponent<NpcFloowPlayer>().HUDParent = HudParent;
			playeNpcList[i].SetActive(false);
			playerNpcSDic.Add(playeNpcList[i], i.ToString());
		}
		//Post("6750632");
		WebSocketBiliBiliMsg();

	}

    // Update is called once per frame
    void Update()
    {

	}


    public async void WebSocketBiliBiliMsg()
	{
		var room_id = roomID;
		JsonData jsonData = new JsonData();
		jsonData["uid"] = 0;
		jsonData["roomid"] = room_id;
		jsonData["protover"] = 3;
		jsonData["platform"] = "web";
		jsonData["clientver"] = "1.6.3";
		jsonData["type"] = 2;
		jsonData["key"] = "token";
		string json1 = jsonData.ToJson();
		//Debug.Log(json1);
		MsgBody sBody = new MsgBody(json1);
		sBody.PacketLength = Encoding.UTF8.GetBytes(json1).Length + 16;
		sBody.HeaderLength = 16;
		sBody.ProtocolVersion = 1;
		sBody.Operation = 7;
		sBody.SequenceId = 1;
		//Debug.Log(sBody.ToByteArray()[5]);


		try
		{
			// add header
			//ws.Options.SetRequestHeader("X-Token", "eyJhbGciOiJIUzI1N");
			Uri url = new Uri("wss://broadcastlv.chat.bilibili.com:2245/sub");
			await ws.ConnectAsync(url, ct);
			await ws.SendAsync(new ArraySegment<byte>(sBody.ToByteArray()), WebSocketMessageType.Binary, true, ct);
			Debug.Log("�������");
			testText.text = "�������";
		}
		catch (Exception ex)
		{
			Debug.Log(ex.Message);
			if (ex.Message == "The remote party closed the WebSocket connection without completing the close handshake.")
			{
				Debug.Log("�������ر�");
				testText.text = "�������ر�";
			}
		}

		//ͨ��ClientWebSocket�������� 
		reMsg(ws);
		//Thread receiveThread = new Thread(reMsg);
		//receiveThread.Start(ws);

		receiveThread2 = new Thread(SendMsg);

		receiveThread2.Start(ws);


	}

	async void reMsg(object ws)
	{
		ClientWebSocket sk = (ClientWebSocket)ws;
		byte[] saveArr = new byte[0];
		var result = new byte[5000];

		int PacketLength1 = 0;//����ܴ�С
		byte[] PacketLengthByte1;

		Int16 HeaderLength1 = 16;//ͷ������
		byte[] HeaderLengthByte1;

		Int16 ProtocolVersion1 = 1;//Э��汾
		byte[] ProtocolVersionByte1;

		int Operation1 = 7;//������ 7��ʾ��֤�����뷿��
		byte[] OperationByte1;

		int SequenceId1 = 1;//��1
		byte[] SequenceIdByte1;

		string BodyData1;//��������
		byte[] BodyDataByte1;

		while (IsLink)
		{

            try
			{
				
				await sk.ReceiveAsync(new ArraySegment<byte>(result), new CancellationToken());
				//Debug.Log(sk.State);

				PacketLengthByte1 = SubByte(result, 0, 4);
				//Debug.Log("result____" + result.Length);
				Array.Reverse(PacketLengthByte1);
				PacketLength1 = BitConverter.ToInt32(PacketLengthByte1, 0);
				//Debug.Log("PacketLength1_____"+PacketLength1);

				HeaderLengthByte1 = SubByte(result, 4, 2);
				Array.Reverse(HeaderLengthByte1);
				HeaderLength1= BitConverter.ToInt16(HeaderLengthByte1, 0);
				//Debug.Log("HeaderLength1_____" + HeaderLength1);

				ProtocolVersionByte1 = SubByte(result, 6, 2);
				Array.Reverse(ProtocolVersionByte1);
				ProtocolVersion1 = BitConverter.ToInt16(ProtocolVersionByte1, 0);
				//Debug.Log("ProtocolVersion1_____" + ProtocolVersion1);

				OperationByte1 = SubByte(result, 8, 4);
				Array.Reverse(OperationByte1);
				Operation1 = BitConverter.ToInt16(OperationByte1, 0);
				//Debug.Log("Operation1_____" + Operation1);

				SequenceIdByte1 = SubByte(result, 12, 4);
				Array.Reverse(SequenceIdByte1);
				SequenceId1 = BitConverter.ToInt16(SequenceIdByte1, 0);
				//Debug.Log("SequenceId1_____" + SequenceId1);

				if(Operation1==5)//��Ļ
				{
					int offset = 0;
					string BodyStr="";
					while (offset< PacketLength1)
					{
						BodyData1 = "";
						BodyDataByte1 = SubByte(result, offset+16, offset+PacketLength1);
						try
						{
							// pako�����޷���ѹ
							//Array.Reverse(BodyDataByte1);
							if (ProtocolVersion1 == 0)
							{
								BodyData1 = Encoding.UTF8.GetString(BodyDataByte1, 0, BodyDataByte1.Length);
							}
							if (ProtocolVersion1 == 1)
							{
								BodyData1 = Encoding.UTF8.GetString(BodyDataByte1, 0, BodyDataByte1.Length);
							}
							if (ProtocolVersion1==2)
							{
								byte[] BodyDataByte2 = SharpZipLibDecompress(BodyDataByte1);
								BodyData1 = Encoding.UTF8.GetString(BodyDataByte2, 0, BodyDataByte2.Length);
							}
							if (ProtocolVersion1 == 3)
							{
								byte[] BodyDataByte2 = BufferDecompress(BodyDataByte1);
								BodyData1 = Encoding.UTF8.GetString(BodyDataByte2, 0, BodyDataByte2.Length);

								//AssetDatabase.Refresh();

								//Debug.Log("BodyData1Lenght{" + BodyData1[1] + "}");
								danmuJsonList=DtrSplitToJson(BodyData1);
								foreach (var msgItem in danmuJsonList)
								{
									bool IsKey = false; ;
									print(msgItem["cmd"]);
									testText.text = msgItem["cmd"].ToString();
									if ((string)msgItem["cmd"] == "DANMU_MSG")
									{
										//print("���ƣ�" + msgItem["info"][2][1]);
										//print("��Ļ��" + msgItem["info"][1]);
										//���j��������ģ��
										gameController.CreateGameObject(msgItem["info"][2][1].ToString(), msgItem["info"][1].ToString());
										//ͨ�������ǳ��ж��Ƿ���ͬ������µ�Ļ��������ͬ����µ�Ļ
										//�����������
										//��������100
									}
									if ((string)msgItem["cmd"] == "SEND_GIFT")
                                    {
										if (isTextInput)
										{
											string path = Application.dataPath + "/test1.txt";
											using (FileStream fs = new FileStream(path, FileMode.Create))
											{
												fs.Write(BodyDataByte2, 0, BodyDataByte2.Length);
												fs.Dispose();
											}
										}
									}
								}
							}
						}
						catch (Exception ex)
						{
							Debug.Log(ex.Message);
							testText.text = ex.Message;
							BodyData1 = Encoding.UTF8.GetString(BodyDataByte1, 0, BodyDataByte1.Length);
						}
						BodyStr = BodyData1;
						offset += PacketLength1;
					}
				}
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
				if (ex.Message == "The remote party closed the WebSocket connection without completing the close handshake.")
				{
					Debug.Log("�������ر�");
				}
			}
		}
	}
	/// <summary>
	/// ����
	/// </summary>
	/// <param name="socket"></param>
	public async void SendMsg(object ws)
	{
		MsgBody sBody1 = new MsgBody("");
		sBody1.PacketLength = Encoding.UTF8.GetBytes("").Length + 16;
		sBody1.HeaderLength = 16;
		sBody1.ProtocolVersion = 1;
		sBody1.Operation = 2;
		sBody1.SequenceId = 1;
		ClientWebSocket sk = (ClientWebSocket)ws;
		while (true)
		{
			await sk.SendAsync(new ArraySegment<byte>(sBody1.ToByteArray()), WebSocketMessageType.Binary, true, ct);
            Thread.Sleep(30000);
			if (!IsLink)
				break;
		}
        if (receiveThread2.ThreadState == ThreadState.Running)
        {
            print("�����̹߳رգ�");
            receiveThread2.Abort();
        }

    }
	private void OnDestroy()
	{
		//�ر�ͨѶ�߳�
		IsLink = false;
	}
	/// <summary>  
	/// ��ȡ�ֽ�����  
	/// </summary>  
	/// <param name="srcBytes">Ҫ��ȡ���ֽ�����</param>  
	/// <param name="startIndex">��ʼ��ȡλ�õ�����</param>  
	/// <param name="length">Ҫ��ȡ���ֽڳ���</param>  
	/// <returns>��ȡ����ֽ�����</returns>  
	public static byte[] SubByte(byte[] srcBytes, int startIndex, int length)
	{
		System.IO.MemoryStream bufferStream = new System.IO.MemoryStream();
		byte[] returnByte = new byte[] { };
		if (srcBytes == null) { return returnByte; }
		if (startIndex < 0) { startIndex = 0; }
		if (startIndex < srcBytes.Length)
		{
			if (length < 1 || length > srcBytes.Length - startIndex) { length = srcBytes.Length - startIndex; }
			bufferStream.Write(srcBytes, startIndex, length);
			returnByte = bufferStream.ToArray();
			bufferStream.SetLength(0);
			bufferStream.Position = 0;
		}
		bufferStream.Close();
		bufferStream.Dispose();
		return returnByte;
	}
	public int readInt(byte[] buffer, int start, int lengh)
	{
		int result = 0;
		for(int i = lengh - 1;i>= 0;i--)
		{
			result += (int)Math.Pow(256, lengh - i - 1 * buffer[start + i]);
		}
		return result;
	}
	public static byte[] SharpZipLibDecompress(byte[] data)
	{
		MemoryStream compressed = new MemoryStream(data);
		MemoryStream decompressed = new MemoryStream();
		InflaterInputStream inputStream = new InflaterInputStream(compressed);
		inputStream.CopyTo(decompressed);
		return decompressed.ToArray();
	}
	// ʹ��System.IO.Compression����Deflate��ѹ
	public byte[] BufferDecompress(byte[] data)
	{
		testText.text = "���ڽ�ѹ��";
		Debug.Log("���ڽ�ѹ��");
		MemoryStream compressed = new MemoryStream(data);
		byte[] result = compressed.DecompressFromBrotli();
		if (isTextInput)
		{
			string path = Application.dataPath + "/test.txt";
			FileStream fs = new FileStream(path, FileMode.Create);
			fs.Write(result, 0, result.Length);
			fs.Dispose();
		}


		return result;
	}
	public static int bytesToInt(byte[] src, int offset)
	{
		int value;
		value = (int)((src[offset] & 0xFF)
				| ((src[offset + 1] & 0xFF) << 8)
				| ((src[offset + 2] & 0xFF) << 16)
				| ((src[offset + 3] & 0xFF) << 24));
		return value;
	}

	/** 
	* byte������ȡint��ֵ��������������(��λ�ں󣬸�λ��ǰ)��˳�򡣺�intToBytes2��������ʹ��
	*/
	public static int bytesToInt2(byte[] src, int offset)
	{
		int value;
		value = (int)(((src[offset] & 0xFF) << 24)
				| ((src[offset + 1] & 0xFF) << 16)
				| ((src[offset + 2] & 0xFF) << 8)
				| (src[offset + 3] & 0xFF));
		return value;
	}
	public List<JsonData> DtrSplitToJson(string str)
	{
		string item = str;
		List<JsonData> msgJsonStr = new List<JsonData>();

		//print(item.Length);
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


		return msgJsonStr;
	}
}



