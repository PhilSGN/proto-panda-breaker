using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour 
{
    public delegate void BallEventHandler(Vector2 pos);
    public static event BallEventHandler BallLandedEvent = delegate{};  
    public static event BallEventHandler BallCollectedEvent = delegate{};

    private bool _inWater = false;

    public float FireSpeed = 20f;

    void Awake()
    {
        InputHandler.BallFiredEvent += HandleBallFiredEvent;
    }

    void Update()
    {
        if(!_inWater)
        {
            rigidbody2D.velocity = FireSpeed * (rigidbody2D.velocity.normalized);
        }
    }

    void HandleBallFiredEvent (Quaternion rot)
    {
        transform.rotation = rot;

        rigidbody2D.isKinematic = false;

        rigidbody2D.AddRelativeForce(-(Vector2.right * FireSpeed), ForceMode2D.Impulse);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.tag == "Brick")
        {
            other.gameObject.GetComponent<Brick>().WasHit();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Collector")
        {
            rigidbody2D.isKinematic = true;
            rigidbody2D.gravityScale = 0;
            rigidbody2D.velocity = Vector2.zero;

            transform.position = other.transform.position + Vector3.up;

            BallCollectedEvent(transform.position);

            _inWater = false;
        }
    }

    public void HitWater()
    {
        if(!_inWater)
        {
            _inWater = true;

            rigidbody2D.isKinematic = true;

            //Vector2 currentVel = rigidbody2D.velocity;

            //currentVel.x = 0f;

            rigidbody2D.velocity = Vector2.zero; //currentVel;

            BallLandedEvent(transform.position);
        }
    }

    void OnDestroy()
    {
        InputHandler.BallFiredEvent -= HandleBallFiredEvent;
    }
}
