using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgroZone : MonoBehaviour{

    public BossController controller;

    private void OnTriggerStay(Collider other) {
        if (other.tag == "Player") {
            if (controller.target == null) controller.target = other.gameObject;
            else if (Vector3.Distance(transform.position, other.transform.position) < Vector3.Distance(transform.position, controller.target.transform.position)) controller.target = other.gameObject;
        }
    }
}
