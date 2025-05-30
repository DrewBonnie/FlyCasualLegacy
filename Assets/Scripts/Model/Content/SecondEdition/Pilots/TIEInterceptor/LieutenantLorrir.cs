﻿using BoardTools;
using Movement;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEInterceptor
    {
        public class LieutenantLorrir : TIEInterceptor
        {
            public LieutenantLorrir() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Lieutenant Lorrir",
                    3,
                    37,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.LieutenantLorrirAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    abilityText: "While you barrel roll, you may use bank templates, instead of straight template"
                );

                ModelInfo.SkinName = "Skystrike Academy";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LieutenantLorrirAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGetAvailableBarrelRollTemplates += ChangeBarrelRollTemplates;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableBarrelRollTemplates -= ChangeBarrelRollTemplates;
        }

        private void ChangeBarrelRollTemplates(List<ManeuverTemplate> availableTemplates)
        {
            availableTemplates.Add(new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Left, ManeuverSpeed.Speed1));
            availableTemplates.Add(new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Right, ManeuverSpeed.Speed1));
            availableTemplates.RemoveAll(n => n.Name == "Straight 1");
        }
    }
}