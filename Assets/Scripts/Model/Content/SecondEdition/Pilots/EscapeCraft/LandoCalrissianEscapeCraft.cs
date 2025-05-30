﻿using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.EscapeCraft
    {
        public class LandoCalrissianEscapeCraft : EscapeCraft
        {
            public LandoCalrissianEscapeCraft() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Lando Calrissian",
                    4,
                    27,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.LandoCalrissianScumPilotAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                PilotNameCanonical = "landocalrissian-escapecraft";

                ShipAbilities.Add(new Abilities.SecondEdition.CoPilotAbility());
            }
        }
    }
}