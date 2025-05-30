﻿using Editions;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SquadBuilderNS
{
    public enum FactionSize
    {
        Small4, // Not used anymore
        Medium6, // Not used anymore
        Medium8,
        Large20
    }

    public class SquadBuilderShipsView
    {
        private int AvailableShipsCounter { get; set; }

        public void ShowAvailableShips(Faction faction)
        {
            AvailableShipsCounter = 0;

            SquadBuilderView.DestroyChildren(GameObject.Find("UI/Panels/SelectShipPanel/Panel").transform);

            bool isAnyShipShown = false;
            ShipPanelSquadBuilder.WaitingToLoad = 0;

            foreach (ShipRecord ship in Global.SquadBuilder.Database.AllShips.OrderBy(s => s.Instance.ShipInfo.ShipName))
            {
                if (ship.Instance.ShipInfo.FactionsAll.Contains(faction) && !ship.Instance.IsHidden && HasPilots(ship, faction))
                {
                    if (ship.Instance.GetType().ToString().Contains(Edition.Current.NameShort))
                    {
                        ShowAvailableShip(ship, faction);
                        isAnyShipShown = true;
                    }
                }
            }

            if (!isAnyShipShown)
            {
                SquadBuilderView.ShowNoContentInfo("Ship");
            }
            else
            {
                ScaleSelectShipPanel(faction);
            }
        }

        private void ScaleSelectShipPanel(Faction faction)
        {
            string prefabPath = (GetFactionSize(faction) == FactionSize.Large20) ? "Prefabs/SquadBuilder/ShipPanelBig" : "Prefabs/SquadBuilder/ShipPanel";
            GameObject prefab = (GameObject)Resources.Load(prefabPath, typeof(GameObject));
            GridLayoutGroup grid = GameObject.Find("UI/Panels/SelectShipPanel/Panel").GetComponentInChildren<GridLayoutGroup>();
            grid.cellSize = prefab.GetComponent<RectTransform>().sizeDelta;

            switch (GetFactionSize(faction))
            {
                case FactionSize.Large20:
                    grid.constraintCount = 5;
                    break;
                case FactionSize.Medium8:
                    grid.constraintCount = 4;
                    break;
                case FactionSize.Medium6:
                    grid.constraintCount = 3;
                    break;
                case FactionSize.Small4:
                    grid.constraintCount = 2;
                    break;
            }

            float panelWidth = grid.constraintCount * (grid.cellSize.x + 25) + 25;
            int rowsCount = AvailableShipsCounter / grid.constraintCount;
            if (AvailableShipsCounter - rowsCount * grid.constraintCount != 0) rowsCount++;
            float panelHeight = (rowsCount) * (grid.cellSize.y + 25) + 25;

            GameObject selechShipPanelGO = GameObject.Find("UI/Panels/SelectShipPanel/Panel");
            selechShipPanelGO.GetComponent<RectTransform>().sizeDelta = new Vector2(panelWidth, panelHeight);
            MainMenu.ScalePanel(selechShipPanelGO.transform);
        }

        private void ShowAvailableShip(ShipRecord ship, Faction faction)
        {
            string prefabPath = (GetFactionSize(faction) == FactionSize.Large20) ? "Prefabs/SquadBuilder/ShipPanelBig" : "Prefabs/SquadBuilder/ShipPanel";
            GameObject prefab = (GameObject)Resources.Load(prefabPath, typeof(GameObject));
            GameObject newShipPanel = MonoBehaviour.Instantiate(prefab, GameObject.Find("UI/Panels/SelectShipPanel/Panel").transform);

            ShipPanelSquadBuilder script = newShipPanel.GetComponent<ShipPanelSquadBuilder>();
            script.ImageUrl = GetImageOfIconicPilot(ship);
            script.ShipName = ship.Instance.ShipInfo.ShipName;
            script.FullType = ship.Instance.ShipInfo.ShipName;

            /*int rowsCount = (IsSmallFaction(faction)) ? SHIP_COLUMN_COUNT_SMALLFACTION : SHIP_COLUMN_COUNT;
            int row = availableShipsCounter / rowsCount;
            int column = availableShipsCounter - (row * rowsCount);

            if (IsSmallFaction(faction))
            {
                newShipPanel.transform.localPosition = new Vector3(210 + column * PILOT_CARD_WIDTH * 2 + 50 * (column), -(DISTANCE_MEDIUM + row * 368 + DISTANCE_MEDIUM * 2 * (row)), 0);
            }
            else
            {
                newShipPanel.transform.localPosition = new Vector3(25 + column * PILOT_CARD_WIDTH + 25 * (column), - (DISTANCE_MEDIUM + row * 184 + DISTANCE_MEDIUM * (row)), 0);                
            }*/

            AvailableShipsCounter++;
        }

        private FactionSize GetFactionSize(Faction faction)
        {
            switch (faction)
            {
                case Faction.Rebel:
                    return FactionSize.Large20;
                case Faction.Imperial:
                    return FactionSize.Large20;
                case Faction.Scum:
                    return FactionSize.Large20;
                case Faction.Resistance:
                    return FactionSize.Medium8;
                case Faction.FirstOrder:
                    return FactionSize.Medium8;
                case Faction.Republic:
                    return FactionSize.Medium8;
                case Faction.Separatists:
                    return FactionSize.Medium8;
                default:
                    return FactionSize.Large20;
            }
        }

        private string GetImageOfIconicPilot(ShipRecord ship)
        {
            string image = null;

            if (ship.Instance.IconicPilots != null)
            {
                image = Global.SquadBuilder.Database.AllPilots.Find(n => n.PilotTypeName == ship.Instance.IconicPilots[Global.SquadBuilder.CurrentSquad.SquadFaction].ToString()).Instance.ImageUrl;
            }

            return image;
        }

        private bool HasPilots(ShipRecord ship, Faction faction)
        {

            if (Global.IsCampaignGame)
            {
               return Global.SquadBuilder.Database.AllPilots
                .Any(n =>
                    n.Ship == ship
                    && n.PilotFaction == faction
                    && n.Instance.GetType().ToString().Contains(Edition.Current.NameShort)
                    && n.PilotName.ToString().Contains("Hotac")
                    && !n.Instance.IsHiddenSquadbuilderOnly
                );
            }
            else
            {
                return Global.SquadBuilder.Database.AllPilots
                .Any(n =>
                    n.Ship == ship
                    && n.PilotFaction == faction
                    && n.Instance.GetType().ToString().Contains(Edition.Current.NameShort)
                    && !n.Instance.IsHiddenSquadbuilderOnly
                );
            }
            //return Global.SquadBuilder.Database.AllPilots.Any(n => n.Ship == ship && n.PilotFaction == faction);
        }
    }
}
