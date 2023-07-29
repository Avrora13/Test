using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Manager : MonoBehaviour
{

    [SerializeField] GameObject player;
    [SerializeField] List<GameObject> enemies;
    [SerializeField] float speedBullet;
    [SerializeField] int damageBullet;
    [SerializeField] int money;
    [SerializeField] float speed;
    List<GameObject> enemiesList = new List<GameObject>();
    [SerializeField] GameObject door;
    [SerializeField] GameObject panelMenu;
    [SerializeField] List<int> costs;
    [SerializeField] List<TMP_Text> costTexts;
    [SerializeField] TMP_Text moneyText;
    public int countEnemies;
    bool isPlay = false;
    void Start()
    {
        UpdateCosts();
    }

    // Update is called once per frame
    void Update()
    {
        if(countEnemies == 0 && isPlay) 
        {
            door.SetActive(false);
            isPlay = false;
        }
    }

    public void StartGame()
    {
        player.transform.position = new Vector3(-0.75f, 55.37f, -1.83f);
        Player playerComp = player.GetComponent<Player>();
        playerComp.bulletSpeed = speedBullet;
        playerComp.damage = damageBullet;
        playerComp.speed = speed;
        for(int i = 0; i < Random.Range(2,5);i++)
        {
            GameObject enemy = Instantiate(enemies[Random.Range(0,enemies.Count)]);
            enemy.transform.position = new Vector3(Random.Range(-5.36f, 3.66f), 56.19f, Random.Range(18.25f, 24.77f));
            enemy.GetComponent<Enemy>().player = player;
            enemy.GetComponent<Enemy>().manager = this;
            enemiesList.Add(enemy);
        }
        countEnemies = enemiesList.Count;
        door.SetActive(true);
        isPlay = true;
    }

    public void EndGame()
    {
        panelMenu.SetActive(true);
        if (enemiesList != null)
        {
            foreach (GameObject enemy in enemiesList)
            {
                Destroy(enemy);
            }
        }
        UpdateCosts();
    }
        
    public void SetMoney(int coins)
    {
        money += coins;
        moneyText.text = $"{money}$";
    }

    void UpdateCosts()
    {
        for (int i = 0; i < costTexts.Count; i++)
        {
            costTexts[i].text = $"{costs[i]} cost.";
        }
        moneyText.text = $"{money}$";
    }

    public void BuySpeedBullet()
    {
        if (money >= costs[0]) 
        {
            money -= costs[0];
            speedBullet += 1f;
            costs[0] = Mathf.RoundToInt(costs[0] * 1.2f);
            UpdateCosts();
        }
    }
    public void BuySpeed()
    {
        if (money >= costs[2])
        {
            money -= costs[2];
            speed += 1f;
            costs[2] = Mathf.RoundToInt(costs[2] * 1.2f);
            UpdateCosts();
        }
    }
    public void BuyDamage()
    {
        if (money >= costs[1])
        {
            money -= costs[1];
            damageBullet += 3;
            costs[1] = Mathf.RoundToInt(costs[1] * 1.2f);
            UpdateCosts();
        }
    }
}
