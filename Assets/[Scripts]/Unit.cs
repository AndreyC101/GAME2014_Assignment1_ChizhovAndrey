using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Unit : MonoBehaviour, IDamageable
{
    public UnitType m_type;

    [SerializeField]
    private Animator m_animator;

    [SerializeField]
    private int m_maxHealth = 100;

    [SerializeField]
    private float m_attackRange = 1.0f;

    [SerializeField]
    private int m_attackDamage = 15;

    [SerializeField]
    private float m_attackDelay = 1.0f;

    [SerializeField]
    private float m_moveSpeed = 1.0f;

    [SerializeField]
    private GameObject m_healthBar;

    [SerializeField]
    private AudioSource m_audio;

    public int m_currentHealth;

    public bool m_friendly;
    public bool dead;
    public float m_healthbarMaxWidth = 0.1f;

    public IDamageable currentTarget = null;
    private float timeSinceLastAttack = 0;
    private CapsuleCollider2D m_collider;
    private bool isMoving = false;
    void Start()
    {
        m_currentHealth = m_maxHealth;
        m_animator = GetComponent<Animator>();
        m_collider = GetComponent<CapsuleCollider2D>();
        m_audio = GetComponent<AudioSource>();
        m_healthBar = transform.Find("HealthBar").gameObject;
        m_healthBar.transform.localScale = new Vector3(m_healthbarMaxWidth, m_healthBar.transform.localScale.y, m_healthBar.transform.localScale.z);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (dead)
        {
            return;
        }
        isMoving = false;
        //check for enemy unit within attack range
        int layerMask = 1 << LayerMask.NameToLayer(m_friendly ? "Enemies" : "Friendlies");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, (m_friendly ? Vector3.right : Vector3.left), m_attackRange, layerMask);
        Debug.DrawLine(transform.position, (m_friendly ? transform.position + Vector3.right * m_attackRange : transform.position + Vector3.left * m_attackRange), GameProperties.Instance.textColors[(int)TextType.INVALID]);
        
        if (hit)
        {
            currentTarget = hit.transform.GetComponent<IDamageable>();
            Attack();
        }
        else
            Move();
        m_animator.SetBool("Moving", isMoving);
    }

    private void Move()
    {
        transform.position = new Vector3(transform.position.x + (m_friendly ? m_moveSpeed : -m_moveSpeed) * Time.fixedDeltaTime, transform.position.y, 0f);
        isMoving = true;
    }

    private void Attack()
    {
        m_animator.SetBool("Moving", false);
        timeSinceLastAttack += Time.deltaTime;
        if (currentTarget != null && timeSinceLastAttack > m_attackDelay)
        {
            timeSinceLastAttack = 0;
            m_animator.SetTrigger("Attack");
            m_audio.PlayOneShot(GameProperties.Instance.gunSFX[(int)m_type]);
            currentTarget.TakeDamage(m_attackDamage);
            currentTarget = null;
        }
    }

    public bool TakeDamage(int incomingDamage)
    {
        m_currentHealth -= incomingDamage;
        if (m_currentHealth > 0)
        {
            float fillPerc = (float)m_currentHealth / (float)m_maxHealth;
            m_healthBar.transform.localScale = new Vector3(m_healthbarMaxWidth * fillPerc, m_healthBar.transform.localScale.y, m_healthBar.transform.localScale.z);
            return false;
        }
        else
        {
            m_animator.SetTrigger("Death");
            HandleDestruction();
            return true;
        }
    }

    public void HandleDestruction()
    {
        if (!m_friendly)
        {
            FindObjectOfType<GameController>().m_playerFunds += GameProperties.Instance.unitCosts[(int)m_type];
        }
        dead = true;
        GetComponent<CapsuleCollider2D>().enabled = false;
        m_healthBar.SetActive(false);
        Invoke("ClearFromField", 3.0f);
    }

    public void ClearFromField()
    {
        Destroy(gameObject);
    }
}
