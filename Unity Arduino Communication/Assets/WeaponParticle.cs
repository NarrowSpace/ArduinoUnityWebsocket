using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponParticle : MonoBehaviour
{
    [SerializeField]
    private float fireRate;
    private ParticleSystem particleSystem;

    private List<ParticleCollisionEvent> particleCollision;

    private bool fireCoolDown;

    private string monsterTag;

    public Animator monsterAnimator;

    private int counter = 10;

    public Image[] hearts;

    public AudioSource hitSrc;
    public AudioClip hitSound;


    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        particleCollision = new List<ParticleCollisionEvent>();
        monsterAnimator = GameObject.Find("Monster").GetComponent<Animator>();
    }

    void OnParticleCollision(GameObject other)
    {
        ParticlePhysicsExtensions.GetCollisionEvents(particleSystem, other, particleCollision);

        for(int i = 0; i < particleCollision.Count; i++)
        {
            var collider = particleCollision[i].colliderComponent;

            if (collider.CompareTag(monsterTag))
            {

                hitSrc.clip = hitSound;
                hitSrc.Play();

                counter -- ;
                Debug.Log("Counter" + counter);

                if (counter < 10 && counter >= 0)
                {
                    hearts[counter].enabled = false;
                }

                monsterAnimator.SetTrigger("GetHit");
            }
        }
    }

    public void SetEnemyTag(string monsterTag)
    {
        this.monsterTag = monsterTag;
    }

    public void Fire()
    {
        if (fireCoolDown) return;
        particleSystem.Emit(1);
        fireCoolDown = true;
        StartCoroutine(StopCoolDownAfterTime());
    }
    
    private IEnumerator StopCoolDownAfterTime()
    {
        yield return new WaitForSeconds(fireRate);
        fireCoolDown = false;
    }

}
