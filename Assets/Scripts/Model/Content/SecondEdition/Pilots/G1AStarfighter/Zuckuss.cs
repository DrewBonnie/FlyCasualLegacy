﻿using Upgrade;

namespace Ship
{
    namespace SecondEdition.G1AStarfighter
    {
        public class Zuckuss : G1AStarfighter
        {
            public Zuckuss() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Zuckuss",
                    3,
                    42,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.ZuckussAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}