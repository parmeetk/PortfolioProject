using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform Player;
    public Transform myPlayer;

    void Update()
    {
        transform.LookAt(Player);
        transform.Translate(Vector3.forward * 2.5f * Time.deltaTime);
    }
}
