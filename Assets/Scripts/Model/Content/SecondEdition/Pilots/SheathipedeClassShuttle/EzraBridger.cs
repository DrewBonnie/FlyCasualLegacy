﻿using System.Collections;
using System.Collections.Generic;
using Upgrade;
using Content;

namespace Ship
{
    namespace SecondEdition.SheathipedeClassShuttle
    {
        public class EzraBridger : SheathipedeClassShuttle
        {
            public EzraBridger() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Ezra Bridger",
                    3,
                    37,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.EzraBridgerPilotAbility),
                    tags: new List<Tags>
                    {
                        Tags.LightSide,
                        Tags.Spectre
                    },
                    force: 1,
                    extraUpgradeIcon: UpgradeType.ForcePower
                );

                PilotNameCanonical = "ezrabridger-sheathipedeclassshuttle";
            }
        }
    }
}
