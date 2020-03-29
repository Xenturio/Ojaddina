using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TerritoryController : MonoBehaviour, UnityEngine.EventSystems.IPointerClickHandler
{
    private Territory territory;

    private GameManager gameManager;

    private TerritoryTextName textName;

    private TextArmy textArmy;

    private Territory attackingTerritory;

    private PlayerController ownerController;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        GetComponentInChildren<Text>().text = name;
        textName = GetComponentInChildren<TerritoryTextName>();
        textArmy = GetComponentInChildren<TextArmy>();
        territory = GetComponent<Territory>();
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateArmyDisplay();
    }

    public void AddArmy()
    {
        if (GameStatesController.IsReinforce() && territory.GetPlayer().GetArmiesPerTurn() > 0)
        {
            territory.GetPlayer().RemoveArmyPerTurn();
            territory.AddArmy();
            UpdateArmyDisplay();
        }
        else if (GameStatesController.IsMove())
        {
            //territory.AddArmy();
            //UpdateArmyDisplay();
        }
        else if (GameStatesController.IsSetupGame() && territory.GetPlayer().GetArmiesPerTurn() > 0)
        {
            territory.GetPlayer().RemoveArmyPerTurn();
            territory.AddArmy();
            UpdateArmyDisplay();
        }
        else if(GameStatesController.IsNotStartedGame())
        {
            territory.AddArmy();
            UpdateArmyDisplay();
        }

    }

    public void RemoveArmy()
    {
        if (territory.GetArmies() > 1)
        {
            territory.GetPlayer().AddArmyPerTurn(1);
            territory.RemoveArmy();
            UpdateArmyDisplay();
        }
    }

    public void SetOwner(PlayerController owner)
    {
        ownerController = owner;
        territory.SetOwner(owner.GetPlayer());
        textName.SetColorPlayerText();
        GetComponentInChildren<SpriteRenderer>().color = owner.GetPlayerColor();
    }

    private void HighlightTerritory()
    {
        if (IsSelectableTerritory())
        {
            gameManager.SetSelectedTerritory(this);
        }
    }

    private bool IsSelectableTerritory()
    {

        //Un territorio è selezionabile quando è un mio territorio
        if (territory.GetPlayer() == null || gameManager.GetCurrentPlayer() == null)
        {
            return false;
        }
        if (territory.GetPlayer().name == gameManager.GetCurrentPlayer().name)
        {
            return true;
        }
        return false;
    }

    private bool IsAttackableTerritory()
    {

        if (territory.GetPlayer().name == gameManager.GetCurrentPlayer().name)
        {
            return false;
        }

        if (gameManager.GetSelectedTerritory() == null) {
            return false;
        }


        if (gameManager.GetSelectedTerritory().GetTerritory().GetArmies() < GameSettings.MIN_DICE_TO_ATTACK)
        {
            return false;
        }
        
        return IsNeighbor();
    }

    private bool IsNeighbor() {

        if (gameManager.GetSelectedTerritory() != null)
        {
            foreach (Territory neighbour in gameManager.GetSelectedTerritory().GetTerritory().GetNeighborTerritories())
            {
                if (neighbour.name.Equals(territory.name))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void UpdateArmyDisplay()
    {
        textArmy.UpdateArmyNumber(territory.GetArmies());
    }

    private void Attack()
    {
        if (IsAttackableTerritory())
        {
            gameManager.SetDesinationTerritory(this);
            GetComponentInChildren<SwordsTerritoryUI>().EnableAnimation();
        }
    }

    private void StopAttack() {
        gameManager.SetDesinationTerritory(null);
        GetComponentInChildren<SwordsTerritoryUI>().DisableAnimation();
    }

    private void MoveArmy() {
        if (gameManager.GetSelectedTerritory() != null && !gameManager.GetSelectedTerritory().Equals(territory) && IsNeighbor())
        {
            gameManager.SetDesinationTerritory(this);
        }
    }

    private void HandleClick(bool left) {
        bool isOwner = territory.GetPlayer().Equals(gameManager.GetCurrentPlayer());
        if (left)
        {
            if (isOwner && !GameStatesController.IsAttack())
            {
                AddArmy();
            }
            else if (isOwner && GameStatesController.IsAttack())
            {
                gameManager.SetSelectedTerritory(this);
            }
            else if (!isOwner && GameStatesController.IsAttack() && gameManager.GetSelectedTerritory() != null)
            {
                Attack();
            }
        }
        else {
            if (isOwner && GameStatesController.IsMove()) {
                MoveArmy();
            }
            if (isOwner && !GameStatesController.IsAttack())
            {
                RemoveArmy();
            }
        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Click territory");        
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            HandleClick(true);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            HandleClick(false);
        }

    }

    public Territory GetTerritory() {
        return territory;
    }

    public PlayerController GetOwnerController() {
        return ownerController;
    }

}
