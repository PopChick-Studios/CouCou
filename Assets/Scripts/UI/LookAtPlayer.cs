using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    bool isLooking = false;

    public IEnumerator StartLooking(Transform camera)
    {
        isLooking = true;
        while (isLooking)
        {
            gameObject.transform.rotation = Quaternion.LookRotation(gameObject.transform.position - camera.position);
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnDestroy()
    {
        isLooking = false;
        StopAllCoroutines();
    }
}
