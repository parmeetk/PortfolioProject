using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

	public GameObject[] enemies;
	public Vector3 spawnValues;
	public float spawnWait;
	public float spawnMostWait;
	public float spawnLeastWait;
	public int startWait;
	public bool stop;
	public int enemiesMax;
	public int enemiesSpawned = 0;

	int randEnemy;

	// Use this for initialization
	void Start ()
	{
		StartCoroutine(EnemySpawner());
	}
	
	// Update is called once per frame
	void Update ()
	{
		spawnWait = Random.Range(spawnLeastWait, spawnMostWait);

		if (enemiesMax <= enemiesSpawned)
		{
			stop = true;
		}
		else
		{
			stop = false;
		}
	}

	IEnumerator EnemySpawner()
	{
		yield return new WaitForSeconds(startWait);

		while(!stop)
		{
			randEnemy = Random.Range(0, 1);

			Vector3 spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x),  1, Random.Range(-spawnValues.z, spawnValues.z));

			Instantiate(enemies[randEnemy], spawnPosition + transform.TransformPoint(0, 0, 0), gameObject.transform.rotation);

			yield return new WaitForSeconds(spawnWait);
		}

	}
}
