﻿using BoardTools;
using Ship;
using SubPhases;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ModifiedTIELnFighter
    {
        public class ForemanProach : ModifiedTIELnFighter
        {
            public ForemanProach() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Foreman Proach",
                    4,
                    27,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ForemanProachAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ForemanProachAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCombatActivation += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCombatActivation -= RegisterAbility;
        }

        private void RegisterAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatActivation, Ability);
        }

        private void Ability(object sender, System.EventArgs e)
        {
            if (TargetsForAbilityExist(FilterAbilityTarget))
            {
                Selection.ChangeActiveShip(HostShip);
                Messages.ShowInfoToHuman("Foreman Proach: Select a ship to gain 1 tractor token");

                SelectTargetForAbility(
                    SelectAbilityTarget,
                    FilterAbilityTarget,
                    GetAiAbilityPriority,
                    HostShip.Owner.PlayerNo,
                    HostShip.PilotInfo.PilotName,
                    "Select a ship to gain 1 tractor token and gain 1 disarm token",
                    HostShip
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        protected virtual bool FilterAbilityTarget(GenericShip ship)
        {
            return
                FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.Enemy }) &&
                FilterTargetsByRange(ship, 1, 2) &&
                Board.IsShipInArcByType(HostShip, ship, Arcs.ArcType.Bullseye);
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            int result = 0;
            //int shipFocusTokens = ship.Tokens.CountTokensByType(typeof(FocusToken));
            //if (shipFocusTokens == 0) result += 100;
            //result += (5 - shipFocusTokens);
            return result;
        }

        protected virtual void SelectAbilityTarget()
        {
            Tokens.TractorBeamToken token = new Tokens.TractorBeamToken(TargetShip, HostShip.Owner);
            HostShip.Tokens.AssignToken(
                typeof(Tokens.WeaponsDisabledToken),
                delegate {
                    TargetShip.Tokens.AssignToken(token, SelectShipSubPhase.FinishSelection);
                }
            );
        }
    }
}