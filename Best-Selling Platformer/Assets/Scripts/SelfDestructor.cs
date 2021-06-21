using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestructor : MonoBehaviour
{
    public void SelfDestruct()
    {
        Destroy(gameObject);
    }
}
