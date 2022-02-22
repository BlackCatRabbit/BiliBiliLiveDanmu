using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
	public float speed = 8;

	private float ver;
	private float hor;
	private Quaternion dir;
	public float RotatemoveSpeed;
	private Animator anim;
	private Rigidbody rigidbody;
	public static PlayerMove _instance;
	public GameObject foot1Point;
	public GameObject foot2Point;

	private bool IsGround;
	private float GroundDistance;
	private float VerticalVelocity;
	private float InputMagnitude;

	private void Awake()
	{
		_instance = this;
	}
	void Start()
	{
		anim = this.GetComponent<Animator>();
		rigidbody = GetComponent<Rigidbody>();
	}
	void Update()
	{
		//hor = joystick.Horizontal;
		//ver = joystick.Vertical;
		InputMagnitude = ver * ver + hor * hor;
		anim.SetFloat("InputMagnitude", InputMagnitude);
		ver = Input.GetAxis("Vertical");
		hor = Input.GetAxis("Horizontal");
		Move(hor,ver);
		Jump();
		if(Input.GetMouseButtonDown(0))
        {
			anim.SetTrigger("StrongAttack");
        }
	}
	public void Move(float hor, float ver)
	{
		AnimatorStateInfo animatorInfo = anim.GetCurrentAnimatorStateInfo(0);
		//if(animatorInfo.IsName("Floor_Light_WithNo_Sword")|| animatorInfo.IsName("Floor_Hard_WithNo_Sword"))
  //      {
		//	hor = 0;
		//	ver = 0;
		//}
		if (hor != 0 || ver != 0)
		{
			//anim.applyRootMotion = false;
			//CameraUIController._instance.IscCntroller = false;
			playerRotate(hor, ver,speed);
			if (animatorInfo.IsName("Run_Sword"))
			{
				anim.speed = 1;
			}
			else
            {
				anim.speed = 1;
			}
			anim.SetBool("IsRunSword", true);
		}
		else
		{
			//CameraUIController._instance.IscCntroller = true;
			anim.SetBool("IsRunSword", false);
		}
	}
	public void Jump()
    {
		RaycastHit raycastHit;
		if(Physics.Raycast(foot1Point.transform.position,Vector3.down,out raycastHit)|| Physics.Raycast(foot2Point.transform.position, Vector3.down, out raycastHit))
        {
			if(raycastHit.collider.CompareTag("Ground"))
            {
				GroundDistance = Mathf.Abs(Vector3.Distance(foot1Point.transform.position, raycastHit.point));
				if (GroundDistance < 0.5f)
                {
					//Debug.Log("碰到地面");
					anim.SetBool("IsGrounded", true);
					if (Input.GetKeyDown(KeyCode.Space))
					{
						rigidbody.AddForce(new Vector3(0, 350, 0), ForceMode.Impulse);
						anim.CrossFadeInFixedTime("Jump_Inplace", 0.1f);
					}
				}
                else
                {
					playerRotate(hor, ver, 2);
					anim.SetBool("IsGrounded", false);
				}
            }			
        }

		//Debug.Log("GroundDistance" + GroundDistance);
		VerticalVelocity = rigidbody.velocity.y;
		//Debug.Log("VerticalVelocity" + VerticalVelocity);
		anim.SetFloat("GroundDistance", GroundDistance);
		anim.SetFloat("VerticalVelocity", VerticalVelocity);
	}
	private void playerRotate(float hor, float ver, float speed)
	{
		if (hor == 0 && ver == 0) return;
		dir = Quaternion.LookRotation(new Vector3(hor, 0, ver));
		Quaternion q = Quaternion.identity;
		q.SetLookRotation(Camera.main.transform.forward);//setlookrotaion定义看向指定方向的rotation
		this.transform.rotation = Quaternion.Lerp(this.transform.rotation, dir * Quaternion.Euler(0, q.eulerAngles.y, 0), RotatemoveSpeed * Time.deltaTime);
		//if (!Input.GetKey(KeyCode.J))
		this.transform.Translate(Vector3.forward * Time.deltaTime * speed);
		//rigidbody.AddForce(new Vector3(ver*speed,0, hor* speed), ForceMode.Force);
	}

}
