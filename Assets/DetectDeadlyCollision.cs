using UnityEngine;
using System.Collections;

public class DetectDeadlyCollision : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            GameObject.Find("Engine").GetComponent<SceneEngine>().GameOver();
        }
    }
}