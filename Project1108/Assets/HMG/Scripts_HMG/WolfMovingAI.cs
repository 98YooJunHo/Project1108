using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfMovingAI : MonoBehaviour
{
    Animator wolfAnimator;
    bool isBoundaryOfPc = false;
    //늑대가 범위내에 음식이 있다면 알아서 찾아간다음 음식을 먹는 제스처를 취해야함

    // Start is called before the first frame update
    void Start()
    {
        wolfAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }
        
    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Food") && isBoundaryOfPc==false)
        {
            // 푸드포지션에서 늑대 포지션을빼서 Vector3를 얻음.  얻은 방향으로 Vector3.up을 기준으로 Quaternion.LookRotation을 사용해서 푸드를 바라보게함.
            // Transform.LookAt을 사용하니 xRotation이 원하지 않는 방향으로 회전한다. LookAt()은 반드시 x,y,z Rotation이 같이 움직임
            Vector3 directionToFood = other.gameObject.transform.position - transform.position;
            Quaternion yRotation = Quaternion.LookRotation(directionToFood, Vector3.up);
            yRotation.eulerAngles = new Vector3(0, yRotation.eulerAngles.y, 0);
            // 늑대의 머리가 푸드쪽으로 갑자기 휙바뀌는것이 자연스럽지 않았다.
            // 부드럽게 회전시키기 위해 Quaternion.Slerp를 사용해야한다.
            // 그러나 OnTriggerEnter는 콜라이더 안에 접촉시에 단 한번만 실행됨으로 코루틴으로 작동하도록해야한다.
            Quaternion startRotation = transform.rotation;
            StartCoroutine(RotateSmoothly(startRotation, yRotation, 0.2f));
        }
     
    }



    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.tag.Equals("Player"))
        {
            isBoundaryOfPc = true;
            //animator의 parameter를 바꿔줘야함.
            wolfAnimator.SetTrigger("IsBark");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            if (isBoundaryOfPc)
            {
                isBoundaryOfPc = false;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        isBoundaryOfPc = false;
    }
        private IEnumerator RotateSmoothly(Quaternion startRotation, Quaternion yRotation, float duration)
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, yRotation, elapsedTime/ duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.rotation = yRotation;
        wolfAnimator.SetTrigger("Eat");
    }    
}
