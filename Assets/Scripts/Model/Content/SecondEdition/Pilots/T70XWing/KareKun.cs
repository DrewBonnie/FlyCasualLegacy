﻿using ActionsList;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T70XWing
    {
        public class KareKun : T70XWing
        {
            public KareKun() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Kare Kun",
                    4,
                    47,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.KareKunAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class KareKunAbility : Abilities.SecondEdition.DareDevilAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGetAvailableBoostTemplates += ChangeBoostTemplates;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableBoostTemplates -= ChangeBoostTemplates;
        }

        private void ChangeBoostTemplates(List<BoostMove> availableMoves, GenericAction action)
        {
            availableMoves.Add(new BoostMove(ActionsHolder.BoostTemplates.LeftTurn1, false));
            availableMoves.Add(new BoostMove(ActionsHolder.BoostTemplates.RightTurn1, false));
        }
    }
}