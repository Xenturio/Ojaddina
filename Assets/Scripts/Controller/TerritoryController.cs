using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using com.xenturio.entity;
using com.xenturio.UI;
using com.xenturio.multiplayer;

namespace com.xenturio.basegame
{
    public class TerritoryController : MonoBehaviour, UnityEngine.EventSystems.IPointerClickHandler
    {
        
        protected TerritoryData territory;

        protected GameManager gameManager;

        private MultiplayerGameManager multiplayerGameManager;

        private PlayerController ownerController;
        
        public TerritoryData Territory { get => territory; set => territory = value; }
        public PlayerController OwnerController { get => ownerController; set => ownerController = value; }

        private void Awake()
        {
            multiplayerGameManager = FindObjectOfType<MultiplayerGameManager>();
            gameManager = FindObjectOfType<GameManager>();
            territory = GetComponent<TerritoryData>();
        }

        // Start is called before the first frame update
        void Start()
        {            
            UpdateArmyDisplay();
        }

        public TerritoryData GetTerritory()
        {
            return territory;
        }

        public PlayerController GetOwnerController()
        {
            return ownerController;
        }

        public void AddArmies(int armies)
        {
            territory.SetArmies(territory.GetArmies() + armies);
            UpdateArmyDisplay();
            if(multiplayerGameManager)
                multiplayerGameManager.RaiseEvent(armies, EventNetwork.TERRITORY_ADD_ARMY, Territory.GetTerritoryName(), null, false);
        }

        public void RemoveArmies(int armies)
        {
            territory.SetArmies(territory.GetArmies() - armies);
            UpdateArmyDisplay();
            if (multiplayerGameManager)
                multiplayerGameManager.RaiseEvent(armies, EventNetwork.TERRITORY_LOST_ARMY, Territory.GetTerritoryName(), null, false);
        }

        public void AddArmy()
        {
            if (GameStatesController.IsReinforce() && ownerController.GetArmiesPerTurn() > 0)
            {
                ownerController.RemoveArmyPerTurn();
                AddArmies(1);
            }
            else if (GameStatesController.IsSetupGame() && ownerController.GetArmiesPerTurn() > 0)
            {
                ownerController.RemoveArmyPerTurn();
                AddArmies(1);
            }
            else if (GameStatesController.IsNotStartedGame())
            {
                AddArmies(1);
            }

        }

        public void RemoveArmy()
        {
            if (territory.GetArmies() > 1)
            {
                if (GameStatesController.IsReinforce())
                {
                    if (territory.GetStartReinforceArmies() >= territory.GetArmies())
                    {
                        return;
                    }
                }
                ownerController.AddArmyPerTurn(1);
                RemoveArmies(1);
            }
        }

        /**
         * Lose armies after battle.
         * */
        public void LoseArmies(int armies)
        {
            ownerController.LostArmies(armies);
            RemoveArmies(armies);
        }

        public void SetOwner(PlayerController owner)
        {
            ownerController = owner;
            territory.SetOwner(owner.GetPlayer());
            SetTankSpriteColor();            
        }

        protected void SetTankSpriteColor() {

            Color color = ownerController.GetPlayerColor();
            if (ownerController.GetPlayerColor().Equals(Color.black)) {
                color = new Color32(70, 70, 70, 255);
            }
            GetComponentInChildren<SpriteRenderer>().color = color;
        }

        protected void HighlightTerritory()
        {
            if (IsSelectableTerritory())
            {
                GetGameManager().SetSelectedTerritory(this);
                CmdEventSelectTerritory(this.Territory);
            }
        }

        protected bool IsSelectableTerritory()
        {

            //Un territorio è selezionabile quando è un mio territorio
            if (ownerController == null || GetGameManager().CurrentPlayerController == null)
            {
                return false;
            }
            if (ownerController.Player.GetPlayerName() == GetGameManager().CurrentPlayerController.Player.GetPlayerName())
            {
                return true;
            }
            return false;
        }

        protected bool IsAttackableTerritory()
        {

            if (ownerController.Player.GetPlayerName() == GetGameManager().CurrentPlayerController.Player.GetPlayerName())
            {
                return false;
            }

            if (GetGameManager().GetSelectedTerritory() == null)
            {
                return false;
            }


            if (GetGameManager().GetSelectedTerritory().GetTerritory().GetArmies() < GameSettings.MIN_DICE_TO_ATTACK)
            {
                return false;
            }

            return IsNeighbor();
        }

