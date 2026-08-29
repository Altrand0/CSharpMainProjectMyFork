using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyFirstScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int i = 7;
        int f = 3;
          
        bool isEqual = i == f;
        bool isNotEqual = i != f;
        bool isLessThan = i < f;
        bool isGreaterThan = i > f;
        bool isLessThanOrEqual = i <= f;
        bool isGreaterThanOrEqual = i <= f;

        Debug.Log(isEqual);
        Debug.Log(isNotEqual);
        Debug.Log(isLessThan);
        Debug.Log(isGreaterThan);
    }

}
