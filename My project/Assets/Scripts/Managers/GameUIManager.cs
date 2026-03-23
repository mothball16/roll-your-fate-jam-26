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
        public TMP_Text HealthText;
        public TMP_Text UpgradesText;
        public TMP_Text XPText;
        public TMP_Text EnemiesLeftText;
        public Image healthBar;

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
            if (_playerHumanoid != null && HealthText != null)
            {
                HealthText.text = $"Health: {_playerHumanoid.Health} / {_playerHumanoid.MaxHealth}";

                if (healthBar != null)
                {
                    float percent = _playerHumanoid.Health / _playerHumanoid.MaxHealth;
                    healthBar.fillAmount = percent;
                }
            }

            if (_gameController != null && XPText != null)
            {
                int xpLevel = _gameController.XPLevel;
                string reqStr = xpLevel < GameController.XPReqs.Length 
                    ? GameController.XPReqs[xpLevel].ToString() 
                    : "MAX";
                XPText.text = $"Level: {xpLevel}  |  XP: {_gameController.XP} / {reqStr}";

                if (UpgradesText != null && _gameController.bought != null)
                {
                    List<string> titles = new List<string>();
                    foreach (var u in _gameController.bought) titles.Add(u.Title);
                    UpgradesText.text = "Upgrades:\n" + string.Join("\n", titles);
                }

                if (EnemiesLeftText != null && LevelManager.Instance != null)
                {
                    EnemiesLeftText.text = $"Enemies Left: {LevelManager.Instance.EnemiesLeft}";
                }
            }
        }
    }
}