        protected bool IsNeighbor()
        {

            if (GetGameManager().GetSelectedTerritory() != null)
            {
                foreach (TerritoryData neighbour in GetGameManager().GetSelectedTerritory().GetTerritory().GetNeighborTerritories())
                {
                    if (neighbour.GetTerritoryName().Equals(territory.GetTerritoryName()))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected void UpdateArmyDisplay()
        {
            if (this.gameObject && this.gameObject.GetComponentInChildren<Text>())
            {
                this.gameObject.GetComponentInChildren<Text>().text = "" + territory.GetArmies();
            }
            else {
                Debug.LogError("UpdateArmyDisplay -> this.gameobject = " + this.gameObject + " --- TextArmy = " + this.gameObject.GetComponentInChildren<TextArmy>());
            }
        }

        protected void Attack()
        {
            if (IsAttackableTerritory())
            {
                GetGameManager().SetDesinationTerritory(this);
                CmdEventDestinationTerritory(this.Territory);
            }
        }

        protected void StopAttack()
        {
            GetGameManager().SetDesinationTerritory(null);
            CmdEventDestinationTerritory(null);
        }

        protected void MoveArmy()
        {
            if (GetGameManager().GetSelectedTerritory() != null && !GetGameManager().GetSelectedTerritory().Equals(this) && IsNeighbor())
            {
                GetGameManager().SetDesinationTerritory(this);
                CmdEventDestinationTerritory(this.Territory);
            }
        }

        public void UpdateTerritoryAtEndTurn()
        {
            territory.SetStartReinforceArmies(territory.GetArmies());
        }

        protected void HandleClick(bool left)
        {
            if (!GetGameManager().IsMyTurn()) { return; }
            if (!ownerController) { return; }
            bool isOwner = ownerController.Equals(GetGameManager().CurrentPlayerController);
            if (left)
            {
                if (isOwner && !GameStatesController.IsAttack() && !GameStatesController.IsMove())
                {
                    AddArmy();
                }
            }
            else
            {
                if (isOwner && !GameStatesController.IsAttack() && !GameStatesController.IsMove())
                {
                    RemoveArmy();
                }
            }
            if (isOwner && GameStatesController.IsAttack())
            {
                GetGameManager().SetSelectedTerritory(this);
                CmdEventSelectTerritory(this.Territory);
            }
            else if (!isOwner && GameStatesController.IsAttack() && GetGameManager().GetSelectedTerritory() != null)
            {
                Attack();
            }
            if (isOwner && GameStatesController.IsMove())
            {
                //Deseleziono il territorio
                if (GetGameManager().GetSelectedTerritory() != null && GetGameManager().GetSelectedTerritory().Equals(this))
                {
                    GetGameManager().SetSelectedTerritory(null);
                    CmdEventSelectTerritory(null);
                    GetGameManager().SetDesinationTerritory(null);
                    CmdEventDestinationTerritory(null);
                }
                else if (GetGameManager().GetSelectedTerritory() != null)
                {
                    MoveArmy();
                }
                else if (GetGameManager().GetSelectedTerritory() == null)
                {
                    GetGameManager().SetSelectedTerritory(this);
                    CmdEventSelectTerritory(this.Territory);
                }
            }

        }

        private void CmdEventSelectTerritory(TerritoryData territory) {
            if (multiplayerGameManager)
            {
                multiplayerGameManager.RaiseEvent(territory ? territory.GetTerritoryName() : null, EventNetwork.SET_SELECTED_TERRITORY, Territory.GetTerritoryName(), null, false);
            }
        }

        private void CmdEventDestinationTerritory(TerritoryData territory)
        {
            if (multiplayerGameManager)
            {
                multiplayerGameManager.RaiseEvent(territory ? territory.GetTerritoryName() : null, EventNetwork.SET_DESTINATION_TERRITORY, Territory.GetTerritoryName(), null, false);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                HandleClick(true);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                HandleClick(false);
            }

        }
        
        private GameManager GetGameManager() {
            if (multiplayerGameManager != null) {
                return multiplayerGameManager;
            }
            return gameManager;
        }
    }
}