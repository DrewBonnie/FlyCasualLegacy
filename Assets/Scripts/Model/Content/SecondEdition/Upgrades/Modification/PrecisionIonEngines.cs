﻿using Upgrade;
using Ship;
using SubPhases;
using Movement;
using UnityEngine;
using System.Collections.Generic;

namespace UpgradesList.SecondEdition
{
    public class PrecisionIonEngines : GenericUpgrade, IVariableCost
    {
        public PrecisionIonEngines() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Precision Ion Engines",
                UpgradeType.Modification,
                cost: 2,
                charges: 2,
                abilityType: typeof(Abilities.SecondEdition.PrecisionIonEnginesAbility),
                restriction: new TagRestriction(Content.Tags.Tie)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/bb/fb/bbfb4727-e2f5-4f23-be9a-3341ea4de7b5/swz80_upgrade_precison-ion-engines.png";
        }

        public void UpdateCost(GenericShip ship)
        {
            Dictionary<int, int> initiativeToCost = new Dictionary<int, int>()
            {
                {0, 1},
                {1, 1},
                {2, 1},
                {3, 1},
                {4, 1},
                {5, 2},
                {6, 2}
            };

            UpgradeInfo.Cost = initiativeToCost[ship.PilotInfo.Initiative];
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PrecisionIonEnginesAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsRevealed += RegisterAskChangeManeuver;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsRevealed -= RegisterAskChangeManeuver;
        }

        private void RegisterAskChangeManeuver(GenericShip ship)
        {
            if (HostShip.RevealedManeuver != null
                && HostShip.RevealedManeuver.Bearing == ManeuverBearing.KoiogranTurn
                && HostShip.RevealedManeuver.Speed >= 1
                && HostShip.RevealedManeuver.Speed <= 3
                && HostUpgrade.State.Charges > 0
            )
            {
                RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, AskChangeManeuver);
            }
        }

        private void AskChangeManeuver(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                "Precision Ion Engines",
                NeverUseByDefault,
                ChangeManeuver,
                descriptionLong: "Do you want to spend 1 charge to execute Segnor's Loop instead?",
                imageHolder: HostUpgrade,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void ChangeManeuver(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostUpgrade.State.SpendCharge();

            HostShip.Maneuvers.Add($"{HostShip.RevealedManeuver.Speed}.L.R", MovementComplexity.Complex);
            HostShip.Maneuvers.Add($"{HostShip.RevealedManeuver.Speed}.R.R", MovementComplexity.Complex);

            HostShip.OnMovementFinish += RemoveAddedManeuvers;

            HostShip.Owner.ChangeManeuver(
                ShipMovementScript.SendAssignManeuverCommand,
                Triggers.FinishTrigger,
                IsSameSegnorsLoop
            );
        }

        private void RemoveAddedManeuvers(GenericShip ship)
        {
            HostShip.OnMovementFinish -= RemoveAddedManeuvers;

            HostShip.Maneuvers.Remove($"{HostShip.RevealedManeuver.Speed}.L.R");
            HostShip.Maneuvers.Remove($"{HostShip.RevealedManeuver.Speed}.R.R");
        }

        private bool IsSameSegnorsLoop(string maneuverString)
        {
            bool result = false;
            
            ManeuverHolder movementStruct = new ManeuverHolder(maneuverString);
            
            if (movementStruct.Speed == HostShip.RevealedManeuver.ManeuverSpeed
                && movementStruct.Bearing == ManeuverBearing.SegnorsLoop
            )
            {
                result = true;
            }

            return result;
        }
    }
}