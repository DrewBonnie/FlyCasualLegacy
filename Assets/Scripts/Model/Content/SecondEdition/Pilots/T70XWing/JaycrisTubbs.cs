﻿using ActionsList;
using Ship;
using SubPhases;
using System;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T70XWing
    {
        public class JaycrisTubbs : T70XWing
        {
            public JaycrisTubbs() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Jaycris Tubbs",
                    1,
                    45,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.JaycrisTubbsAbility)
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class JaycrisTubbsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += JaycrisTubbsPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= JaycrisTubbsPilotAbility;
        }

        protected void JaycrisTubbsPilotAbility(GenericShip ship)
        {
            if (BoardTools.Board.IsOffTheBoard(ship)) return;
            if (ship.AssignedManeuver.ColorComplexity == Movement.MovementComplexity.Easy)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, SelectTargetForJaycrisTubbsAbility);
            }
        }

        private void SelectTargetForJaycrisTubbsAbility(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                RemoveStressToken,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                HostShip.PilotName,
                "Choose a friendly ship, it removes a stress token",
                HostShip
            );
        }

        private void RemoveStressToken()
        {
            TargetShip.Tokens.RemoveToken(typeof(StressToken), SelectShipSubPhase.FinishSelection);
        }

        private bool FilterTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, TargetTypes.AnyFriendly) && FilterTargetsByRange(ship, 0, 1) && ship.Tokens.HasToken(typeof(StressToken));
        }

        private int GetAiPriority(GenericShip ship)
        {
            int priority = 0;
            
            if (ship.Tokens.HasToken(typeof(StressToken))) priority += 100;

            priority += ship.PilotInfo.Cost;

            return priority;
        }
    }
}