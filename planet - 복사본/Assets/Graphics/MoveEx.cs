using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEx : MonoBehaviour
{
    public Rigidbody cube;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MoveObject());
    }

    IEnumerator MoveObject()
    {
        cube = GetComponent<Rigidbody>();

        while (true)
        {
            float dir1 = Random.Range(-1f, 1f);
            float dir2 = Random.Range(-1f, 1f);
            float dir3 = Random.Range(-1f, 1f);

            yield return new WaitForSeconds(1);
            cube.velocity = new Vector3(dir1, dir3, dir2);
        }
    }
}
