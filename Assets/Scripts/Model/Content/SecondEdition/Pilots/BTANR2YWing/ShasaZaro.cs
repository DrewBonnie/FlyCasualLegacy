﻿using Upgrade;
using Ship;
using Tokens;
using BoardTools;
using System;
using System.Collections.Generic;
using SubPhases;

namespace Ship
{
    namespace SecondEdition.BTANR2YWing
    {
        public class ShasaZaro : BTANR2YWing
        {
            public ShasaZaro() : base()
            {
                PilotInfo = new PilotCardInfo
                (
                    "Shasa Zaro",
                    3,
                    32,
                    extraUpgradeIcon: UpgradeType.Talent,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ShasaZaroAbility)
                );

                ModelInfo.SkinName = "Red";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ShasaZaroAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsDefender += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsDefender -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if(HostShip.Tokens.GetTokensByColor(TokenColors.Green).Count > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, AskToSelectShip);
            }
        }

        private void AskToSelectShip(object sender, EventArgs e)
        {
            List<GenericShip> ships = Board.GetShipsInArcAtRange(HostShip, Arcs.ArcType.FullRear, new UnityEngine.Vector2(0, 2), Team.Type.Friendly);
            if (ships.Count > 0)
            {
                SelectTargetForAbility(
                    ShipIsSelected,
                    FilterTargets,
                    GetAiPriority,
                    HostShip.Owner.PlayerNo,
                    "Shasa Zaro",
                    "Choose a friendly ship in your full rear arc at range 0-2, it may gain a token matching one of your green tokens",
                    imageSource: HostShip
                );
            }
            else
            {
                //No ships in range
                Triggers.FinishTrigger();
            }
        }

        private int GetAiPriority(GenericShip ship)
        {
            return 0;
        }

        private bool FilterTargets(GenericShip ship)
        {
            return Tools.IsFriendly(ship, HostShip)
                && HostShip.SectorsInfo.IsShipInSector(ship, Arcs.ArcType.FullRear) 
                && FilterTargetsByRange(ship, 0, 2);
        }

        private void ShipIsSelected()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            SelectTokenToAssignToTarget();
        }

        private void SelectTokenToAssignToTarget()
        {
            SelectTokenToReassignSubphase subphase = Phases.StartTemporarySubPhaseNew<SelectTokenToReassignSubphase>(
                "Assign Token",
                Triggers.FinishTrigger
            );

            subphase.Name = HostShip.PilotInfo.PilotName;
            subphase.DescriptionShort = "Select a token to assign to the target";
            subphase.ImageSource = HostShip;

            subphase.DecisionOwner = HostShip.Owner;
            subphase.ShowSkipButton = true;

            foreach (GenericToken token in HostShip.Tokens.GetTokensByColor(TokenColors.Green))
            {
                if (GenericToken.SupportedTokenTypes.Contains(token.GetType()))
                {
                    subphase.AddDecision(
                        token.Name ,
                        delegate {
                            TargetShip.Tokens.AssignToken(token.GetType(), DecisionSubPhase.ConfirmDecision); }
                    );
                }
            }

            if (subphase.GetDecisions().Count > 0)
            {
                subphase.Start();
            }
            else
            {
                Phases.GoBack();
                Messages.ShowInfoToHuman("Shasa Zaro: No tokens to assign to the target");
                Triggers.FinishTrigger();
            }
        }

        private class SelectTokenToReassignSubphase : DecisionSubPhase { }
    }
}
