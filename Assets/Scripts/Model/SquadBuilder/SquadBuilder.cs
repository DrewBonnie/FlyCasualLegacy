﻿using Editions;
using Players;
using System;
using Upgrade;

namespace SquadBuilderNS
{
    public class SquadBuilder
    {
        //TODO: Hide
        public ContentDatabase Database;
        public SquadLists SquadLists;
        public SquadBuilderView View { get; set; }

        public PlayerNo CurrentPlayer;
        public SquadList CurrentSquad => SquadLists[CurrentPlayer];
        // TEMP???
        public SquadListShip CurrentShip { get; set; }
        public UpgradeSlot CurrentUpgradeSlot { get; set; }
        public string CurrentShipName { get; set; }

        //TEMP
        public static SquadBuilder Instance;

        public SquadBuilder()
        {
            Instance = this;

            GenerateDatabase();
            SquadLists = new SquadLists();

            CurrentPlayer = PlayerNo.Player1;
        }

        public void GenerateDatabase()
        {
            Database = new ContentDatabase(Edition.Current);
        }

        public void SetPlayers(string modeName)
        {
            SetPlayerTypesByMode(modeName);
            SetDefaultPlayerNames();
        }

        private void SetPlayerTypesByMode(string modeName)
        {
            switch (modeName)
            {
                case "vsAI":
                    SetPlayerTypes(typeof(HumanPlayer), typeof(AggressorAiPlayer));
                    break;
                case "Campaign":
                    SetPlayerTypes(typeof(HumanPlayer), typeof(AggressorAiPlayer));
                    break;
                case "Internet":
                    SetPlayerTypes(typeof(HumanPlayer), typeof(NetworkOpponentPlayer));
                    break;
                case "HotSeat":
                    SetPlayerTypes(typeof(HumanPlayer), typeof(HumanPlayer));
                    break;
                case "AIvsAI":
                    SetPlayerTypes(typeof(AggressorAiPlayer), typeof(AggressorAiPlayer));
                    break;
                case "Replay":
                    SetPlayerTypes(typeof(ReplayPlayer), typeof(ReplayPlayer));
                    break;
                default:
                    break;
            }
        }

        private void SetPlayerTypes(Type playerOneType, Type playerTwoType)
        {
            SquadLists[PlayerNo.Player1].PlayerType = playerOneType;
            SquadLists[PlayerNo.Player2].PlayerType = playerTwoType;
        }

        public void SetDefaultPlayerNames()
        {
            SquadLists[PlayerNo.Player1].GenerateDefaultNameForSquad();
            SquadLists[PlayerNo.Player2].GenerateDefaultNameForSquad();
        }

        public void SaveAutosaveSquadConfigurations()
        {
            if (Global.IsCampaignGame)
            {
                SquadLists[Tools.IntToPlayer(1)].SaveCampaignSquadronToFile("Campaign Autosave");
            }
            else
            {
                if (!DebugManager.DebugNetworkSingleDevice)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        try
                        {
                            // Autosaving feature, comment out here to remove
                            SquadLists[Tools.IntToPlayer(i + 1)].SaveSquadronToFile("Autosave " + (i + 1));
                        }
                        catch (Exception)
                        {
                            DebugManager.DebugNetworkSingleDevice = true;
                        }
                    }
                }
            }            
        }

        public void GenerateSavedConfigurationsLocal()
        {
            // TODO: Change to Enumerator
            foreach (SquadList squad in SquadLists.Squads.Values)
            {
                squad.SavedConfiguration = squad.GetSquadInJson();

                JSONObject playerInfoJson = new JSONObject();
                playerInfoJson.AddField("NickName", Options.NickName);
                playerInfoJson.AddField("Title", Options.Title);
                playerInfoJson.AddField("Avatar", Options.Avatar);
                squad.SavedConfiguration.AddField("PlayerInfo", playerInfoJson);
            }
        }

        public void ReGenerateSquads()
        {
            SquadLists.ReGenerateSquads();
        }
    }
}
