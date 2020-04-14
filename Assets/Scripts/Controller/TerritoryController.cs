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

        [SerializeField] protected Sprite selectedSprite;

        [SerializeField] protected Sprite alternativeSprite;

        protected Sprite baseSprite;

        protected TerritoryData territory;

        protected GameManager gameManager;

        private MultiplayerGameManager multiplayerGameManager;

        protected TerritoryTextName textName;

        private PlayerController ownerController;

        protected bool hasSelectedSprite = false;
        protected bool hasAlternativeSprite = false;

        public TerritoryData Territory { get => territory; set => territory = value; }
        public PlayerController OwnerController { get => ownerController; set => ownerController = value; }

        private void Awake()
        {
            multiplayerGameManager = FindObjectOfType<MultiplayerGameManager>();
            gameManager = FindObjectOfType<GameManager>();
            textName = GetComponentInChildren<TerritoryTextName>();
            territory = GetComponent<TerritoryData>();
            baseSprite = GetComponent<Button>().image.sprite;
            GetComponentInChildren<Text>().text = territory.GetTerritoryName();
        }

        // Start is called before the first frame update
        void Start()
        {            
            UpdateArmyDisplay();
        }

        private void OnDestroy()
        {
            Debug.Log("Chi cazzo è stato?");
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

        public void LoseArmies(int armies)
        {
            if (territory.GetArmies() > 1)
            {
                ownerController.LostArmies(armies);
                RemoveArmies(armies);
            }
        }

        public void SetOwner(PlayerController owner)
        {
            ownerController = owner;
            territory.SetOwner(owner.GetPlayer());
            textName.SetColorPlayerText();
            SetTankSpriteColor();
            if (multiplayerGameManager)
                multiplayerGameManager.RaiseEvent(owner.GetPlayer().GetPlayerName(), EventNetwork.TERRITORY_SET_OWNER, Territory.GetTerritoryName(), null, false);
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
            if (this.gameObject && this.gameObject.GetComponentInChildren<TextArmy>())
            {
                this.gameObject.GetComponentInChildren<TextArmy>().UpdateArmyNumber(territory.GetArmies());
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
            }
        }

        protected void StopAttack()
        {
            GetGameManager().SetDesinationTerritory(null);
        }

        protected void MoveArmy()
        {
            if (GetGameManager().GetSelectedTerritory() != null && !GetGameManager().GetSelectedTerritory().Equals(this) && IsNeighbor())
            {
                GetGameManager().SetDesinationTerritory(this);
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
            SplitSprite();
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
                    GetGameManager().SetDesinationTerritory(null);
                }
                else if (GetGameManager().GetSelectedTerritory() != null)
                {
                    MoveArmy();
                }
                else if (GetGameManager().GetSelectedTerritory() == null)
                {
                    GetGameManager().SetSelectedTerritory(this);
                }
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

        private void OnMouseExit()
        {
            SplitSprite();
        }

        

        protected void SplitSprite()
        {

            if (GetGameManager().GetDestinationTerritory() != null && GetGameManager().GetDestinationTerritory().Equals(this))
            {
                if (!hasAlternativeSprite)
                {
                    hasAlternativeSprite = true;
                    hasSelectedSprite = false;
                    GetComponent<Button>().image.sprite = alternativeSprite;
                }
                return;
            }
            if (GetGameManager().GetSelectedTerritory() != null && GetGameManager().GetSelectedTerritory().Equals(this))
            {
                if (!hasSelectedSprite)
                {
                    GetComponent<Button>().image.sprite = selectedSprite;
                    hasSelectedSprite = true;
                    hasAlternativeSprite = false;
                }
                return;
            }
            hasAlternativeSprite = false;
            hasSelectedSprite = false;
            if (!GetComponent<Button>().image.sprite.Equals(baseSprite))
            {
                GetComponent<Button>().image.sprite = baseSprite;
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