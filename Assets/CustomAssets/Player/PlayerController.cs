using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public PhotonView photonView;
    public Rigidbody rb;
    public Animator animator;
    public Transform body;
    public Collider bodyCollider;
    public PickupSpawner rainEmitter;
    public ClanManager clanManager;
    [Space]

    public TMP_Text rainCount;
    public TMP_Text lifeCount;
    public Image healthBar;
    [Space]
    public SpriteRenderer miniMapIndicator;

    [Space]
    public List<GameObject> ownedPlayerObjects = new List<GameObject>();
    public List<GameObject> digObjects = new List<GameObject>();
    public List<GameObject> smashObjects = new List<GameObject>();
    public List<GameObject> inviteObjects = new List<GameObject>();
    [Space]
    public GameObject farCam;
    public GameObject closeCam;
    [Space]
    public GameObject rainlossEffect;
    public GameObject zapEffect;
    [Space]
    public float runSpeed = 20.0f;
    [Space]
    public GameObject dumpster;
    [Space]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth = 100;
    [SerializeField] private int regenerateSpeed = 20;
    [SerializeField] private bool isInvincible = false;

    float horizontal;
    float vertical;

    bool canMove = true;

    //joinData
    Coroutine joinRoutineSave = null;
    string requestID = "";

    private void Start() {
        if (photonView.IsMine)
        {
            miniMapIndicator.color = new Color(0, 255, 0);
            healthBar.color = new Color(0, 255, 0);
        }
        else
        {
            miniMapIndicator.color = new Color(255, 0, 0);
            healthBar.color = new Color(255, 0, 0);
        }

        foreach (GameObject go in ownedPlayerObjects) go.SetActive(!Game.instance.online || photonView.AmOwner);
    }

    void Update() {
        if (!Game.instance.online || photonView.AmOwner) {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");

            if (Input.GetMouseButton(0)) {
                StartCoroutine(SmashRoutine());
            }

            if (canMove && Input.GetMouseButton(1) && GetCurrentTileWet()) {
                StartCoroutine(DigRoutine());
            }

            if (canMove && Input.GetKey(KeyCode.I)) {
                StartCoroutine(InviteRoutine());
            }

            if (Input.GetKeyDown(KeyCode.Q)) {
                farCam.SetActive(true);
                closeCam.SetActive(false);
            }

            if (Input.GetKeyUp(KeyCode.Q)) {
                farCam.SetActive(false);
                closeCam.SetActive(true);
            }

            if (Input.GetKeyUp(KeyCode.R))
            {
                dumpster.GetComponent<DumpsterController>().photonView.RPC("RPCTeleport", dumpster.GetComponent<DumpsterController>().photonView.Owner, this.transform.position);
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

    public GroundTile GetCurrentTile() {
        GroundTile tile = GroundGrid.instance.getTileAt(new Vector2(transform.position.x, transform.position.z));
        return tile;
    }

    public void ToggleInvincible(bool _target)
    {
        photonView.RPC("RPCToggleInvincible", RpcTarget.AllBuffered, _target);
    }


    #region ACTIONS

    IEnumerator SmashRoutine() {
        canMove = false;
        foreach (GameObject go in smashObjects) go.SetActive(true);
        animator.SetBool("Smashing", true);
        yield return new WaitForSeconds(0.367f);
        animator.SetBool("Smashing", false);
        foreach (GameObject go in smashObjects) go.SetActive(false);
        canMove = true;
    }

    IEnumerator DigRoutine() {
        canMove = false;
        foreach (GameObject go in digObjects) go.SetActive(true);
        animator.SetBool("Digging", true);
        yield return new WaitForSeconds(2f);
        animator.SetBool("Digging", false);
        foreach (GameObject go in digObjects) go.SetActive(false);
        canMove = true;
        GetCurrentTile().spawnTrash(6);
    }

    IEnumerator InviteRoutine() {
        foreach (GameObject go in inviteObjects) go.SetActive(true);
        yield return new WaitForSeconds(2f);
        foreach (GameObject go in inviteObjects) go.SetActive(false);
    }

    #endregion


    #region EFFECTS

    [PunRPC]
    public void RPCAddHealth(int _health)
    {
        currentHealth = Mathf.Clamp(currentHealth + _health, 0, maxHealth);
        healthBar.fillAmount = (float)currentHealth / (float)maxHealth;
    }

    [PunRPC]
    public void RPCRemoveHealth(int _health)
    {
        currentHealth = Mathf.Clamp(currentHealth - _health, 0, maxHealth);

        if (currentHealth <= 0)
        {
            
        }

        healthBar.fillAmount = (float)currentHealth / (float)maxHealth;
    }

    [PunRPC]
    public void RPCToggleInvincible(bool _target)
    {
        isInvincible = _target;
    }

    [PunRPC]
    public void RPCTeleport(Vector3 _position)
    {
        this.transform.position = _position;
    }

    //owner
    [PunRPC]
    public void RPCGetHit(Photon.Realtime.Player dealer) {
        if (!isInvincible)
        {
            //photonView.RPC("showRainLoss", RpcTarget.AllBuffered);
            int loss = Game.instance.RemoveRain(Game.instance.rainTransferOnHit);
            rainEmitter.spawn(loss);
            //photonView.RPC("awardFromHit", dealer, loss);

            photonView.RPC("RPCRemoveHealth", RpcTarget.AllBuffered, Game.instance.playerDamage);
        }
    }

    //owner
    [PunRPC]
    public void RPCGetZapped() {
        photonView.RPC("RPCShowRainLoss", RpcTarget.AllBuffered);
        photonView.RPC("RPCShowZapping", RpcTarget.AllBuffered);
        Game.instance.RemoveAllRain();
        Game.instance.RemoveLife();
    }

    //owner
    [PunRPC]
    public void RPCRewardRain(int amount) {
        Game.instance.AddRain(amount);
        //foreach (PlayerController player in FindObjectsOfType<PlayerController>()) { //Should be okay since it will be handled localy by client and will not be repeated too often
        //    if (player != this) {
        //        player.photonView.RPC("rewardClanRain", player.photonView.Owner, amount, clanManager.getClan());
        //    }
        //}
    }
    
    //owner
    [PunRPC]
    public void RPCRewardClanRain(int amount, string clan) {
        if(clanManager.GetClan() == clan) Game.instance.AddRain(amount);
    }


    #endregion


    #region VISUALS

    //all
    [PunRPC]
    public void RPCUpdateCharacterUI(int raindrops, int lives, string clanID) {
        rainCount.text = raindrops.ToString();
        lifeCount.text = lives.ToString();
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = ColorFromID(clanID);
        body.GetComponentInChildren<MeshRenderer>().sharedMaterial = mat;
    }

    //all
    [PunRPC]
    public void RPCShowRainLoss() {
        rainlossEffect.SetActive(false);
        rainlossEffect.SetActive(true);
    }

    //all
    [PunRPC]
    public void RPCShowZapping() {
        zapEffect.SetActive(false);
        zapEffect.SetActive(true);
    }


    public Color ColorFromID(string ID) {
        if (ID == null) return Color.white;
        Random.InitState(ID.GetHashCode());
        return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }


    #endregion


    




}
