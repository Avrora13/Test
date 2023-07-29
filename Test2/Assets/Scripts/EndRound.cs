using UnityEngine;

public class EndRound : MonoBehaviour
{
    [SerializeField] Manager manager;
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player")
        {
            manager.EndGame();
        }
    }
}
