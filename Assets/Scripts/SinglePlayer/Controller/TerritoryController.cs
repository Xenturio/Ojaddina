using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using com.xenturio.entity;
using com.xenturio.enums;

namespace com.xenturio.singleplayer
{
    public class TerritoryController : MonoBehaviour, UnityEngine.EventSystems.IPointerClickHandler
    {
        [SerializeField] Sprite selectedSprite;
        [SerializeField] Sprite alternativeSprite;

        Sprite baseSprite;

        private Territory territory;

        private GameManager gameManager;

        private TerritoryTextName textName;

        private TextArmy textArmy;

        private PlayerController ownerController;

        private bool hasSelectedSprite = false;
        private bool hasAlternativeSprite = false;

        public Territory Territory { get => territory; set => territory = value; }
        public PlayerController OwnerController { get => ownerController; set => ownerController = value; }

        private void Awake()
        {
            gameManager = FindObjectOfType<GameManager>();
            GetComponentInChildren<Text>().text = name;
            textName = GetComponentInChildren<TerritoryTextName>();
            textArmy = GetComponentInChildren<TextArmy>();
            territory = GetComponent<Territory>();
            baseSprite = GetComponent<Button>().image.sprite;
        }

        // Start is called before the first frame update
        void Start()
        {
            if (string.IsNullOrEmpty(territory.GetTerritoryName()))
            {
                territory.TerritoryName = this.gameObject.name;
            }
            GetComponentInChildren<Text>().text = territory.TerritoryName;
            UpdateArmyDisplay();
        }

        void Update()
        {
            SplitSprite();
        }

        public void AddArmies(int armies)
        {
            territory.SetArmies(territory.GetArmies() + armies);
            UpdateArmyDisplay();
        }

        public void RemoveArmies(int armies)
        {
            territory.SetArmies(territory.GetArmies() - armies);
            UpdateArmyDisplay();
        }

        public void AddArmy()
        {
            if (GameStatesController.IsReinforce() && ownerController.GetArmiesPerTurn() > 0)
            {
                ownerController.RemoveArmyPerTurn();
                territory.AddArmy();
                UpdateArmyDisplay();
            }
            else if (GameStatesController.IsSetupGame() && ownerController.GetArmiesPerTurn() > 0)
            {
                ownerController.RemoveArmyPerTurn();
                territory.AddArmy();
                UpdateArmyDisplay();
            }
            else if (GameStatesController.IsNotStartedGame())
            {
                territory.AddArmy();
                UpdateArmyDisplay();
            }

        }

        public void RemoveArmy()
        {
            if (territory.GetArmies() > 1)
            {
                if (GameStatesController.IsReinforce())
                {
                    if (territory.StartReinforceArmies >= territory.Armies)
                    {
                        return;
                    }
                }
                ownerController.AddArmyPerTurn(1);
                territory.RemoveArmy(1);
                UpdateArmyDisplay();
            }
        }

        public void LoseArmies(int armies)
        {
            if (territory.GetArmies() > 1)
            {
                ownerController.LostArmies(armies);
                territory.RemoveArmy(armies);
                UpdateArmyDisplay();
            }
        }

        public void SetOwner(PlayerController owner)
        {
            ownerController = owner;
            territory.SetOwner(owner.GetPlayer());
            textName.SetColorPlayerText();
            SetTankSpriteColor();
        }

        private void SetTankSpriteColor() {

            Color color = ownerController.GetPlayerColor();
            if (ownerController.GetPlayerColor().Equals(Color.black)) {
                color = new Color32(70, 70, 70, 255);
            }
            GetComponentInChildren<SpriteRenderer>().color = color;
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
            if (ownerController == null || gameManager.CurrentPlayerController == null)
            {
                return false;
            }
            if (ownerController.Player.PlayerName == gameManager.CurrentPlayerController.Player.PlayerName)
            {
                return true;
            }
            return false;
        }

        private bool IsAttackableTerritory()
        {

            if (ownerController.Player.PlayerName == gameManager.CurrentPlayerController.Player.PlayerName)
            {
                return false;
            }

            if (gameManager.GetSelectedTerritory() == null)
            {
                return false;
            }


            if (gameManager.GetSelectedTerritory().GetTerritory().GetArmies() < GameSettings.MIN_DICE_TO_ATTACK)
            {
                return false;
            }

            return IsNeighbor();
        }

        private bool IsNeighbor()
        {

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
            }
        }

        private void StopAttack()
        {
            gameManager.SetDesinationTerritory(null);
        }

        private void MoveArmy()
        {
            if (gameManager.GetSelectedTerritory() != null && !gameManager.GetSelectedTerritory().Equals(this) && IsNeighbor())
            {
                gameManager.SetDesinationTerritory(this);
            }
        }

        public void UpdateTerritoryAtEndTurn()
        {
            territory.StartReinforceArmies = territory.Armies;
        }

        private void HandleClick(bool left)
        {
            bool isOwner = ownerController.Equals(gameManager.CurrentPlayerController);

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
                gameManager.SetSelectedTerritory(this);
            }
            else if (!isOwner && GameStatesController.IsAttack() && gameManager.GetSelectedTerritory() != null)
            {
                Attack();
            }
            if (isOwner && GameStatesController.IsMove())
            {
                //Deseleziono il territorio
                if (gameManager.GetSelectedTerritory() != null && gameManager.GetSelectedTerritory().Equals(this))
                {
                    gameManager.SetSelectedTerritory(null);
                    gameManager.SetDesinationTerritory(null);
                }
                else if (gameManager.GetSelectedTerritory() != null)
                {
                    MoveArmy();
                }
                else if (gameManager.GetSelectedTerritory() == null)
                {
                    gameManager.SetSelectedTerritory(this);
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

        public Territory GetTerritory()
        {
            return territory;
        }

        public PlayerController GetOwnerController()
        {
            return ownerController;
        }

        private void SplitSprite()
        {

            if (gameManager.GetDestinationTerritory() != null && gameManager.GetDestinationTerritory().Equals(this))
            {
                if (!hasAlternativeSprite)
                {
                    hasAlternativeSprite = true;
                    hasSelectedSprite = false;
                    GetComponent<Button>().image.sprite = alternativeSprite;
                }
                return;
            }
            if (gameManager.GetSelectedTerritory() != null && gameManager.GetSelectedTerritory().Equals(this))
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


    }
}