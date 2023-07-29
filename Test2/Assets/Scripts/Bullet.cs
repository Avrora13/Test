using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 10;
    float lifeTime = 5f;
    public float speed = 4f;
    Vector3 target;
    Vector3 direction;
    private Rigidbody rb;
    void Start()
    {
        Destroy(gameObject, lifeTime);  // Уничтожаем пулю через lifeTime секунд
        rb = GetComponent<Rigidbody>();
        if (target == null)
        {
            Destroy(gameObject);  // Если враг не установлен, уничтожаем пулю
        }
    }

    void Update()
    {
        if (target != null)
        {
            rb.velocity = direction * speed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Enemy>().health -= damage;
        }
        else if (collision.transform.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().health -= damage;
        }
        Destroy(this.gameObject);
    }

    public void SetTarget(GameObject obj) // Назначение цели
    {
        target = obj.transform.position;
        direction = (target - transform.position).normalized;
    }
}
