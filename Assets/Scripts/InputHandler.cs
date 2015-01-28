using UnityEngine;
using System.Collections;

public class InputHandler : MonoBehaviour 
{
    public delegate void BallFiredEventHandler(Quaternion rot);
    public static event BallFiredEventHandler BallFiredEvent = delegate{};  

    public GameObject LineIndicator;

    private bool _isFingerDown = false;

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            _isFingerDown = true;
            LineIndicator.SetActive(true);
        }

        if(_isFingerDown)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            LineIndicator.transform.rotation = Quaternion.LookRotation(Vector3.forward, mousePos - LineIndicator.transform.position) * Quaternion.Euler(0f, 0f, 270f);
        }

        if(Input.GetMouseButtonUp(0))
        {
            _isFingerDown = false;
            LineIndicator.SetActive(false);

            //TODO Fire Ball
            BallFiredEvent(LineIndicator.transform.rotation);
        }
    }
}
