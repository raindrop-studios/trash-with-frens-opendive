using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour{

    public PhotonView photonView;
    public Rigidbody rb;
    public Animator animator;
    public Transform body;
    [Space]
    public List<GameObject> smashObjects = new List<GameObject>();
    [Space]
    public GameObject rainlossEffect;
    [Space]
    public GameObject target = null;
    [Space]
    public float runSpeed = 20.0f;
    public int life = 3;
    public float attackDistance = 0.05f;
    public float deagroDistance = 0.4f;

    float horizontal;
    float vertical;

    bool canMove = true;

    private void Update() {
        horizontal = vertical = 0f;
        if (!Game.instance.online || photonView.AmOwner) {
            if (target != null) {
                float distance = Vector3.Distance(transform.position, target.transform.position);

                if (distance > deagroDistance) {
                    target = null;
                } else {

                    horizontal = transform.position.x - target.transform.position.x < 0 ? 1 : -1;
                    vertical = transform.position.z - target.transform.position.z < 0 ? 1 : -1;
                    if (canMove && distance < attackDistance) StartCoroutine(SmashRoutine());

                }
            }
        }
    }

    private void FixedUpdate() {
        if (canMove) rb.velocity = new Vector3(horizontal * runSpeed, 0, vertical * runSpeed);
        else rb.velocity = Vector3.zero;
        if (rb.velocity.magnitude > 0) body.rotation = Quaternion.LookRotation(rb.velocity);
        animator.SetBool("Running", rb.velocity.magnitude > 0);
    }

    public bool GetCurrentTileRain() {
        GroundTile tile = GroundGrid.instance.getTileAt(new Vector2(transform.position.x, transform.position.z));
        if (tile != null && tile.isRaining) return true;
        else return false;
    }

    public bool GetCurrentTileWet() {
        GroundTile tile = GroundGrid.instance.getTileAt(new Vector2(transform.position.x, transform.position.z));
        if (tile != null && tile.isWet) return true;
        else return false;
    }


    //owner
    [PunRPC]
    public void RPCGetHit(Photon.Realtime.Player dealer) {
        photonView.RPC("RPCShowRainLoss", RpcTarget.AllBuffered);
        life--;
        if(life == 0) photonView.RPC("RPCDie", RpcTarget.AllBuffered);
    }

    //all
    [PunRPC]
    public void RPCShowRainLoss() {
        rainlossEffect.SetActive(false);
        rainlossEffect.SetActive(true);
    }

    //all
    [PunRPC]
    public void RPCDie() {
        PhotonNetwork.Destroy(gameObject);
    }


    IEnumerator SmashRoutine() {
        canMove = false;
        foreach (GameObject go in smashObjects) go.SetActive(true);
        animator.SetBool("Smashing", true);
        yield return new WaitForSeconds(0.367f);
        animator.SetBool("Smashing", false);
        foreach (GameObject go in smashObjects) go.SetActive(false);
        canMove = true;
    }

}
