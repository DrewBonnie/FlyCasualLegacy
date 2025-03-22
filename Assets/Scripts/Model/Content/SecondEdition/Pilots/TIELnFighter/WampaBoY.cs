﻿using Upgrade;
using Abilities.SecondEdition;
using Actions;
using ActionsList;
using Content;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class WampaBoY : TIELnFighter
        {
            public WampaBoY() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Wampa\"",
                    1,
                    31,
                    isLimited: true,
                    abilityType: typeof(WampaAbility),
                    charges: 1,
                    tags: new List<Tags>
                    {
                        Tags.BoY
                    },
                    regensCharges: 1
                );
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(TargetLockAction)));
                ShipInfo.Hull++;
                ShipInfo.UpgradeIcons.Upgrades.Remove(UpgradeType.Modification);
                PilotNameCanonical = "wampa-battleofyavin-lsl";
            }
        }
    }
}