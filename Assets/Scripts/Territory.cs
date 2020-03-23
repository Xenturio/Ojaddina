using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Territory : MonoBehaviour
{

    [SerializeField] string name;

    [SerializeField] Continent continent;

    [SerializeField] List<Territory> neighborTerritories;

    private Player owner;

    private List<Army> armies;

    // Start is called before the first frame update
    void Start()
    {
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log(name + " contatto con " + collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log(name + " trigger con " + collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(name + " contatto con " + collision);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(name + " trigger con " + collision);
    }


    public void SetOwner(Player owner) {
        this.owner = owner;
    }
}
