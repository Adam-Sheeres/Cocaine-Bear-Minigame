using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 shootDir;
    private GameObject bear;
    private SphereCollider smallestHitbox;

    public void setup(Vector3 shootDir)
    {
        this.shootDir = shootDir;
        transform.eulerAngles = new Vector3(0, 0, GetAnglefromVectorFloat(shootDir));
        bear = GameObject.FindWithTag("Player");
        Destroy(gameObject, 5f);

        SphereCollider[] hitboxes = bear.GetComponents<SphereCollider>();
        smallestHitbox = hitboxes[0];
        for (int i = 1; i < hitboxes.Length; i++)
        {
            if (hitboxes[i].radius < smallestHitbox.radius)
            {
                smallestHitbox = hitboxes[i];
            }
        }
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
        if (other == smallestHitbox)
        {
            bear.GetComponent<PlayerStatus>().sendMessage("take damage");
            Destroy(gameObject);
        }
    }
}
