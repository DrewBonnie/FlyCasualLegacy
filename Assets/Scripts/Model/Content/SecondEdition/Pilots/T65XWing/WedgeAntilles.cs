﻿using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T65XWing
    {
        public class WedgeAntilles : T65XWing
        {
            public WedgeAntilles() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Wedge Antilles",
                    6,
                    57,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.WedgeAntillesAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ModelInfo.SkinName = "Wedge Antilles";
            }
        }
    }
}
