﻿using Ship;
using Upgrade;
using Arcs;
using ActionsList;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class Finn : GenericUpgrade
    {
        public Finn() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Finn",
                UpgradeType.Gunner,
                cost: 8,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Resistance),
                abilityType: typeof(Abilities.SecondEdition.FinnAbility)
            );

            Avatar = new AvatarInfo(
                Faction.Resistance,
                new Vector2(349, 1)
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class FinnAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += FinnActionEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= FinnActionEffect;
        }

        private void FinnActionEffect(GenericShip host)
        {
            GenericAction newAction = new ActionsList.SecondEdition.FinnDiceModification()
            {
                HostShip = host,
                ImageUrl = HostUpgrade.ImageUrl
            };
            host.AddAvailableDiceModificationOwn(newAction);
        }
    }
}

namespace ActionsList.SecondEdition
{
    public class FinnDiceModification : ActionsList.FirstEdition.FinnDiceModification
    {
        protected override bool CheckArcRequirements(GenericShip thisShip, GenericShip anotherShip)
        {
            return thisShip.SectorsInfo.IsShipInSector(anotherShip, ArcType.Front);
        }
    }
}