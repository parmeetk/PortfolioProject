using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollision : MonoBehaviour
{
    private ParticleSystem part;
    private List<ParticleCollisionEvent> collisionEvents;
    public GameObject fxPrefab;
    public float explosionRadius = 3f;
    public float explosionMaxForce = 1000f;

    private Vector3 lastExplosion = Vector3.zero;
    void Start()
    {
        part = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
    }

    private void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        int i = 0;

        while (i < numCollisionEvents)
        {
            if (rb)
            {
                //Debug.Log("!");
                Vector3 pos = collisionEvents[i].intersection;
                Vector3 force = collisionEvents[i].velocity * 10;
                rb.AddForce(force);
            }

            RaycastHit2D[] affectedObjects = Physics2D.CircleCastAll(collisionEvents[i].intersection, explosionRadius, Vector2.zero);
            foreach (RaycastHit2D hit in affectedObjects)
            {
                rb = hit.transform.GetComponent<Rigidbody2D>();
                if (rb)
                {
                    Vector2 f0 = (rb.transform.position - collisionEvents[i].intersection);
                    rb.AddForce(f0.normalized * Remap(f0.magnitude, 0f, explosionRadius, explosionMaxForce, 0f), ForceMode2D.Impulse);
                }
            }

            if (fxPrefab)
                Instantiate(fxPrefab, collisionEvents[i].intersection, Quaternion.identity);

            i++;
        }
    }

    public static float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}