using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
	public const int maxHealth = 100;
	public int currentHealth = maxHealth;

	void Update()
	{
		var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
		var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

		transform.Rotate(0, x, 0);
		transform.Translate(0, 0, z);
	}


	public void TakeDamage(int amount)
	{
		currentHealth -= amount;
		if (currentHealth <= 0)
		{
			currentHealth = 0;
			Debug.Log("Dead!");
		}
	}
}

// https://unity3d.com/learn/tutorials/topics/multiplayer-networking/player-health-single-player
// ^ includes health UI.