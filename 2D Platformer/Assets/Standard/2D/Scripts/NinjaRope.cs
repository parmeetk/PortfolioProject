using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaRope : MonoBehaviour
{
    private PlatformerCharacter2D character;
    private DistanceJoint2D distanceJoint;
    private LineRenderer lineRenderer;

    private bool connected = false;

    private float minimumDistance = 0f;
    private float maximumDistance = 15f;
    private float normalizedDistance = 0.5f;
    private Ray ray;

    private Coroutine routine;
    private float ropeSpeedScalar = 5f;

    void Awake()
    {
        distanceJoint = GetComponent<DistanceJoint2D>();
        distanceJoint.enabled = false;
        lineRenderer = GetComponent<LineRenderer>();
        character = GetComponent<PlatformerCharacter2D>();
        lineRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update ()
    {
		if(Input.GetMouseButtonDown(0) && !character.IsGrounded)
        {
            if(!connected)
            {
                // if clicked on something
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 100f);
                if(hit)
                {
                    Debug.Log(hit.transform.gameObject.name);
                    RaycastHit2D hit2 = Physics2D.Raycast(transform.position, new Vector3(hit.point.x, hit.point.y, transform.position.z) - transform.position, maximumDistance, LayerMask.GetMask("Default", "Walls"));
                    if (hit2)
                    {
                        if (hit2.transform.gameObject == hit.transform.gameObject)
                        {
                            TriggerGrapple(hit.point);
                        }
                    }
                }
            }
        } else if(Input.GetMouseButtonUp(0) || (routine == null && connected && character.IsGrounded))
        {
            TriggerRelease();
        }

        if(connected)
        {
            lineRenderer.SetPosition(1, this.transform.position);
            lineRenderer.textureMode = LineTextureMode.Tile;

            if (Input.GetAxis("Vertical") >= 0.05f) 
                normalizedDistance -= Time.fixedDeltaTime;
            else if (Input.GetAxis("Vertical") <= -0.05f) 
                normalizedDistance += Time.fixedDeltaTime;
            normalizedDistance = Mathf.Clamp01(normalizedDistance);
            distanceJoint.distance = Mathf.Lerp(minimumDistance, maximumDistance, normalizedDistance);
        }
	}

    void TriggerGrapple(Vector2 pos)
    {
        Debug.Log("Triggered");
        connected = true;
        distanceJoint.enabled = true;
        distanceJoint.connectedAnchor = pos;
        distanceJoint.distance = Vector2.Distance(this.transform.position, pos);
        normalizedDistance = distanceJoint.distance / maximumDistance;
        //Debug.Log(distanceJoint.distance);
        lineRenderer.enabled = true;
        // lineRenderer.SetPosition(0, pos);

        lineRenderer.SetPosition(1, transform.position);
        // GetComponent<Rigidbody2D>().velocity *= 1.5f;
        // Character.AttachGrapple();
        if (routine != null)
            StopCoroutine(routine);
        routine = StartCoroutine(GrappleRoutine(pos));
    }

    private IEnumerator GrappleRoutine(Vector2 pos)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * ropeSpeedScalar;
            lineRenderer.SetPosition(0, Vector2.Lerp(transform.position, pos, t));
            yield return new WaitForEndOfFrame();
        }
        routine = null;
    }

    void TriggerRelease()
    {
        // Debug.Log((GetComponent<Rigidbody2D>().velocity);
        if (routine != null)
            StopCoroutine(routine);
        routine = StartCoroutine(ReleaseRoutine());
        // Character.DetatchGrapple();
        //GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0, 5), ForceMode2D.Impulse);
    }

    private IEnumerator ReleaseRoutine()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * ropeSpeedScalar;
            lineRenderer.SetPosition(0, Vector2.Lerp(lineRenderer.GetPosition(0), transform.position, t));
            yield return new WaitForEndOfFrame();
        }
        connected = false;
        distanceJoint.enabled = false;
        lineRenderer.enabled = false;
        routine = null;
    }
}
