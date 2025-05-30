﻿using ActionsList;
using Obstacles;
using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class TierfonBellyRun : GenericUpgrade, IVariableCost
    {
        public TierfonBellyRun() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Tierfon Belly Run",
                UpgradeType.Talent,
                cost: 1,
                abilityType: typeof(Abilities.SecondEdition.TierfonBellyRunAbility),
                restriction: new ShipRestriction(
                    typeof(Ship.SecondEdition.BTLA4YWing.BTLA4YWing),
                    typeof(Ship.SecondEdition.BTLBYWing.BTLBYWing)
                )
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/e1/fc/e1fc1361-b9c1-4c44-8144-1bb7a16da4f3/swz85_upgrade_tierfonbellyrun.png";
        }

        public void UpdateCost(GenericShip ship)
        {
            Dictionary<int, int> initiativeToCost = new Dictionary<int, int>()
            {
                {0, 0},
                {1, 0},
                {2, 0},
                {3, 0},
                {4, 1},
                {5, 1},
                {6, 1}
            };

            UpgradeInfo.Cost = initiativeToCost[ship.PilotInfo.Initiative];
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TierfonBellyRunAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCheckObstacleDenyAttack += CheckAttackAllowAbility;
            HostShip.OnCheckIsForbiddenWeapon += DenyPrimaryOnAsteroid;

            GenericShip.OnTryAddAvailableDiceModificationGlobal += DenyAttackRerollWhileLanded;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckObstacleDenyAttack -= CheckAttackAllowAbility;
            HostShip.OnCheckIsForbiddenWeapon -= DenyPrimaryOnAsteroid;

            GenericShip.OnTryAddAvailableDiceModificationGlobal -= DenyAttackRerollWhileLanded;
        }

        private void CheckAttackAllowAbility(GenericObstacle obstacle, ref bool isAllowed)
        {
            if (obstacle is Asteroid) isAllowed = true;
        }

        private void DenyPrimaryOnAsteroid(GenericShip ship, IShipWeapon weapon, ref bool isDenied)
        {
            if (weapon.WeaponType == WeaponTypes.PrimaryWeapon
                && HostShip.ObstaclesLanded.Any(n => n is Asteroid))
            {
                isDenied = true;
            }
        }

        private void DenyAttackRerollWhileLanded(GenericShip ship, GenericAction diceModification, ref bool isAllowed)
        {
            if (Tools.IsSameShip(HostShip, Combat.Defender) && HostShip.ObstaclesLanded.Any(n => n is Asteroid))
            {
                if (diceModification.IsReroll) isAllowed = false;
            }
        }
    }
}