using UnityEngine;
using System.Collections;

public class Brick : MonoBehaviour 
{
    public bool IsUnbreakable = false;

    public void WasHit()
    {
        if (!IsUnbreakable)
        {
            StartCoroutine(fallDelay());
        }

    }

    private IEnumerator fallDelay()
    {
        //TODO Add hit effect
        yield return new WaitForSeconds(0.05f);
        rigidbody2D.isKinematic = false;
        //_myCollider.enabled = false;
    }

    void Update()
    {
        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }
}
