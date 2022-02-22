using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUIController : MonoBehaviour
{
	//�۲�Ŀ��
	public Transform player;
	// public GameObject luotianyi;
	public bool Iscontroller = false;
	public static CameraUIController _instance;

	private void Awake()
	{
		_instance = this;
	}
	//�۲����
	public float Distance = 3f;
	//��ת�ٶ�
	private float SpeedX = 60;
	private float SpeedY = 50;
	//�Ƕ�����
	private float MinLimitY = -30;
	private float MaxLimitY = 60;
	//��ת�Ƕ�
	private float mX = 0.0f;
	private float mY = 0.0f;
	//������ž�����ֵ
	private float MaxDistance = 5.5f;
	private float MinDistance = 0f;
	//�����������
	private float ZoomSpeed = 5F;
	//�Ƿ����ò�ֵ
	public bool isNeedDamping = true;
	//�ٶ�
	public float Damping = 10F;
	//�洢�Ƕȵ���Ԫ��
	private Quaternion mRotation;

	private Vector3 mPosition;

	//������갴��ö��
	private enum MouseButton
	{
		//������
		MouseButton_Left = 0,
		//����Ҽ�
		MouseButton_Right = 1,
		//����м�
		MouseButton_Midle = 2
	}
	//����ƶ��ٶ�
	//private float MoveSpeed=5.0F;
	//��Ļ����
	private Vector3 mScreenPoint;
	//����ƫ��
	private Vector3 mOffset;
	private Vector3 pos;
	private Quaternion mark;
	private Vector3 cposition;
	private bool IsRecentqiang;

	//�Ƕ�����
	private float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360) angle += 360;
		if (angle > 360) angle -= 360;
		return Mathf.Clamp(angle, min, max);
	}
	void Start()
	{
		Camera.main.fieldOfView = Camera.main.fieldOfView;
		//��ʼ����ת�Ƕ�
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
		//����������
		Distance -= Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed;
		Distance = Mathf.Clamp(Distance, MinDistance, MaxDistance);

		if (player != null /*&& Input.GetMouseButton((int)MouseButton.MouseButton_Right)*/)
		{    //��ȡ�������
			mX += Input.GetAxis("Mouse X") * SpeedX * 0.02F;
			mY -= Input.GetAxis("Mouse Y") * SpeedY * 0.02F;
			//��Χ����
			mY = ClampAngle(mY, MinLimitY, MaxLimitY);
			// mX = ClampAngle(mX, MinLimitY, MaxLimitY);
			//������ת
			mRotation = Quaternion.Euler(mY, mX, 0);
			if (isNeedDamping)//���ݲ�ֵ��ͬ��ȡ��ͬ�Ƕȼ�����ʽ
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
			//Debug.Log("����"+ raycastHit.collider.name);
			float cosA = Vector3.Dot(playerPos.normalized, raycastHit.point.normalized);
			//Debug.Log(cosA);
			float offes = 2 * cosA;
			float CurDistance = Mathf.Abs(Vector3.Distance(playerPos, raycastHit.point))- offes;//��������ӷ�Χ
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
