using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public bool tracingMonster;
    protected bool findTarget;
    protected GameObject targetObj;
    public int MaxHp;
    [SerializeField]protected int hp;
    [SerializeField]protected int atk;
    [SerializeField]protected float atkSpeed;
    [SerializeField]protected float moveSpeed;
    [SerializeField]protected Vector3[] movePoint = new Vector3[2];
    [SerializeField]protected bool moveFlag = true;
    [SerializeField]protected Material normalMaterial;
    [SerializeField]protected Material hitMaterial;

    public int Hp { get => hp; set => hp = value; }
    public int Atk { get => atk; set => atk = value; }
    public float AtkSpeed { get => atkSpeed; set => atkSpeed = value; }

    protected virtual void OnEnable()
    {
        hp = MaxHp;
    }


    protected virtual void Update()
    {
        Move();
        if (hp <= 0)
        {
            Destroy(this.gameObject);
        }
    }
    public void Hit()
    {
        StartCoroutine(HitCoroutine());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !collision.isTrigger)
        {
            findTarget = true;
            targetObj = collision.gameObject;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !collision.isTrigger)
        {
            findTarget = false;
        }
    }
    protected virtual void Move()
    {
        if(tracingMonster && findTarget) {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetObj.transform.position.x, movePoint[0].y), moveSpeed * Time.deltaTime);
            if (this.transform.position.x > targetObj.transform.position.x)
                transform.localScale = new Vector3(-1, 1, 1);
            else
                transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveFlag)
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoint[0], moveSpeed * Time.deltaTime);
            if (this.transform.position.x > movePoint[0].x)
                transform.localScale = new Vector3(-1, 1, 1);
            else
                transform.localScale = new Vector3(1, 1, 1);
            if (transform.position.x == movePoint[0].x)
                moveFlag = false;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoint[1], moveSpeed * Time.deltaTime);
            if (this.transform.position.x > movePoint[1].x)
                transform.localScale = new Vector3(-1, 1, 1);
            else
                transform.localScale = new Vector3(1, 1, 1);
            if (transform.position.x == movePoint[1].x)
                moveFlag = true;
        }
    }

    IEnumerator HitCoroutine()
    {
        GetComponent<Renderer>().material = hitMaterial;
        yield return new WaitForSeconds(0.1f);
        GetComponent<Renderer>().material = normalMaterial;
    }
}
