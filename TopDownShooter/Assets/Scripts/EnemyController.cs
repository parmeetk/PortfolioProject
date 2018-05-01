using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
	public GameObject playerObject;
	public Transform Player;
	public float moveSpeed;

	void Update()
	{
		transform.LookAt(Player);
		transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

		
	}
}