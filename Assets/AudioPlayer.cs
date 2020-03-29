using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{


    public AudioSource sound;
    private Rect audiorect;

    void Start()
    {
        audiorect = new Rect(20, 20, 1920, 1080);
    }


    void Update()
    {

    }

    void OnGUI()
    {
        
        if (audiorect.Contains(Event.current.mousePosition))
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {                
                sound.Play();
            }


        }
    }

}
