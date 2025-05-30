﻿using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.GauntletFighter
    {
        public class PreVizsla : GauntletFighter
        {
            public PreVizsla() : base()
            {
                PilotInfo = new PilotCardInfo
                (
                    "Pre Vizsla",
                    3,
                    61,
                    charges: 2,
                    regensCharges: 1,
                    pilotTitle: "Leader of Death Watch",
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.PreVizslaAbility),
                    tags: new List<Tags>
                    {
                        Tags.Mandalorian
                    },
                    extraUpgradeIcons: new List<UpgradeType>() { UpgradeType.Talent, UpgradeType.Illicit },
                    factionOverride: Faction.Separatists
                );

                ModelInfo.SkinName = "CIS Light";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PreVizslaAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += RegisterPreVizslaAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= RegisterPreVizslaAbility;
        }

        // Offensive portion
        private void RegisterPreVizslaAbility()
        {
            if(Combat.Defender.PilotInfo.Initiative>=HostShip.PilotInfo.Initiative)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackStart, ShowDecision);
            }
        }

        private void ShowDecision(object sender, System.EventArgs e)
        {
            if (HostShip.State.Charges > 1)
            {
                // give user the option to use ability
                AskToUseAbility(
                    HostShip.PilotInfo.PilotName,
                    AlwaysUseByDefault,
                    UseAbility,
                    descriptionLong: "Do you want ot spend 2 Charge to roll 1 additional attack die?",
                    imageHolder: HostShip
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice += PreVizslaAddAttackDice;
            HostShip.State.Charges -= 2; 
            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        private void PreVizslaAddAttackDice(ref int value)
        {
            Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": +1 attack die");
            value++;
            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice -= PreVizslaAddAttackDice;
        }
    }
}