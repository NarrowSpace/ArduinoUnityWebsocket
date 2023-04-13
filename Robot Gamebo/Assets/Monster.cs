using UnityEngine;
using CodeMonkey.HealthSystemCM;


public class Monster : MonoBehaviour
{
    private HealthSystem healthSystem;

    private void Awake()
    {
        healthSystem = new HealthSystem(100);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damage() 
    {
        healthSystem.Damage(10);
    }
}
