using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeMove : Monster
{
    public bool canJump = true;
    public bool jumpCoroutineRun = false;
    public bool canMove = false;
    bool chase;
    float targetDir;
    [SerializeField] protected float jumpPower = 1;

    Rigidbody2D rigid;

    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        Move();
        Jump();
        if (hp <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    protected virtual void Move()
    {
        if (canMove)
        {
            if (tracingMonster && chase)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetDir, transform.position.y), moveSpeed * Time.deltaTime);
                if (this.transform.position.x > targetObj.transform.position.x)
                    transform.localScale = new Vector3(-1, 1, 1);
                else
                    transform.localScale = new Vector3(1, 1, 1);
            }
            else if (moveFlag)
            {

                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x + 5, transform.position.y), moveSpeed * Time.deltaTime);
                if (this.transform.position.x > movePoint[0].x)
                    transform.localScale = new Vector3(-1, 1, 1);
                else
                    transform.localScale = new Vector3(1, 1, 1);


            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x - 5, transform.position.y), moveSpeed * Time.deltaTime);
                if (this.transform.position.x > movePoint[1].x)
                    transform.localScale = new Vector3(-1, 1, 1);
                else
                    transform.localScale = new Vector3(1, 1, 1);

            }
        }
    }

    protected virtual void Jump()
    {
        if (canJump == true)
        {
            chase = false;
            if (findTarget && targetObj.transform.position.x > this.transform.position.x)
            {
                targetDir = transform.position.x + 5;
                chase = true;
            }
            else if (findTarget && targetObj.transform.position.x < this.transform.position.x)
            {
                targetDir = transform.position.x - 5;
                chase = true;
            }
            else
            {
                targetDir = transform.position.x;
            }
            int randomFlag = Random.Range(0, 2);
            if (randomFlag == 0)
                moveFlag = true;
            else
                moveFlag = false;
            Debug.Log("Jump!");
            rigid.velocity = Vector2.zero;
            Vector2 jumpForce = new Vector2(0, jumpPower);
            rigid.AddForce(jumpForce, ForceMode2D.Impulse);
            canJump = false;
            canMove = true;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            Debug.Log("G Hit");
            canMove = false;
            StartCoroutine(JumpCoroutine());
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            canMove = true;
        }
    }


    IEnumerator JumpCoroutine()
    {
        Debug.Log("Start");
        if (!jumpCoroutineRun)
        {
            jumpCoroutineRun = true;
            yield return new WaitForSeconds(2f);
            Debug.Log("End");
            canJump = true;
            canMove = true;
            jumpCoroutineRun = false;
        }
    }
}
