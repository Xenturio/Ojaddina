using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace com.xenturio.multiplayer
{
    public class TankController : MonoBehaviour
    {

        [SerializeField] GameObject deathVfx;
        [SerializeField] AudioSource audioSource;

        private void Start()
        {
        }

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
