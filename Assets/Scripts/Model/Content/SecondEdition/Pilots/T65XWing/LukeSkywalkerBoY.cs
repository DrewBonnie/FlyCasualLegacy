﻿using ActionsList;
using BoardTools;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;
using Content;
using Abilities.SecondEdition;

namespace Ship
{
    namespace SecondEdition.T65XWing
    {
        public class LukeSkywalkerBoY : T65XWing
        {
            public LukeSkywalkerBoY() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Luke Skywalker",
                    5,
                    61,
                    isLimited: true,
                    abilityType: typeof(LukeSkywalkerAbility),
                    force: 2,
                    tags: new List<Tags>
                    {
                        Tags.BoY,
                        Tags.LightSide
                    },
                    extraUpgradeIcon: UpgradeType.ForcePower
                );
                ShipAbilities.Add(new HopeAbility());
                PilotNameCanonical = "lukeskywalker-battleofyavin-lsl";
                ModelInfo.SkinName = "Luke Skywalker";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class HopeAbility : GenericAbility
    {
        private GenericShip PreviousCurrentShip { get; set; }

        public override void ActivateAbility()
        {
            GenericShip.OnShipIsDestroyedGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnShipIsDestroyedGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship, bool flag)
        {
            if (!Tools.IsSameTeam(HostShip, ship) || Tools.IsSameShip(HostShip, ship)) return;
            
            DistanceInfo distanceInfo = new DistanceInfo(HostShip, ship);
            if (distanceInfo.Range > 3) return;

            RegisterAbilityTrigger(
                TriggerTypes.OnShipIsDestroyed,
                AskWhatToDo,
                customTriggerName: $"{HostShip.PilotInfo.PilotName} (ID: {HostShip.ShipId})"
            );
        }

        private void AskWhatToDo(object sender, EventArgs e)
        {
            PreviousCurrentShip = Selection.ThisShip;

            Selection.ChangeActiveShip(HostShip);
            CameraScript.RestoreCamera();
            Selection.ThisShip.AskPerformFreeAction(
                new List<GenericAction>()
                {
                    new FocusAction(){ HostShip = HostShip },
                    new BoostAction(){ HostShip = HostShip },
                },
                FinishAbility,
                descriptionShort: HostShip.PilotInfo.PilotName,
                descriptionLong: "You may perform a Focus or Boost action",
                imageHolder: HostShip
            );
        }

        private void FinishAbility()
        {
            Selection.ChangeActiveShip(PreviousCurrentShip);
            Triggers.FinishTrigger();
        }
    }
}