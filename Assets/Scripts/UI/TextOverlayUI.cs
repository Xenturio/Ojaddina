using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextOverlayUI : MonoBehaviour
{
    bool warBegin = false;
    Color baseColor;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        baseColor = GetComponent<Text>().color;
        GetComponentInParent<Canvas>().gameObject.SetActive(false);
    }
    
    public void WarBegin()
    {
        if (!warBegin)
        {
            Debug.Log("Start WarBegin");
            warBegin = true;
            if (GetComponentInParent<Canvas>() != null && GetComponentInParent<Canvas>().gameObject != null)
            {
                GetComponentInParent<Canvas>().gameObject.SetActive(true);
                StartCoroutine(UpdateWarBegin());
            }
        }
    }

    IEnumerator UpdateWarBegin()
    {
        GetComponent<Text>().text = "IT'S TIME FOR WAR";
        GetComponent<Text>().color = baseColor;
        yield return new WaitForSeconds(2f);
        GetComponentInParent<Canvas>().gameObject.SetActive(false);       
    }

    public void NextPlayer(Player player)
    {
        if (warBegin)
        {
            GetComponentInParent<Canvas>().gameObject.SetActive(true);
            StartCoroutine(NextPlayerText(player));
        }
    }

    IEnumerator NextPlayerText(Player player)
    {
        Debug.Log("Next Player");
        GetComponent<Text>().text = player.name.ToUpper();
        GetComponent<Text>().color = player.GetPlayerColor();
        yield return new WaitForSeconds(1f);
        GetComponentInParent<Canvas>().gameObject.SetActive(false);
    }
}
