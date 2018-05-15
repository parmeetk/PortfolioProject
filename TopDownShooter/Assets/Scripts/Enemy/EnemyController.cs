using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
	public Transform Player;
	NavMeshAgent agent;

	// Float value that I can edit in the inspector window for the enemy movement speed
	//public float moveSpeed;

	void Start()
	{
		agent = GetComponent();
	}


	void Update()
	{
		// Makes the enemy look at the Player transform
		transform.LookAt(Player);

		

		// Movement for the enemy that takes a float from the inspector window. will be changed after better numbers.
		//transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);	
	}

}

// https://unity3d.com/learn/tutorials/topics/navigation/nav-meshes