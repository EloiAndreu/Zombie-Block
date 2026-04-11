using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnCountDown : MonoBehaviour
{
    public float liveTime = 1f;

    void Start(){
        StartCoroutine(countDownToDestroy());
    }

    IEnumerator countDownToDestroy(){
        yield return new WaitForSeconds(liveTime);
        Destroy(this.gameObject);
    }
}
