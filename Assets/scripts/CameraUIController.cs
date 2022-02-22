using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUIController : MonoBehaviour
{
	//观察目标
	public Transform player;
	// public GameObject luotianyi;
	public bool Iscontroller = false;
	public static CameraUIController _instance;

	private void Awake()
	{
		_instance = this;
	}
	//观察距离
	public float Distance = 3f;
	//旋转速度
	private float SpeedX = 60;
	private float SpeedY = 50;
	//角度限制
	private float MinLimitY = -30;
	private float MaxLimitY = 60;
	//旋转角度
	private float mX = 0.0f;
	private float mY = 0.0f;
	//鼠标缩放距离最值
	private float MaxDistance = 5.5f;
	private float MinDistance = 0f;
	//鼠标缩放速率
	private float ZoomSpeed = 5F;
	//是否启用差值
	public bool isNeedDamping = true;
	//速度
	public float Damping = 10F;
	//存储角度的四元数
	private Quaternion mRotation;

	private Vector3 mPosition;

	//定义鼠标按键枚举
	private enum MouseButton
	{
		//鼠标左键
		MouseButton_Left = 0,
		//鼠标右键
		MouseButton_Right = 1,
		//鼠标中键
		MouseButton_Midle = 2
	}
	//相机移动速度
	//private float MoveSpeed=5.0F;
	//屏幕坐标
	private Vector3 mScreenPoint;
	//坐标偏移
	private Vector3 mOffset;
	private Vector3 pos;
	private Quaternion mark;
	private Vector3 cposition;
	private bool IsRecentqiang;

	//角度限制
	private float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360) angle += 360;
		if (angle > 360) angle -= 360;
		return Mathf.Clamp(angle, min, max);
	}
	void Start()
	{
		Camera.main.fieldOfView = Camera.main.fieldOfView;
		//初始化旋转角度
		mX = transform.eulerAngles.x;
		mY = transform.eulerAngles.y;
	}

	void Update()
	{
		if (!Iscontroller) return;
		if (Input.GetKeyDown(KeyCode.LeftAlt))
		{
			if(Cursor.visible)
			{
				Cursor.visible = false;

			}
			else
			{
				Cursor.visible = true;

			}
		}
		//Screen.showCursor = false;
		//鼠标滚轮缩放
		Distance -= Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed;
		Distance = Mathf.Clamp(Distance, MinDistance, MaxDistance);

		if (player != null /*&& Input.GetMouseButton((int)MouseButton.MouseButton_Right)*/)
		{    //获取鼠标输入
			mX += Input.GetAxis("Mouse X") * SpeedX * 0.02F;
			mY -= Input.GetAxis("Mouse Y") * SpeedY * 0.02F;
			//范围限制
			mY = ClampAngle(mY, MinLimitY, MaxLimitY);
			// mX = ClampAngle(mX, MinLimitY, MaxLimitY);
			//计算旋转
			mRotation = Quaternion.Euler(mY, mX, 0);
			if (isNeedDamping)//根据差值不同采取不同角度计量方式
			{
				transform.rotation = Quaternion.Lerp(transform.rotation, mRotation, Time.deltaTime * Damping);
				//luotianyi.transform.rotation = Quaternion.Lerp(mRotation, transform.rotation, Time.deltaTime * Damping);
			}
			else
			{
				transform.rotation = mRotation;
				//mRotation=luotianyi.transform.rotation ;
			}
		}

		RaycastHit raycastHit;
		Vector3 playerPos = player.transform.position+ new Vector3(0f, 1.5f, 0);
		Physics.Raycast(playerPos, -playerPos + this.transform.position, out raycastHit);
		if(raycastHit.collider!=null)
		{
			//Debug.Log("看见"+ raycastHit.collider.name);
			float cosA = Vector3.Dot(playerPos.normalized, raycastHit.point.normalized);
			//Debug.Log(cosA);
			float offes = 2 * cosA;
			float CurDistance = Mathf.Abs(Vector3.Distance(playerPos, raycastHit.point))- offes;//摄像机可视范围
			if(Distance> CurDistance)
			{
				if(Distance>=0)
				{
					Distance = CurDistance;
				}
			}
			if (Distance <= 0)
			{
				Distance = 0;
			}
			if(Distance <= CurDistance&& Distance<=3.0f)
			{
				float ver = Input.GetAxis("Vertical");
				float hor = Input.GetAxis("Horizontal");
				if (ver != 0 || hor != 0)
				{
					Distance += 2 * Time.deltaTime;
				}
			}
		}
		mPosition = mRotation * new Vector3(0f, 0f, -Distance) + playerPos;
		if (isNeedDamping)
		{
			transform.position = Vector3.Lerp(transform.position, mPosition, Time.deltaTime * Damping);
		}
		else
		{
			transform.position = mPosition;

		}

	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(player.transform.position+new Vector3(0,1,0), -(player.transform.position + new Vector3(0, 1, 0)) + this.transform.position);
	}
}
