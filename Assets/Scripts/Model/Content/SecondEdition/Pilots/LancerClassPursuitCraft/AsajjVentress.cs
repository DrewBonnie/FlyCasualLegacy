﻿using Arcs;
using BoardTools;
using Content;
using SubPhases;
using System.Collections.Generic;
using Tokens;
using Upgrade;
using Ship;

namespace Ship
{
    namespace SecondEdition.LancerClassPursuitCraft
    {
        public class AsajjVentress : LancerClassPursuitCraft
        {
            public AsajjVentress() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Asajj Ventress",
                    4,
                    66,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.AsajjVentressPilotAbility),
                    tags: new List<Tags>
                    {
                        Tags.DarkSide,
                        Tags.BountyHunter
                    },
                    force: 2,
                    extraUpgradeIcon: UpgradeType.ForcePower
                );

                ModelInfo.SkinName = "Asajj";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AsajjVentressPilotAbility : Abilities.FirstEdition.AsajjVentressPilotAbility
    {

        protected override void TryRegisterAsajjVentressPilotAbility()
        {
            if (TargetsForAbilityExist(FilterTargetsOfAbility) && HostShip.State.Force > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskSelectShip);
            }
        }

        protected override void AskSelectShip(object sender, System.EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);

            SelectTargetForAbility(
                CheckAssignStress,
                FilterTargetsOfAbility,
                GetAiPriorityOfTarget,
                HostShip.Owner.PlayerNo,
                HostShip.PilotInfo.PilotName,
                "Choose a ship inside your mobile firing arc to assign Stress token to it",
                HostShip
            );
        }

        private void CheckAssignStress()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();
            AsajjVentressAbilityDecisionSubPhaseSE subphase = (AsajjVentressAbilityDecisionSubPhaseSE)
                Phases.StartTemporarySubPhaseNew(
                "Choose effect of " + HostShip.PilotInfo.PilotName + "' ability.",
                typeof(AsajjVentressAbilityDecisionSubPhaseSE),
                Triggers.FinishTrigger
            );

            Selection.ThisShip = TargetShip;
            Selection.ActiveShip = HostShip;
            subphase.SourceShip = HostShip;
            subphase.Start();
        }
    }
}

namespace SubPhases
{
    public class AsajjVentressAbilityDecisionSubPhaseSE : RemoveGreenTokenDecisionSubPhase
    {
        public GenericShip SourceShip;

        public override void PrepareCustomDecisions()
        {
            DescriptionShort = "Asajj Ventress";
            DescriptionLong = Selection.ThisShip.ShipId + ": " + "Select the effect of Asajj Ventress' ability.";
            ImageSource = SourceShip;

            DecisionOwner = Selection.ThisShip.Owner;
            DefaultDecisionName = "Recieve a stress token.";

            AddDecision("Recieve a stress token.", RecieveStress);
        }

        private void RecieveStress(object sender, System.EventArgs e)
        {
            SourceShip.State.SpendForce(
                1,
                delegate { Selection.ThisShip.Tokens.AssignToken(typeof(StressToken), DecisionSubPhase.ConfirmDecision); }
            );
        }
    }
}