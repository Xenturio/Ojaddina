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

    private Vector2 startScale;

    private bool positionChanged = false;

    private BoxCollider2D boxCollider;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
    

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Territory neighborTerritory = collider.GetComponent<Territory>();
        if (neighborTerritory != null && !neighborTerritories.Contains(neighborTerritory))
        {
            Debug.Log(name + " confina con " + neighborTerritory.name);
            neighborTerritories.Add(neighborTerritory);
        }
    }


    public void SetOwner(Player owner) {
        this.owner = owner;
    }
}
