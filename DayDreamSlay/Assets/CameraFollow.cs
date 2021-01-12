using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float speed;
    float val = 0;
    private void Start()
    {
    }
    void Update()
    {
        val = speed - ((speed - val) * (Mathf.Pow(0.1f, Time.deltaTime)));
        this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(PlayerMove.Instance.gameObject.transform.position.x
            , PlayerMove.Instance.gameObject.transform.position.y + 3
            , PlayerMove.Instance.gameObject.transform.position.z - 14), val);
    }
}


