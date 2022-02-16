using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potato : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AkSoundEngine.PostEvent("Stasis_Pod_Hiss", gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
