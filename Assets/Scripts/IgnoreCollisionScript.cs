using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollisionScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("TryIgnoreCollision");
    }
    IEnumerator TryIgnoreCollision()
    {
        while (true)
        {
            if (transform.GetComponentInChildren<PolygonCollider2D>() != null)
            {
                //Debug.Log(nin.transform.GetComponentInChildren<CircleCollider2D>().OverlapCollider(filter, results) + " " + nin.transform.position.x + " "+ nin.transform.position.y);
                Physics2D.IgnoreCollision(transform.GetComponentInChildren<PolygonCollider2D>(), GameObject.Find("Character").GetComponent<Collider2D>());
                Physics2D.IgnoreCollision(transform.GetComponentInChildren<PolygonCollider2D>(), GameObject.Find("Character").transform.Find("Player Collider").GetComponent<Collider2D>());
                Physics2D.IgnoreCollision(transform.Find("Player Collider").GetComponent<BoxCollider2D>(), GameObject.Find("Character").GetComponent<Collider2D>());

                if (transform.GetComponentInChildren<PolygonCollider2D>().OverlapCollider(OverworldGeneration.instance.villageFilter, OverworldGeneration.instance.results) > 0)
                    Destroy(transform.gameObject);

                Destroy(this);
            }
            yield return new WaitForSeconds(1);
        }
    }
}
