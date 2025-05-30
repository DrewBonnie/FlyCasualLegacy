﻿using Arcs;
using BoardTools;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESfFighter
    {
        public class CaptainPhasma : TIESfFighter
        {
            public CaptainPhasma() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Captain Phasma",
                    4,
                    34,
                    isLimited: true,
                    extraUpgradeIcon: UpgradeType.Talent,
                    abilityType: typeof(Abilities.SecondEdition.CaptainPhasmaPilotAbility)
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    // While you defend, after the Neutralize Results step, another friendly ship at range 0-1 must suffer
    // 1 hit/crit damage to cancel 1 matching result.

    public class CaptainPhasmaPilotAbility : Abilities.SecondEdition.PrinceXizorAbility
    {
        protected override bool CheckInArcRequirements(GenericShip ship)
        {
            // No arc requirement
            return true;
        }

        protected override bool IsShowSkipButton()
        {
            // Cannot be skipped
            return false;
        }
    }
}
