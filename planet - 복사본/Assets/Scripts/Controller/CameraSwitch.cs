using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{

    public Camera cameraOne;
    public Camera cameraTwo;


    //AudioListener cameraOneAudioLis;
    //AudioListener cameraTwoAudioLis;

    // Start is called before the first frame update
    void Start()
    {
        //cameraOneAudioLis = cameraOne.GetComponent<AudioListener>();
        //cameraTwoAudioLis = cameraTwo.GetComponent<AudioListener>();

        cameraOne.enabled = true;
       // cameraOneAudioLis.enabled = true;
        cameraTwo.enabled = false;
       // cameraTwoAudioLis.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
        {
            CameraChange();
        }
    }

    void CameraChange()
    {
        cameraOne.enabled = !cameraOne.enabled;
        //cameraOneAudioLis.enabled = !cameraOneAudioLis.enabled;
        cameraTwo.enabled = !cameraTwo.enabled;
        //cameraTwoAudioLis.enabled = !cameraTwoAudioLis.enabled;
    }


}
