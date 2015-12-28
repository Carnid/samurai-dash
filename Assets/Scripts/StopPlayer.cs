using UnityEngine;
using System.Collections;

public class StopPlayer : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Rigidbody2D>().gravityScale = 5;
            JumpController jumpCtrl = other.gameObject.GetComponent<JumpController>();
            jumpCtrl.StopDash();
        }
    }
    
}
