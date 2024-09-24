﻿using Actions;
using ActionsList;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.V19TorrentStarfighter
{
    public class Swoop : V19TorrentStarfighter
    {
        public Swoop()
        {
            PilotInfo = new PilotCardInfo(
                "\"Swoop\"",
                3,
                26,
                true,
                abilityType: typeof(Abilities.SecondEdition.SwoopAbility)
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    //After a friendly small or medium ship fully executes a speed 3-4 maneuver, if it is at range 0-1, it may perform a red boost action.
    public class SwoopAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnMovementFinishSuccessfullyGlobal += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnMovementFinishSuccessfullyGlobal -= RegisterTrigger;
        }

        private void RegisterTrigger(GenericShip ship)
        {
            if (Tools.IsFriendly(ship, HostShip)
                && (ship.ShipInfo.BaseSize == BaseSize.Small || ship.ShipInfo.BaseSize == BaseSize.Medium)
                && (ship.AssignedManeuver.Speed == 3 || ship.AssignedManeuver.Speed == 4)
                && new BoardTools.DistanceInfo(ship, HostShip).Range <= 1)
            {
                TargetShip = ship;
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskPerformBoostAction);
            }
        }

        private void AskPerformBoostAction(object sender, System.EventArgs e)
        {
            TargetShip.AskPerformFreeAction(
                new List<GenericAction>() { new BoostAction() { Color = ActionColor.Red } },
                Triggers.FinishTrigger,
                HostShip.PilotInfo.PilotName,
                "You perform a red Boost action",
                HostShip
            );
        }
    }
}
