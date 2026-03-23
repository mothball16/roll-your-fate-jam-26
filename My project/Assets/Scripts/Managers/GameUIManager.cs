using Assets.Scripts.Utility;
using DangryGames;
using System;
using Assets.Scripts.Components;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



namespace Assets.Scripts.Managers
{
    class GameUIManager : MonoSingleton<GameUIManager>
    {
        public GameObject ShopUIPanel;
        public Button ShopUIPanel_Exit;
        public Transform ShopUIPanel_Main;
        public UpgradeData ShopUIPanel_SelectedUpgrade;
        public Action<UpgradeData> ShopExitCallback;

        public GameObject GameUIPanel;
        
        [Header("HUD")]
        public TMP_Text UpgradesText;
        public TMP_Text EnemiesLeftText;
        public Image healthBar;
        public Image XPbar;
        public Transform upgradesContainer; 
        public GameObject upgradeIconPrefab;

        [Header("Smootihng")]
        float currentHealthPercent;
        float currentXPPercent;
        public float smoothSpeed = 5f;


        private Humanoid _playerHumanoid;
        private GameController _gameController;

        public void Start()
        {
            _gameController = FindAnyObjectByType<GameController>();
            ShopUIPanel_Main = ShopUIPanel.transform.Find("Main");
            ShopUIPanel_Exit.onClick.AddListener(CloseShopUI);
            
        }

        private void DrawShop(List<UpgradeData> bought, List<UpgradeData> upgrades)
        {


            var origTemplate = ShopUIPanel.transform.Find("Template");
            origTemplate.gameObject.SetActive(false); // Make sure template stays hidden
            MouseUtils.ClearChildren(ShopUIPanel_Main);
            foreach (var upgrade in upgrades)
            {
                var template = Instantiate(origTemplate, ShopUIPanel_Main, false);
                Button btn = template.transform.GetComponent<Button>();
                template.gameObject.SetActive(true);

                template.Find("Title").GetComponent<TMP_Text>().text = upgrade.Title;
                template.Find("Desc").GetComponent<TMP_Text>().text = upgrade.Description;

                btn.gameObject.SetActive(true);


                if (ShopUIPanel_SelectedUpgrade == upgrade)
                {
                    template.GetComponent<Image>().color = new Color32(100, 255, 100, 255);
                } else if (bought.Contains(upgrade)){
                    template.GetComponent<Image>().color = new Color32(100, 100, 100, 255);
                    btn.gameObject.SetActive(false);
                }

                // could mem leak idk tho
                btn.onClick.AddListener(() =>
                {
                    ShopUIPanel_SelectedUpgrade = upgrade;
                    DrawShop(bought, upgrades);
                });
            }
        }

        public void OpenShopUI(List<UpgradeData> bought, List<UpgradeData> upgrades)
        {
            ShopUIPanel.SetActive(true);
            ShopUIPanel_SelectedUpgrade = null;
            DrawShop(bought, upgrades);
        }

        public void CloseShopUI()
        {
            ShopUIPanel.SetActive(false);
            ShopExitCallback?.Invoke(ShopUIPanel_SelectedUpgrade);
        }

        public void SetPlayer(GameObject player)
        {
            _playerHumanoid = player.GetComponent<Humanoid>();
        }

        public void Update()
        {
            if (_playerHumanoid != null && healthBar != null)
            {
                float targetHealth = (float)_playerHumanoid.Health / _playerHumanoid.MaxHealth;

                currentHealthPercent = Mathf.Lerp(currentHealthPercent, targetHealth, Time.deltaTime * smoothSpeed);

                healthBar.fillAmount = currentHealthPercent;
            }

            if (_gameController != null)
            {
                int xpLevel = _gameController.XPLevel;

                if (xpLevel < GameController.XPReqs.Length)
                {
                    float requiredXP = GameController.XPReqs[xpLevel];
                    float targetXP = _gameController.XP / requiredXP;

                    currentXPPercent = Mathf.Lerp(currentXPPercent, targetXP, Time.deltaTime * smoothSpeed);

                    XPbar.fillAmount = currentXPPercent;
                }
                else
                {
                    currentXPPercent = Mathf.Lerp(currentXPPercent, 1f, Time.deltaTime * smoothSpeed);
                    XPbar.fillAmount = currentXPPercent;
                }

                if (upgradesContainer != null && _gameController.bought != null)
                {
                    // Clear old icons
                    foreach (Transform child in upgradesContainer)
                    {
                        Destroy(child.gameObject);
                    }

                    // Create icons
                    foreach (var u in _gameController.bought)
                    {
                        GameObject icon = Instantiate(upgradeIconPrefab, upgradesContainer);

                        Image img = icon.GetComponent<Image>();
                        img.sprite = u.Icon; // IMPORTANT: your upgrade needs a Sprite
                    }
                }

                if (EnemiesLeftText != null && LevelManager.Instance != null)
                {
                    int enemies = LevelManager.Instance.EnemiesLeft;
                    EnemiesLeftText.text = $"Enemies: {ToRoman(enemies)}";
                }
            }
        }
        string ToRoman(int number)
        {
            if (number <= 0) return "0";

            string[] thousands = { "", "M", "MM", "MMM" };
            string[] hundreds = { "", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM" };
            string[] tens = { "", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC" };
            string[] ones = { "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX" };

            return thousands[number / 1000] +
                   hundreds[(number % 1000) / 100] +
                   tens[(number % 100) / 10] +
                   ones[number % 10];
        }
    }
}
