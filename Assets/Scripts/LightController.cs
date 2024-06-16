using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    Light candleLight;
    int currentColor = 1;

    // Start is called before the first frame update
    void Start()
    {
        candleLight = GetComponentInChildren<Light>();
        candleLight.color = Color.green;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            candleLight.enabled = !candleLight.enabled;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentColor < 3)
            {
                currentColor+= 1;
            }
            else
            {
                currentColor = 1;
            }

            switch(currentColor)
            {
                case 1:
                    candleLight.color = Color.green;
                    break;
                
                case 2:
                    candleLight.color = Color.blue;
                    break;
                
                case 3:
                    candleLight.color = Color.red;
                    break;
            }
        }
    }
}
