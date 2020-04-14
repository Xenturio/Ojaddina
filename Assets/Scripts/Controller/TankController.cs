using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace com.xenturio.basegame
{
    public class TankController : MonoBehaviour
    {

        [SerializeField] protected GameObject deathVfx;
        [SerializeField] protected AudioSource audioSource;
        
        public void Death()
        {
            if (deathVfx)
            {
                if (audioSource != null)
                {
                    audioSource.Play(0);
                }
                GameObject effetc = Instantiate(deathVfx, transform.position, transform.rotation);
                gameObject.SetActive(false);
                Destroy(effetc, 2f);
            }
        }
    }
}