﻿using Abilities.SecondEdition;
using BoardTools;
using Upgrade;
using Content;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.BTLA4YWing
    {
        public class DexTireeBoY : BTLA4YWing
        {
            public DexTireeBoY() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Dex Tiree",
                    2,
                    31,
                    isLimited: true,
                    abilityType: typeof(DexTireeAbility),
                    tags: new List<Tags>
                    {
                        Tags.BoY
                    },
                    extraUpgradeIcon: UpgradeType.Modification
                );
                ShipAbilities.Add(new HopeAbility());
                PilotNameCanonical = "dextiree-battleofyavin-lsl";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DexTireeAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotStartAsDefender += CheckConditionsDefense;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotStartAsDefender -= CheckConditionsDefense;
        }

        private void CheckConditionsDefense()
        {
            if (Board.GetShipsAtRange(HostShip, new UnityEngine.Vector2(0, 1), Team.Type.Friendly).Count > 1)
            {
                HostShip.AfterGotNumberOfDefenceDice += RollExtraDefenseDice;
            }
        }

        private void RollExtraDefenseDice(ref int count)
        {
            count++;
            Messages.ShowInfo(HostShip.PilotInfo.PilotName + " is within range 1 of another friendly ship and gains +1 defense die");
            HostShip.AfterGotNumberOfDefenceDice -= RollExtraDefenseDice;
        }
    }
}