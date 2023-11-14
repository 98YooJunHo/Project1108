using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    [Header("�κ��丮")]
    public InvenManager inventory;
    public GameObject inven;

    private Animator animator;
    private float aniSpeed = 0; //�ִϸ��̼� �ӵ�

    //private Player_UI uI;
    private float horizontal;
    private float vertical;
    public int speed = 5;

    public GameObject hitobject;

    public bool moving = false;

    private ObjItem obj;

    [SerializeField]//�ν����Ϳ����� ���� �����ϰ�
    private float smoothRotationTime;//target ������ ȸ���ϴµ� �ɸ��� �ð�
    [SerializeField]
    private float smoothMoveTime;//target �ӵ��� �ٲ�µ� �ɸ��� �ð�
    [SerializeField]
    private float moveSpeed;//�����̴� �ӵ�
    private float rotationVelocity;//The current velocity, this value is modified by the function every time you call it.
    private float speedVelocity;//The current velocity, this value is modified by the function every time you call it.
    private float currentSpeed;
    private float targetSpeed;

    public GameObject equip;
    private InvenManager equip_;

    // Start is called before the first frame update
    void Start()
    {
        inventory = inven.GetComponent<InvenManager>();
        animator = GetComponent<Animator>();
        equip_ = equip.GetComponent<InvenManager>();
        //uI = GetComponent<Player_UI>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();
        RayCast();
        Equip();
        if (Input.GetMouseButtonDown(1))
        {
            inventory.RemoveItem();
        }

    }

    private void PlayerMove()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        Vector2 input = new Vector2(horizontal, vertical);
        //GetAxisRaw("Horizontal") :������ ����Ű������ 1�� ��ȯ, �ƹ��͵� �ȴ����� 0, ���ʹ���Ű�� -1 ��ȯ
        //GetAxis("Horizontal"):-1�� 1 ������ �Ǽ����� ��ȯ
        //Vertical�� ���ʹ���Ű ������ 1,�ƹ��͵� �ȴ����� 0, �Ʒ��ʹ���Ű�� -1 ��ȯ

        Vector2 inputDir = input.normalized;
        //���� ����ȭ. ���� input=new Vector2(1,1) �̸� �������� �밢������ �����δ�.
        //������ ã���ش�

        if (inputDir != Vector2.zero)//�������� ������ �� �ٽ� ó�� ������ ���ư��°� ��������
        {
            float rotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, rotation, ref rotationVelocity, smoothRotationTime);
        }
        //������ �����ִ� �ڵ�, �÷��̾ ������ �� �밢������ �����Ͻ� �� ������ �ٶ󺸰� ���ش�
        //Mathf.Atan2�� ������ return�ϱ⿡ �ٽ� ������ �ٲ��ִ� Mathf.Rad2Deg�� �����ش�
        //Vector.up�� y axis�� �ǹ��Ѵ�
        //SmoothDampAngle�� �̿��ؼ� �ε巴�� �÷��̾��� ������ �ٲ��ش�.

        targetSpeed = moveSpeed * inputDir.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedVelocity, smoothMoveTime);
        //���罺�ǵ忡�� Ÿ�ٽ��ǵ���� smoothMoveTime ���� ���Ѵ�
        transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);

        if (Mathf.Abs(horizontal) > 0 || Mathf.Abs(vertical) > 0)
        {
            aniSpeed = Mathf.MoveTowards(aniSpeed, 1, Time.deltaTime * 3);
            moving = true;
        }
        else
        {
            aniSpeed = Mathf.MoveTowards(aniSpeed, 0, Time.deltaTime * 3);
            moving = false;
        }
        //animator.SetFloat("Speed", aniSpeed);      


        ///////////////////////////////
        //// �����̴� ������ ī�޶� �������� �غ���
        ///////////////////////////////
    }

    private void RayCast()
    {
        RaycastHit hitInfo;
        Debug.DrawRay(transform.position + new Vector3(0, 0.5f, 0), transform.forward * 2.0f, Color.magenta);
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), transform.forward, out hitInfo, 2.0f))
            {

                if (hitInfo.collider != null)
                {

                    HitCheckObject(hitInfo);

                }
            }
            else
            {
                hitobject = null;
            }
        }
    }
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
    //        Debug.Log(hit.collider.gameObject.name);
    //        if (hit.collider != null)
    //        {
    //            Debug.Log("3");
    //            HitCheckObject(hit);
    //        }
    //    }
    void HitCheckObject(RaycastHit hitInfo)
    {
        obj = hitInfo.transform.gameObject.GetComponent<ObjItem>();

        if (obj != null)
        {
            Item item = obj.ClickItem();
            inventory.AddItem(item);
        }
    }

    void Equip()
    {
        for (int i = 1; i <10; i++)
        {
            if(Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                
                equip_.Equip(i);
            }
        }
    }    
}
