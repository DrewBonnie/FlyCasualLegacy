using Upgrade;
using System.Collections.Generic;
using ActionsList;
using Ship;

namespace UpgradesList.SecondEdition
{
    public class DutchVanderPilotAbility : GenericUpgrade
    {
        public DutchVanderPilotAbility() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Dutch Vander Pilot Ability",
                UpgradeType.Pilot,

                cost: 8,
                restriction: new StatValueRestriction(
                        StatValueRestriction.Stats.Initiative,
                        StatValueRestriction.Conditions.HigherThanOrEqual,
                        4
                    ),
                abilityType: typeof(Abilities.SecondEdition.DutchVanderAbility)
            );
            ImageUrl = "https://raw.githubusercontent.com/sampson-matt/Hotac-Upgrade-Cards/main/PilotAbilities/Rebel/dutchvander.png";
        }


    }
}