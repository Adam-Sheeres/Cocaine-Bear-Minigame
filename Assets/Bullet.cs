using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 shootDir;
    private GameObject bear;

    public void setup(Vector3 shootDir)
    {
        this.shootDir = shootDir;
        transform.eulerAngles = new Vector3(0, 0, GetAnglefromVectorFloat(shootDir));
        bear = GameObject.FindWithTag("Player");
        Destroy(gameObject, 5f);
            
    }

    public static float GetAnglefromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }

    private void Update()
    {
        float moveSpeed = 10f;
        transform.position += shootDir * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == bear)
        {
            bear.GetComponent<PlayerStatus>().sendMessage("take damage");
        }
    }


}
