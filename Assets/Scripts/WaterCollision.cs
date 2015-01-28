using UnityEngine;
using System.Collections;

public class WaterCollision : MonoBehaviour 
{
    void OnTriggerEnter2D(Collider2D hit)
    {
        if (hit.rigidbody2D != null && transform.parent != null)
        {
            if(hit.transform.tag == "Brick")
            {
                Water water = transform.parent.GetComponent<Water>();

                if(water != null)
                {
                    water.Splashes(transform.position.x, hit.rigidbody2D.velocity.y * hit.rigidbody2D.mass / 55f);
                }
            }

            if(hit.transform.tag == "Ball")
            {
                hit.gameObject.GetComponent<Ball>().HitWater();
            }
        }


    }

    void OnTriggerStay2D(Collider2D hit)
    {
        if (hit.rigidbody2D != null &&
            (hit.transform.tag == "Panda" || (hit.transform.tag == "Ball" && !hit.rigidbody2D.isKinematic)))
        {
            if(hit.transform.position.y < -5.73f && hit.transform.position.y > -100f)
            {
                hit.rigidbody2D.AddForce(new Vector2(0f, (10f * (-5.25f - hit.transform.position.y)) ),ForceMode2D.Force); //- (hit.rigidbody2D.velocity.y * 1.75f)
            }

            else
            {
                hit.rigidbody2D.velocity = Vector2.zero;
            }
        }
    }
    
}
