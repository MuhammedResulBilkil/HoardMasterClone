using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallCollider : MonoBehaviour
{
    int collectiblesCount = 0;
    public int collectiblesMaxCount = 5;

    public static FallCollider Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.transform.tag == "Collectibles")
        {
            collectiblesCount++;
            if(collectiblesCount == collectiblesMaxCount)
            {
                collectiblesCount = 0;
                HoleMovement.Instance.SetHoleScale(1.5f);
                //warning that is full, so needs to empty
            }
            Destroy(other.gameObject);
        }
    }

}
