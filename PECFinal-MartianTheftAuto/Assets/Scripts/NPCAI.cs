using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/** Define mediante Delegates los diferentes estados de un NPC del  en el juego, 
 * teniendo una rutina definida en el espacio de juego, pudiendo 
 * Esteban.Hernandez
 */
public class NPCAI : MonoBehaviour
{
    public float Health = 10;
    public float Speed = 30;
    public int FleeMultiplier = 3;
    public float StateEvaluationFrequency = 0.05f; // 1/20 de segundo, el ciclo de ejecución de los estados.
    public float TargetProximity = 1.2f;
    public float InfectionDistanteThreshold = 0.5f;
    public List<Transform> FollowRoute;
    public AudioClip DamageClip;
    public AudioClip FleeClip;
    public AudioClip SplatClip;
    public GameObject ZombiePrefab;
    public GameObject ExplosionPrefab;
    //Delegar la definicion de corrutinas sin parámetros mediante State()
    delegate IEnumerator State();
    [SerializeField]
    private int _currentTarget = 0;
    [SerializeField]
    private State _state;
    private NavMeshAgent _agent;
    [SerializeField]
    private AudioSource _enemySoundSource;
    private Animator _animator;
    private GameObject _enemyReference;
    private ParticleSystem _explosionParticles;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();
        _enemySoundSource = GetComponent<AudioSource>();
        _explosionParticles = Instantiate(ExplosionPrefab).GetComponent<ParticleSystem>();
        _explosionParticles.gameObject.SetActive(false);
        _agent.speed = Speed;
        _animator.SetBool("IsAlive", true);
        _animator.SetBool("IsDamaged", false);
        _animator.SetBool("IsFleeing", false);
    }

    IEnumerator Start()
    {
        _state = PatrolState;
        while (enabled)
        {
            yield return StartCoroutine(_state());
        }
    }

    //Rutina de patrulla, moviendose entre waypoints
    IEnumerator PatrolState()
    {
        _agent.isStopped = false;
        _agent.speed = Speed;
        _animator.SetFloat("Speed", Speed);
        if (_agent.remainingDistance < TargetProximity)
        {
            if (_currentTarget == FollowRoute.Count - 1)
            {
                _currentTarget = 0;
            }
            else
            {
                _currentTarget++;
            }
        }
        _agent.destination = FollowRoute[_currentTarget].position;
        yield return StateEvaluationFrequency;
    }


    //Rutina de alerta al detectar un enemigo, debe empezar a correr 
    IEnumerator FleeState()
    {
        _agent.isStopped = false;
        _animator.SetFloat("Speed", Speed * FleeMultiplier);
        _animator.SetBool("IsFleeing",true);
        Vector3 awayFromTarget = transform.position - _enemyReference.transform.position;
        Vector3 fleeDestination = transform.position + awayFromTarget;
        _agent.SetDestination(fleeDestination);
        yield return StateEvaluationFrequency;
    }

    IEnumerator AttackedState()
    {
        yield return StateEvaluationFrequency;
    }

    //Hacer que el enemigo no efectue más acciones una vez haya muerto
    IEnumerator DeadState()
    {
        yield return StateEvaluationFrequency;
    }


    //Metodo para ser invocado cada que el NPC sufra daño
    public void ReceiveHit(float damage)
    {
        Debug.Log($"Damage! {damage}");
        _animator.SetFloat("Speed", 0);
        _animator.SetTrigger("IsDamaged");
        _enemySoundSource.PlayOneShot(DamageClip);
        _agent.speed = 0;
        _state = AttackedState;
        Health -= damage;
        if (Health <= 0)
        {
            ProcessDeath();
        }
        else
        {
            _agent.isStopped = true;
        }
    }

    protected void ProcessDeath()
    {
        _state = DeadState;
        _enemySoundSource.Play();
        _animator.SetBool("IsAlive", false);
        _animator.Play("Death");
        _agent.speed = 0;
        _agent.isStopped = true;
        _explosionParticles.transform.position = transform.position;
        _explosionParticles.gameObject.SetActive(true);
        _explosionParticles.Play();
        Instantiate(this.ZombiePrefab, this.transform.position, this.transform.rotation);
        Destroy(_explosionParticles, 3);
        Destroy(this.gameObject);
    }
    protected void ProcessCarHit()
    {
        _state = DeadState;
        _enemySoundSource.Play();
        _animator.SetBool("IsAlive", false);
        _animator.Play("Death");
        _agent.speed = 0;
        _agent.isStopped = true;
        _explosionParticles.transform.position = transform.position;
        _explosionParticles.gameObject.SetActive(true);
        _explosionParticles.Play();
        Destroy(this.gameObject);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Car"))
        {
            _enemySoundSource.clip = SplatClip;
            _enemySoundSource.Play();
            ProcessCarHit();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && _state != DeadState)
        {
            _state = FleeState;
            _enemyReference = other.gameObject;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy") && _state != DeadState)
        {
            if (Vector3.Distance(_enemyReference.transform.position, this.transform.position) < InfectionDistanteThreshold)
            {
                _state = DeadState;
                _enemySoundSource.PlayOneShot(FleeClip);
                _animator.SetFloat("Speed", 0);
            }
        }
    }



    private void OnTriggerExit(Collider other)
    {
        _state = PatrolState;
        _animator.SetFloat("Speed", Speed);
    }

}
