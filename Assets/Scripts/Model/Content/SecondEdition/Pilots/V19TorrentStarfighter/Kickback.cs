﻿using Actions;
using ActionsList;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.V19TorrentStarfighter
{
    public class Kickback : V19TorrentStarfighter
    {
        public Kickback()
        {
            PilotInfo = new PilotCardInfo(
                "\"Kickback\"",
                4,
                27,
                true,
                abilityType: typeof(Abilities.SecondEdition.KickbackAbility),
                extraUpgradeIcon: UpgradeType.Talent
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you perform a barrel roll action, you may perform a red lock action.
    public class KickbackAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckConditions;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckConditions;
        }

        private void CheckConditions(GenericAction action)
        {
            if (action is BarrelRollAction)
            {
                HostShip.OnActionDecisionSubphaseEnd += PerformLockAction;
            }
        }

        private void PerformLockAction(GenericShip ship)
        {
            HostShip.OnActionDecisionSubphaseEnd -= PerformLockAction;

            RegisterAbilityTrigger(TriggerTypes.OnFreeAction, AskPerformBoostAction);
        }

        private void AskPerformBoostAction(object sender, System.EventArgs e)
        {
            HostShip.AskPerformFreeAction(
                new TargetLockAction() { Color = ActionColor.Red },
                Triggers.FinishTrigger,
                HostShip.PilotInfo.PilotName,
                "After you perform a Barrel Roll action, you may perform a red Lock action",
                HostShip
            );
        }
    }
}
