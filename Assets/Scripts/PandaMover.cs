using UnityEngine;
using System.Collections;

public class PandaMover : MonoBehaviour 
{
    public float MoveSpeed = 1f;

    private Vector2 _newPos = Vector2.zero;

    //private bool _collecting = false;
    private bool _ballCollected = false;

    void Awake()
    {
        Ball.BallLandedEvent += HandleBallLandedEvent;
        Ball.BallCollectedEvent += HandleBallCollectedEvent;
        InputHandler.BallFiredEvent += HandleBallFiredEvent;
    }

    void HandleBallFiredEvent (Quaternion rot)
    {
        _ballCollected = false;
    }

    void HandleBallCollectedEvent (Vector2 pos)
    {
        _ballCollected = true;
    }

    void HandleBallLandedEvent (Vector2 pos)
    {
        _newPos = pos;
    }

    void FixedUpdate()
    {
       
        if(_ballCollected)
        {
            rigidbody2D.velocity = Vector2.zero;
        }

        else
        {
            rigidbody2D.AddForce(new Vector2((MoveSpeed * (_newPos.x - transform.position.x)) - (rigidbody2D.velocity.x * 1.75f),0f), ForceMode2D.Force);
        }

    }

    void OnDestroy()
    {
        Ball.BallLandedEvent -= HandleBallLandedEvent;
        Ball.BallCollectedEvent -= HandleBallCollectedEvent;
        InputHandler.BallFiredEvent -= HandleBallFiredEvent;
    }
}
