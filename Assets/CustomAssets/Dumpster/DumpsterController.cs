using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DumpsterController : MonoBehaviour
{
    public GameObject owner;
    [Space]
    public PickupSpawner rainEmitter;
    [Space]
    public GameObject rainlossEffect;
    [Space]
    public PhotonView photonView;
    public Rigidbody rb;
    public Transform body;

    [Header("GameObjects Reference")]
    public GameObject innerBase;
    public GameObject outterCase;
    public GameObject invincibleCase;
    public SpriteRenderer miniMapIndicator;

    [Header("UI Reference")]
    public GameObject doorIcon;
    public GameObject baseIndicator;
    public TMP_Text rainCount;
    public TMP_Text lifeCount;
    public Image healthBar;
    [Space]
    public GameObject upgradeMenuRoot;
    public Button closeMenuBtn;

    public Button upgradeBaseHealthBtn;
    public Button upgradePlayerAttckBtn;
    public Button upgradePlayerHealthBtn;
    public Button trahsferRainBtn;

    [Space]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth = 100;
    [SerializeField] private int regenerateSpeed = 20;
    [SerializeField] private bool nearBase = false;
    [SerializeField] private bool inBase = false;
    [SerializeField] private bool isInvincible = false;

    void Start()
    {
        if (photonView.IsMine)
        {
            baseIndicator.SetActive(true);
            miniMapIndicator.color = new Color(0, 255, 0);
            healthBar.color = new Color(0, 255, 0);
        }
        else
        {
            miniMapIndicator.color = new Color(255, 0, 0);
            healthBar.color = new Color(255, 0, 0);
        }

        closeMenuBtn.onClick.AddListener(delegate { OnCloseMenuClicked(); });
        upgradeBaseHealthBtn.onClick.AddListener(delegate { OnUpgradeBaseHealthClicked(); });
        upgradePlayerAttckBtn.onClick.AddListener(delegate { OnUpgradePlayerAttckClicked(); });
        upgradePlayerHealthBtn.onClick.AddListener(delegate { OnUpgradePlayerHealthClicked(); });
        trahsferRainBtn.onClick.AddListener(delegate { OnTransferRainClicked(); });

        StartCoroutine(Regenerate());
    }

    // Update is called once per frame
    void Update()
    {
        ControllerCheck();
    }

    void ControllerCheck()
    {
        if (Input.GetKeyDown(KeyCode.E) && nearBase)
        {
            owner.GetComponent<PlayerController>().photonView.RPC("RPCTeleport", owner.GetComponent<PlayerController>().photonView.Owner, this.transform.position);
            photonView.RPC("RPCToggleInvincible", RpcTarget.AllBuffered, true);
            owner.GetComponent<PlayerController>().ToggleInvincible(true);
            doorIcon.SetActive(false);
            nearBase = false;
            inBase = true;
        }
        else if(Input.GetKeyDown(KeyCode.P) && inBase)
        {
            upgradeMenuRoot.SetActive(true);
        }
    }

    public void SetOwner(GameObject _target)
    {
        owner = _target;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == owner)
        {
            doorIcon.SetActive(true);
            nearBase = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == owner)
        {
            doorIcon.SetActive(false);
            upgradeMenuRoot.SetActive(false);
            photonView.RPC("RPCToggleInvincible", RpcTarget.AllBuffered, false);
            owner.GetComponent<PlayerController>().ToggleInvincible(false);

            nearBase = false;
            inBase = false;
        }
    }

    #region RPC

    [PunRPC]
    public void RPCTeleport(Vector3 _position)
    {
        this.transform.position = _position;
    }

    [PunRPC]
    public void RPCToggleInvincible(bool _target)
    {
        invincibleCase.SetActive(_target);
        isInvincible = _target;
    }

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
            photonView.RPC("RPCShowRainLoss", RpcTarget.AllBuffered);
            int loss = Game.instance.RemoveDumpsterRain(Game.instance.rainTransferOnHit);
            rainEmitter.spawn(loss);

            currentHealth = maxHealth;
        }

        healthBar.fillAmount = (float)currentHealth / (float)maxHealth;
    }

    //owner
    [PunRPC]
    public void RPCGetHit(Photon.Realtime.Player dealer)
    {
        if (!isInvincible)
        {
            //photonView.RPC("showRainLoss", RpcTarget.AllBuffered);
            //int loss = Game.instance.RemoveDumpsterRain(Game.instance.rainTransferOnHit);
            //rainEmitter.spawn(loss);

            photonView.RPC("RPCRemoveHealth", RpcTarget.AllBuffered, Game.instance.playerDamage);
        }
    }

    [PunRPC]
    public void RPCShowRainLoss()
    {
        rainlossEffect.SetActive(false);
        rainlossEffect.SetActive(true);
    }

    [PunRPC]
    public void RPCUpdateDumpsterUI(int raindrops, int lives, string clanID)
    {
        rainCount.text = raindrops.ToString();
        lifeCount.text = lives.ToString();
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = ColorFromID(clanID);
        outterCase.GetComponentInChildren<MeshRenderer>().sharedMaterial = mat;
    }

    #endregion

    public Color ColorFromID(string ID)
    {
        if (ID == null) return Color.white;
        Random.InitState(ID.GetHashCode());
        return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }

    IEnumerator Regenerate()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            photonView.RPC("RPCAddHealth", RpcTarget.AllBuffered, regenerateSpeed);
        }
    }

    #region Function Callback

    private void OnCloseMenuClicked()
    {
        upgradeMenuRoot.SetActive(false);
    }

    private void OnUpgradeBaseHealthClicked()
    {
        
    }

    private void OnUpgradePlayerAttckClicked()
    {

    }

    private void OnUpgradePlayerHealthClicked()
    {

    }

    private void OnTransferRainClicked()
    {
        int _rain = Game.instance.playerRaindrops;
        Game.instance.RemoveRain(_rain);
        Game.instance.AddDumpsterRain(_rain);
    }
    #endregion
}
