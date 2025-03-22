﻿using Abilities.SecondEdition;
using Arcs;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BoardTools;
using Content;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLA4YWing
    {
        public class HolOkandBoYSL : BTLA4YWing
        {
            public HolOkandBoYSL() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Hol Okand",
                    4,
                    49,
                    pilotTitle: "Battle of Yavin",
                    isLimited: true,
                    abilityType: typeof(HolOkandBoYAbility),
                    tags: new List<Tags>
                    {
                        Tags.BoY
                    },
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Turret,
                        UpgradeType.Torpedo,
                        UpgradeType.Astromech
                    },
                    isStandardLayout: true
                );
                ShipAbilities.Add(new HopeAbility());

                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.DorsalTurret));
                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.AdvProtonTorpedoes));
                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.PreciseAstromech));

                PilotNameCanonical = "holokand-battleofyavin";
            }
        }
    }
}