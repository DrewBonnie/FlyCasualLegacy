﻿using Upgrade;

namespace Ship
{
    namespace SecondEdition.AlphaClassStarWing
    {
        public class LieutenantKarsabi : AlphaClassStarWing
        {
            public LieutenantKarsabi() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Lieutenant Karsabi",
                    3,
                    33,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.LieutenantKarsabiAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}
