using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	public GameObject bulletPrefab;
	public Transform bulletSpawn;
	//public GameObject zombie;

	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			Fire();
		}
	}

	void Fire()
	{
		// Create the Bullet from the Bullet Prefab
		var bullet = (GameObject)Instantiate(
			bulletPrefab,
			bulletSpawn.position,
			bulletSpawn.rotation);

		// Add velocity to the bullet
		bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 10;

		// Destroy the bullet after 2 seconds
		Destroy(bullet, 2.0f);
	}

	// Setting up bullet to do 10 damage when the bullet collides with enemy
	void OnCollisionEnter(Collision collision)
	{
		var hit = collision.gameObject;
		//var health = hit.GetComponent<EnemyController>(); // Need help
		//if (health != null)
		//{
		//	//health.TakeDamage(10);
		//}

		Destroy(gameObject);
	}
}
