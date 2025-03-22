﻿using Abilities.SecondEdition;
using SubPhases;
using Upgrade;
using Ship;
using System.Linq;
using Content;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.TIEInterceptor
    {
        public class IdenVersioBoYSL : TIEInterceptor
        {
            public IdenVersioBoYSL() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Iden Versio",
                    4,
                    67,
                    isLimited: true,
                    charges: 2,
                    regensCharges: 1,
                    abilityType: typeof(IdenVersioBoYAbility),
                    tags: new List<Tags>
                    {
                        Tags.BoY
                    },
                     extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Modification
                    },
                    isStandardLayout: true
                );
                ShipInfo.Shields++;
                AutoThrustersAbility oldAbility = (AutoThrustersAbility)ShipAbilities.First(n => n.GetType() == typeof(AutoThrustersAbility));
                ShipAbilities.Remove(oldAbility);
                ShipAbilities.Add(new SensitiveControlsRealAbility());

                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.Predator));
                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.Fanatic));

                PilotNameCanonical = "idenversio-battleofyavin";
            }
        }
    }
}