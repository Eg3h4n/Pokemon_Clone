using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject health;

    public void SetHP(float hpNormalized)
    {
        health.transform.localScale = new Vector3(hpNormalized, 1f);
    }

    public IEnumerator SetHPSmooth(float newHPp)
    {
        float currentHp = health.transform.localScale.x;
        float changeAmount = currentHp - newHPp;

        while (currentHp - newHPp > Mathf.Epsilon)
        {
            currentHp -= changeAmount * Time.deltaTime;
            health.transform.localScale = new Vector3(currentHp, 1f);
            yield return null;
        }

        health.transform.localScale = new Vector3(newHPp, 1f);
    }
}
