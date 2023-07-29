using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject player;
    public int health = 100;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform spawnPoint;
    float timer = 0;
    bool isRotate = false;
    Rigidbody rg;
    float speedRotation = 200f;
    [SerializeField] LayerMask playerLayer;
    Transform lastPlayerVision;
    bool isSee = false;
    Vector3 hideSpot;
    enum enemyState {Stay,Attack,Hide,Follow }
    enemyState state = enemyState.Stay;
    public Manager manager;
    float timerHide = 3f;

    void Start()
    {
        rg = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float step;
        switch (state)
        {
            case enemyState.Stay:
                RaycastHit hit;
                Vector3 directionToPlayer = player.transform.position - transform.position;           
                if (!Physics.Raycast(transform.position, directionToPlayer.normalized, directionToPlayer.magnitude, ~playerLayer)) // ¬раг ищет игрока и выполн€етс€ код, если игрок не за укрытием
                {
                    lastPlayerVision = player.transform;
                    isSee = true;
                    Quaternion lookPlayer = Quaternion.LookRotation(directionToPlayer);

                    transform.rotation = Quaternion.RotateTowards(transform.rotation, lookPlayer, speedRotation * Time.deltaTime);
                    float angleDifference = Quaternion.Angle(transform.rotation, lookPlayer);
                    if (angleDifference <= 3f)
                    {
                        isRotate = true;
                        state = enemyState.Attack; // ≈сли враг повернулс€ на игрока, переходит в другой режим
                    }
                    else
                    {
                        isRotate = false;
                    }                 
                }
                if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hit, Mathf.Infinity)) // ≈сли игрок не найдет пр€четс€ за укрытие
                {
                    if (hit.collider.tag == "HideSpot")
                    {
                        hideSpot = hit.point + new Vector3(Random.Range(0.5f, 1f), 0f, Random.Range(0.5f, 1f));
                        state = enemyState.Hide;
                        timerHide = 3f;
                    }
                }
                else
                {
                    rg.velocity = Vector3.zero;
                }
                break;   
            case enemyState.Attack:
                if (timer <= 0)
                {
                    SpawnBullet();
                }
                else
                {
                    timer -= Time.deltaTime;
                }
                directionToPlayer = player.transform.position - transform.position;
                if (!Physics.Raycast(transform.position, directionToPlayer.normalized, directionToPlayer.magnitude, ~playerLayer))
                {

                }
                else
                {
                    state = enemyState.Stay;
                }
                    break;
            case enemyState.Hide:
                if(isSee && timerHide > 0)
                {
                    step = 5f * Time.deltaTime; // —корость движени€ врага
                    transform.position = Vector3.MoveTowards(transform.position, hideSpot, step);

                    // ѕровер€ем, достигли ли мы укрыти€, и если да, выключаем движение:
                    if (transform.position == hideSpot)
                    {
                        isSee = false;
                        state = enemyState.Follow;
                    }
                }
                else
                {
                    state = enemyState.Stay;
                }
                break;
            case enemyState.Follow:
                step = 5f * Time.deltaTime; // —корость движени€ врага
                transform.position = Vector3.MoveTowards(transform.position, lastPlayerVision.position, step);
                directionToPlayer = player.transform.position - transform.position;
                if (!Physics.Raycast(transform.position, directionToPlayer.normalized, directionToPlayer.magnitude, ~playerLayer))
                {
                    state = enemyState.Stay;
                }
                else if (transform.position == lastPlayerVision.position)
                {
                    state = enemyState.Stay;
                }
                break;
        }    
        if (health <= 0)
        {
            manager.SetMoney(Random.Range(5, 25));
            manager.countEnemies--;
            Destroy(gameObject);
        }
        if(timerHide > 0)
        {
            timerHide -= Time.deltaTime;
        }
    }

    void SpawnBullet()
    {
        if (isRotate)
        {
            GameObject canvasBullet = Instantiate(bulletPrefab);
            canvasBullet.transform.SetPositionAndRotation(spawnPoint.transform.position, Quaternion.identity);
            Bullet bulletComponent = canvasBullet.GetComponent<Bullet>();
            bulletComponent.SetTarget(player);
            canvasBullet.SetActive(true);
            timer = 2f;
        }
    }
}
