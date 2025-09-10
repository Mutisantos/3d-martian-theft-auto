using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/** Define mediante Delegates los diferentes estados de un Enemigo en el juego, 
 * permitiendole parametrizar vida, velocidad, daño,  cadencia de ataque, rango y rutina de seguimiento
 * Esteban.Hernandez
 */
public class EnemyIA : MonoBehaviour
{
    public float Health = 100;
    public float Speed = 30;
    public float Damage = 10;
    public int SpeedMultiplier = 3;
    public float StateEvaluationFrequency = 0.05f; // 1/20 de segundo, el ciclo de ejecución de los estados.
    public float ProximityThreshold = 0.02f; //Margen de proximidad a un punto de destino 
    public float RandomPointRadius = 10f; //Radio de evaluación de puntos aleatorios
    public float PursuitRadius = 8f; //Radio de Visión
    public float AttackRange = 2f; //Radio de Ataque
    public float AttackRate = 0.5f; //Frecuencia de ataques

    public AudioClip DamageClip;
    public AudioClip AttackClip;
    public AudioClip SplatClip;
    public GameObject ExplosionPrefab;

    [SerializeField]
    private GameObject _pursuitReference;
    //Delegar la definicion de corrutinas sin parámetros mediante State()
    delegate IEnumerator State();
    private State _state;
    private NavMeshAgent _agent;
    private float _timeBetweenAttacks;
    private AudioSource _enemySoundSource;
    private ParticleSystem _explosionParticles;
    private Animator _animator;
    private CapsuleCollider _collider;
    private Rigidbody _rigidbody;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _collider = GetComponent<CapsuleCollider>();
        _animator = GetComponentInChildren<Animator>();
        _enemySoundSource = GetComponent<AudioSource>();
        _enemySoundSource = GetComponent<AudioSource>();
        _explosionParticles = Instantiate(ExplosionPrefab).GetComponent<ParticleSystem>();
        _explosionParticles.gameObject.SetActive(false);
        _timeBetweenAttacks = 0f;
        _agent.speed = Speed;
        _animator.SetBool("IsAlive", true);
        _rigidbody = GetComponent<Rigidbody>();
    }

    IEnumerator Start()
    {

        _state = WanderState;
        while (enabled)
        {
            yield return StartCoroutine(_state());
            //Debug.Log(_state.Method);
        }
    }

    //Rutina de patrulla, moviendose entre waypoints
    IEnumerator WanderState()
    {
        _agent.isStopped = false;
        _agent.speed = Speed;
        _animator.SetFloat("Speed", Speed);
        _animator.SetBool("IsAttacking", false);
        if (_agent.remainingDistance < ProximityThreshold)
        {
            Vector3 point;
            if (RandomPoint(transform.position, RandomPointRadius, out point))
            {
                Debug.DrawRay(point, Vector3.up, Color.cyan, 11.0f);
                _agent.destination = point;
            }
        };
        CheckNearbyVictims(this.AttackRange);
        yield return StateEvaluationFrequency;
    }


    //Rutina de alerta al detectar al jugador, desde ese instante empezará a perseguirlo
    IEnumerator FollowState()
    {
        //Mientras el objetivo esté en el rango de persecusión del enemigo
        if (_pursuitReference != null && IsTargetInRange(PursuitRadius))
        {
            _agent.destination = _pursuitReference.transform.position;
            _agent.speed = Speed * SpeedMultiplier;
            _animator.SetFloat("Speed", Speed * SpeedMultiplier);
            if (IsTargetInRange(AttackRange))
            {
                _state = AttackState;
                _animator.SetBool("IsAttacking", true);
            }
        }
        else
        {
            _state = WanderState;
        }
        yield return StateEvaluationFrequency;
    }

    //Hacer que el enemigo no efectue más acciones una vez haya muerto
    IEnumerator DeadState()
    {
        yield return StateEvaluationFrequency;

    }

    //Rutina cuando el jugador está en el rango de ataque del jugador
    IEnumerator AttackState()
    {
        _agent.velocity = Vector3.zero;
        _agent.isStopped = true;
        _timeBetweenAttacks += Time.deltaTime;

        //Si ya no hay objetivo de ataque, volver a deambular
        if (_pursuitReference == null)
        {
            //Si no hay objetivo al que seguir o atacar, vuelve a deambular
            _state = WanderState;
        }
        //Si el objetivo no está en rango de ataque, debe seguir persiguiendo
        else if (_pursuitReference != null && !IsTargetInRange(AttackRange))
        {
            _state = FollowState;
        }
        if (_timeBetweenAttacks > AttackRate)
        {
            _timeBetweenAttacks = 0;
            _pursuitReference.SendMessage("ReceiveHit", Damage);
            _enemySoundSource.PlayOneShot(AttackClip);
            _animator.SetFloat("Speed", 0);
        }
        yield return StateEvaluationFrequency;
    }

    //Metodo para ser invocado cada que el enemigo sufra un disparo.
    public void Hit(float damage, Vector3 source)
    {
        _animator.SetFloat("Speed", 0);
        _animator.SetTrigger("IsDamaged");
        _animator.Play("Hurt");
        _enemySoundSource.PlayOneShot(DamageClip);
        Health -= damage;
        if (Health <= 0)
        {
            ProcessDeath();
        }
        else
        {
            transform.LookAt(source);
            _agent.isStopped = true;
        }
    }

    protected void ProcessDeath()
    {
        _state = DeadState;
        _enemySoundSource.Play();
        _animator.SetBool("IsAlive", false);
        _agent.speed = 0;
        _collider.enabled = false;
        _enemySoundSource.PlayOneShot(DamageClip);
        Destroy(this.gameObject, 2);
        _explosionParticles.transform.position = transform.position;
        _explosionParticles.gameObject.SetActive(true);
        _explosionParticles.Play();
        Destroy(_explosionParticles, 3);
    }


    protected bool IsTargetInRange(float range)
    {
        if (_pursuitReference != null)
        {
            float distance = Vector3.Distance(transform.position, _pursuitReference.transform.position);
            return distance < range;
        }
        return false;
    }

    private void CheckNearbyVictims(float detectionRadius)
    {
        //Con el fin de evitar conflictos entre triggers, la busqueda de enemigos se hace mediante overlapping sphere
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("NPC"))
            {
                GameObject npcObject = collider.gameObject;
                if (_pursuitReference == null || !_pursuitReference.CompareTag("Player")){
                    Debug.Log("NPC detected: " + npcObject.name);
                    _pursuitReference = npcObject;
                    _state = FollowState;
                }
            }
            //El jugador siempre va a ser la prioridad de la persecusión
            else if (collider.CompareTag("Player"))
            {
                GameObject npcObject = collider.gameObject;
                Debug.Log("Player detected: " + npcObject.name);
                _pursuitReference = npcObject;
                _state = FollowState;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Car") && PlayerHelper.instance.PlayerFollowTarget.activeSelf)
        {

            _enemySoundSource.clip = SplatClip;
            _enemySoundSource.Play();
            ProcessDeath();
        }
    }


    private void OnDrawGizmos()
    {
        if (_state == WanderState)
        {
            Gizmos.color = Color.green;
        }
        if (_state == FollowState)
        {
            Gizmos.color = Color.yellow;
        }
        if (_state == AttackState)
        {
            Gizmos.color = Color.red;
        }
        if (_state == DeadState)
        {
            Gizmos.color = Color.white;
        }
        Gizmos.DrawWireSphere(transform.position, this.AttackRange * 2);
    }



    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

}
