using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour
{
    [SerializeField] DynamicJoystick joystick;
    Rigidbody rb;
    public float speed = 5f;
    float rotationSpeed = 20f;
    public float health = 100;
    [SerializeField] List<LayerMask> enemyLayers;
    private Transform targetEnemy;
    [SerializeField] GameObject bulletPrefab;
    public float bulletSpeed;
    public int damage;
    [SerializeField] GameObject spawnPoint;
    float timer;
    public float visionRadius = Mathf.Infinity;
    [SerializeField] Manager manager;
    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = new Vector3(joystick.Horizontal, 0f, joystick.Vertical);
        movement = movement.normalized;
        rb.velocity = movement * speed;

        if (movement != Vector3.zero)
        {
            float angle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg;
            float angle2 = Mathf.LerpAngle(transform.eulerAngles.y, angle, Time.deltaTime * rotationSpeed); // ƒвижение персонажа и его поворот.
            transform.rotation = Quaternion.Euler(0f, angle2, 0f);
        }
        else
        {
            FindNearestEnemy();
            if (targetEnemy != null)
            {
                Vector3 direction = targetEnemy.position - transform.position;
                direction.y = 0f;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed); // ѕри остановке поворот персонажа на ближнего врага
            }
            if(timer <= 0)
            {
                SpawnBullet();
            }
        }
        timer -= Time.deltaTime;
        if(health <= 0)
        {
            manager.EndGame();
        }
    }

    void FindNearestEnemy() // Ќахождение ближнего врага к игроку
    {
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;
        foreach (LayerMask enemyLayer in enemyLayers)
        {
            Collider[] enemies = Physics.OverlapSphere(transform.position, visionRadius, enemyLayer);

            foreach (Collider enemy in enemies)
            {
                Vector3 distanceToEnemy = enemy.transform.position - transform.position;
                if (!Physics.Raycast(transform.position, distanceToEnemy.normalized, distanceToEnemy.magnitude, ~enemyLayer)) // ѕроверка не находитс€ ли враг за укрытием
                {
                    // ≈сли преп€тствий нет, значит враг в зоне видимости
                    float distance = Vector3.Distance(transform.position, enemy.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestEnemy = enemy.transform;
                    }
                }
            }
        }
        if (closestDistance == Mathf.Infinity)
        {
            closestEnemy = null;
        }
        targetEnemy = closestEnemy;
    }
    void SpawnBullet() // ¬ыстрел во врага
    {
        if(targetEnemy != null)
        {
            GameObject canvasBullet = Instantiate(bulletPrefab);
            canvasBullet.transform.SetPositionAndRotation(spawnPoint.transform.position, Quaternion.identity);
            Bullet bulletComponent = canvasBullet.GetComponent<Bullet>();
            bulletComponent.SetTarget(targetEnemy.gameObject);
            bulletComponent.speed = bulletSpeed;
            bulletComponent.damage = damage;
            canvasBullet.SetActive(true);
            timer = 2f;
        }   
    }
}
