using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private static PlayerMove m_instance;
    public static PlayerMove Instance {
        get
        {
            if (m_instance == null)
            {
               m_instance = FindObjectOfType<PlayerMove>();
            }
            return m_instance;
        }
    }

    public List<GameObject> UnUsedSlashEf = new List<GameObject>();

    public float moveSpeed;
   // public float jumpPower;
    public float dash = 3;

    public WeafonInfo myWeafon;

    public bool canMove = true;
  //  public bool canJump = true;
    public bool canAtk = true;
    public bool canDash = true;
    [SerializeField] private GameObject SlashEffect;
    public GameObject dashController;
    public LayerMask dashTarget;
    public LayerMask wall;
    Rigidbody2D rigid;
    Vector3 vec;
    public Animator legAnim;
    public Animator handAnim;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        dashController = GameObject.Find("Dash");
        dashController.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        //if (Input.GetMouseButtonDown(1))
          //  Dash();
        if (Input.GetMouseButton(0))
            Attack();
          
        
        //if (Input.GetKeyDown(KeyCode.Space))
          //  Jump();

    }

    void Attack()
    {
        if (canAtk)
        {
       
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject effect;
            if (UnUsedSlashEf.Count == 0)
            {
                effect = Instantiate(SlashEffect, transform);
                effect.transform.parent = null;
                effect.transform.localScale = new Vector3(-myWeafon.weafonScale, myWeafon.weafonScale, 1);
            }
            else
            {
                effect = UnUsedSlashEf[0];
                effect.transform.position = this.transform.position;
                UnUsedSlashEf.RemoveAt(0);
                effect.transform.localScale = new Vector3(-myWeafon.weafonScale, myWeafon.weafonScale, 1);
                effect.SetActive(true);
            }
            mousePos = new Vector3(mousePos.x - transform.position.x, mousePos.y - transform.position.y, 0);
            Debug.Log(mousePos);
            effect.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(mousePos.y, mousePos.x) * 180 / Mathf.PI));
            if (Mathf.Pow(mousePos.x, 2) + Mathf.Pow(mousePos.y, 2) >= Mathf.Pow(myWeafon.reach, 2))
                effect.transform.position += mousePos.normalized * myWeafon.reach;
            else
                effect.transform.position += mousePos;
            effect.SetActive(true);
            canAtk = false;
            handAnim.SetBool("isAtk", true);
            StartCoroutine(AtkCoroutine());
        }
    }


    void Move()
    {
        if (canMove)
        {
            float horizontalPos = Input.GetAxisRaw("Horizontal");
            float verticalPos = Input.GetAxisRaw("Vertical");
            if (horizontalPos > 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                dashController.transform.localScale = new Vector3(1, 1, 1);
                legAnim.SetBool("isWalk", true);
                handAnim.SetBool("isWalk", true);
            }
            else if (horizontalPos < 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
                dashController.transform.localScale = new Vector3(1, 1, 1);
                legAnim.SetBool("isWalk", true);
                handAnim.SetBool("isWalk", true);
            }
            if (verticalPos < 0)
            {
                legAnim.SetBool("isWalk", true);
                handAnim.SetBool("isWalk", true);
            }
            else if (verticalPos < 0)
            {
                legAnim.SetBool("isWalk", true);
                handAnim.SetBool("isWalk", true);
            }
            if(verticalPos==0 && horizontalPos==0)
            {
                legAnim.SetBool("isWalk", false);
                handAnim.SetBool("isWalk", false);
            }
            vec = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            vec = new Vector3(vec.x * Time.deltaTime * moveSpeed, vec.y * Time.deltaTime * moveSpeed);
            //if (verticalPos * horizontalPos != 0)
            //{
            //    vec = new Vector3(Input.GetAxisRaw("Horizontal") * Time.deltaTime * moveSpeed/2, Input.GetAxisRaw("Vertical") * Time.deltaTime * moveSpeed/2);

            //}
            //else
            //{
            //    vec = new Vector3(Input.GetAxisRaw("Horizontal") * Time.deltaTime * moveSpeed, Input.GetAxisRaw("Vertical") * Time.deltaTime * moveSpeed);
            //}
            //이렇게 짜면 대각선으로 갈때 발이 4개다.
            transform.Translate(vec);
        }
    }

    void Dash()
    {
        if (canDash)
        {
            canDash = false;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 firstPos = transform.position;
            Debug.Log(mousePos);
            mousePos = new Vector3(mousePos.x - transform.position.x, mousePos.y - transform.position.y, 0).normalized;
            Debug.Log(mousePos);
            StartCoroutine(DashCoroutine());
            for (float i = dash; i > 0.3; i -= 0.1f)
            {
                RaycastHit2D hit1 = Physics2D.Raycast(transform.position, mousePos, i, wall);
                RaycastHit2D hit2 = Physics2D.Raycast(new Vector3(transform.position.x, transform.position.y-0.7f), mousePos, i, wall);
                RaycastHit2D hit3 = Physics2D.Raycast(new Vector3(transform.position.x, transform.position.y + 0.9f), mousePos, i, wall);
                if (!hit1 && !hit2 && !hit3)
                {
                    Debug.Log(i);
                    legAnim.SetBool("isDash", true);
                    handAnim.SetBool("isDash", true);
                    Debug.Log("Dash2");
                    RaycastHit2D[] hits2 = Physics2D.RaycastAll(transform.position, mousePos, i, dashTarget);
                    foreach (var hit in hits2)
                    {

                    }
                    //mousePos = new Vector3(mousePos.x - transform.position.x, mousePos.y - transform.position.y, 0);
                    transform.Translate(mousePos.normalized * (i-0.25f));
                    dashController.SetActive(true);
                    dashController.transform.position = this.transform.position;
                    dashController.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(mousePos.y, mousePos.x) * 180 / Mathf.PI));
                    Debug.Log(Mathf.Atan2(mousePos.y - firstPos.y, mousePos.x - firstPos.x) * 180 / Mathf.PI);
                    break;
                }
            }
        }
    }
    //void Jump()
    //{
    //    if (canJump == true)
    //    {
    //        Debug.Log("Jump!");
    //        rigid.velocity = Vector2.zero;
    //        Vector2 jumpForce = new Vector2(0, jumpPower);
    //        rigid.AddForce(jumpForce, ForceMode2D.Impulse);
    //        canJump = false;
    //        canMove = true;
    //    }
    //}

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    Debug.Log("Trigger");
    //    if (collision.gameObject.tag == "Ground")
    //        canJump = true;
    //}


    IEnumerator DashCoroutine()
    {
        canMove = false;
        yield return new WaitForSeconds(0.1f);
        canMove = true;
        canDash = true;
        dashController.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        legAnim.SetBool("isDash", false);
        handAnim.SetBool("isDash", false);
    }
    IEnumerator AtkCoroutine()
    {
        float waitTime = 1f / (float)myWeafon.atkSpeed;
        handAnim.speed = (float)myWeafon.atkSpeed / 10;
        yield return new WaitForSeconds(waitTime);
        if (!Input.GetMouseButton(0))
        {
            handAnim.SetBool("isAtk", false);
            handAnim.speed = 1;
        }
        canAtk = true;
    }
}
