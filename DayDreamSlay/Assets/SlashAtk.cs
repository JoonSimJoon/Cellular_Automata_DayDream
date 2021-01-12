using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashAtk : MonoBehaviour
{
    public float DeleteTime = 0.2f;
    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(SlashOff());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Monster" && collision.isTrigger == false)
        {
            Monster monster = collision.gameObject.GetComponent<Monster>();
            monster.Hp -= PlayerMove.Instance.myWeafon.damage;
            monster.Hit();
        }
    }

    IEnumerator SlashOff()
    {
        Debug.Log("Slash!");
        yield return new WaitForSeconds(DeleteTime);
        PlayerMove.Instance.UnUsedSlashEf.Add(this.gameObject);
        gameObject.SetActive(false);
    }
}
