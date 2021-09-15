using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnWildCouCou : MonoBehaviour
{
    private FindWildCouCou findWildCouCou;
    public CouCouDatabase.Element element;

    private Coroutine previousCoroutine;

    private void Awake()
    {
        findWildCouCou = GameObject.FindGameObjectWithTag("GameManager").GetComponent<FindWildCouCou>();
    }

    public IEnumerator SpawnCouCou()
    {
        while (Time.timeSinceLevelLoad < 3f)
        {
            yield return new WaitForEndOfFrame();
        }
        int rnd = Random.Range(1, 16);
        yield return new WaitForSeconds(rnd);
        findWildCouCou.WildCouCouAttack(element);
    }

    private void OnTriggerEnter(Collider other)
    {
        previousCoroutine = StartCoroutine(SpawnCouCou());
    }

    private void OnTriggerExit(Collider other)
    {
        if (previousCoroutine != null)
        {
            StopCoroutine(previousCoroutine);
            previousCoroutine = null;
        }
    }
}
