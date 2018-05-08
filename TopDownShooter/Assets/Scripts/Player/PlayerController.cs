using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
	// Sets health to be 100 at the start of each round for the player
	public const int maxHealth = 100;
	
	// Setting currentHealth = maxHealth means that the player wont be able to get back health. The amount of health that the player has during the game after they have taken a hit to their health is the max they will have after it and so forth after each hit.
	public int currentHealth = maxHealth;

	void Update()
	{ 
		// Movement
		var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
		var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

		transform.Rotate(0, x, 0);
		transform.Translate(0, 0, z);
	}

	// Code to implement health
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