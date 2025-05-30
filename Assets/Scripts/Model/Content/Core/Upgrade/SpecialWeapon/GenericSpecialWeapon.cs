﻿using System;
using System.Collections;
using System.Collections.Generic;
using Ship;
using System.Linq;
using Tokens;
using BoardTools;
using Upgrade;
using SubPhases;

namespace Upgrade
{

    public class GenericSpecialWeapon : GenericUpgrade, IShipWeapon
    {
        public string Name { get { return UpgradeInfo.Name; } }

        public SpecialWeaponInfo WeaponInfo { get {return UpgradeInfo.WeaponInfo ; } }

        public WeaponTypes WeaponType
        {
            get
            {
                WeaponTypes weaponType = WeaponTypes.PrimaryWeapon;

                if (UpgradeInfo.HasType(UpgradeType.Cannon))
                {
                    weaponType = WeaponTypes.Cannon;
                }
                else if (UpgradeInfo.HasType(UpgradeType.Missile))
                {
                    weaponType = WeaponTypes.Missile;
                }
                else if (UpgradeInfo.HasType(UpgradeType.Torpedo))
                {
                    weaponType = WeaponTypes.Torpedo;
                }
                else if (UpgradeInfo.HasType(UpgradeType.Turret))
                {
                    weaponType = WeaponTypes.Turret;
                }
                else if (UpgradeInfo.HasType(UpgradeType.Illicit))
                {
                    weaponType = WeaponTypes.Illicit;
                }
                else if (UpgradeInfo.HasType(UpgradeType.Talent))
                {
                    weaponType = WeaponTypes.Talent;
                }
                else if (UpgradeInfo.HasType(UpgradeType.ForcePower))
                {
                    weaponType = WeaponTypes.Force;
                }

                return weaponType;
            }
        }

        public GenericSpecialWeapon() : base()
        {

        }

        public virtual bool IsShotAvailable(GenericShip targetShip)
        {
            bool result = true;

            int MinRangeUpdated = WeaponInfo.MinRange;
            int MaxRangeUpdated = WeaponInfo.MaxRange;
            HostShip.CallUpdateWeaponRange(this, ref MinRangeUpdated, ref MaxRangeUpdated, targetShip);

            if (!State.IsFaceup) return false;

            if (State.UsesCharges && State.Charges == 0) return false;

            ShotInfo shotInfo = new ShotInfo(HostShip, targetShip, this);
            int range = shotInfo.Range;

            if (!shotInfo.IsShotAvailable) return false;

            if (range < MinRangeUpdated) return false;
            if (range > MaxRangeUpdated) return false;

            if (!AreTokenRequirementsMet(targetShip)) return false;

            return result;
        }

        private bool AreTokenRequirementsMet(GenericShip targetShip)
        {
            List<Type> tokenRequirements = HostShip.GetWeaponAttackRequirement(this, isSilent: true);

            if (tokenRequirements.Count > 0)
            {
                foreach (Type tokenRequirement in tokenRequirements)
                {
                    if (tokenRequirement == typeof(BlueTargetLockToken))
                    {
                        List<GenericToken> waysToPay = new List<GenericToken>();

                        List<char> letters = ActionsHolder.GetTargetLocksLetterPairs(HostShip, targetShip);
                        GenericToken targetLockToken = HostShip.Tokens.GetToken(typeof(BlueTargetLockToken), letters.FirstOrDefault());
                        if (targetLockToken != null) waysToPay.Add(targetLockToken);

                        HostShip.CallOnGenerateAvailableAttackPaymentList(waysToPay);

                        if (waysToPay.Count != 0) return true;
                    }
                    else
                    {
                        if (HostShip.Tokens.HasToken(tokenRequirement)) return true;
                    }
                }
            }
            else
            {
                return true;
            }

            return false;
        }

        public virtual void PayAttackCost(Action callBack)
        {
            PayDiscardCost(delegate { PayTokenCost(callBack); });
        }

        private void PayDiscardCost(Action callBack)
        {
            if (WeaponInfo.Discard)
            {
                TryDiscard(callBack);
            }
            else if (WeaponInfo.Charges > 0)
            {
                State.SpendCharge();
                callBack();
            }
            else
            {
                callBack();
            };
        }

        private void PayTokenCost(Action callBack)
        {
            List<Type> tokenRequirements = HostShip.GetWeaponAttackRequirement(this, isSilent: false);

            if (tokenRequirements.Contains(typeof(BlueTargetLockToken)))
            {
                List<GenericToken> waysToPay = new List<GenericToken>();

                List<char> letters = ActionsHolder.GetTargetLocksLetterPairs(Combat.Attacker, Combat.Defender);
                GenericToken targetLockToken = Combat.Attacker.Tokens.GetToken(typeof(BlueTargetLockToken), letters.FirstOrDefault());
                if (targetLockToken != null) waysToPay.Add(targetLockToken);

                Combat.Attacker.CallOnGenerateAvailableAttackPaymentList(waysToPay);

                if (waysToPay.Count == 1)
                {
                    if (WeaponInfo.SpendsToken == typeof(BlueTargetLockToken) || waysToPay.First() is ForceToken)
                    {
                        Combat.Attacker.Tokens.SpendToken(
                            waysToPay.First().GetType(),
                            callBack,
                            (waysToPay.First() as BlueTargetLockToken != null) ? (waysToPay.First() as BlueTargetLockToken).Letter : ' '
                        );
                    }
                    else
                    {
                        callBack();
                    }
                }
                else
                {
                    if (WeaponInfo.SpendsToken != null)
                    {
                        PayAttackCostDecisionSubPhase subphase = Phases.StartTemporarySubPhaseNew<PayAttackCostDecisionSubPhase>(
                            "Choose how to pay attack cost",
                            callBack
                        );
                        subphase.Weapon = this;
                        subphase.Start();
                    }
                    else
                    {
                        callBack();
                    }
                }
            }
            else if (tokenRequirements.Contains(typeof(FocusToken)) && WeaponInfo.SpendsToken == typeof(FocusToken))
            {
                Combat.Attacker.Tokens.SpendToken(typeof(FocusToken), callBack);
            }
            else
            {
                callBack();
            }
        }

    }

}

namespace SubPhases
{

    public class PayAttackCostDecisionSubPhase : DecisionSubPhase
    {
        public GenericSpecialWeapon Weapon;

        public override void PrepareDecision(System.Action callBack)
        {
            DescriptionShort = "Choose how to pay attack cost";

            List<GenericToken> waysToPay = new List<GenericToken>();

            List<char> letters = ActionsHolder.GetTargetLocksLetterPairs(Combat.Attacker, Combat.Defender);
            if (letters.Count > 0)
            {
                GenericToken targetLockToken = Combat.Attacker.Tokens.GetToken(typeof(BlueTargetLockToken), letters.First());
                if (targetLockToken != null) waysToPay.Add(targetLockToken);
            }

            Combat.Attacker.CallOnGenerateAvailableAttackPaymentList(waysToPay);

            foreach (var wayToPay in waysToPay)
            {
                if (wayToPay.GetType() == typeof(BlueTargetLockToken)) {
                    AddDecision(
                        "Target Lock token",
                        delegate {
                            PayCost(wayToPay);
                        });
                }
                else if (wayToPay.GetType() == typeof(FocusToken))
                {
                    AddDecision(
                        "Focus token",
                        delegate {
                            PayCost(wayToPay);
                        });
                }
                else if (wayToPay.GetType() == typeof(CalculateToken))
                {
                    AddDecision(
                        "Calculate token",
                        delegate {
                            PayCost(wayToPay);
                        });
                }
                else if (wayToPay.GetType() == typeof(ForceToken))
                {
                    AddDecision(
                        "Force token",
                        delegate {
                            PayCost(wayToPay);
                        });
                }
            }

            List<Decision> decision = GetDecisions();
            if (decision != null) DefaultDecisionName = GetDecisions().First().Name;

            callBack();
        }

        private void PayCost(GenericToken token)
        {
            if (Weapon.WeaponInfo.SpendsToken == typeof(BlueTargetLockToken) || token is ForceToken)
            {
                Combat.Attacker.Tokens.SpendToken(
                    token.GetType(),
                    ConfirmDecision,
                    (token as BlueTargetLockToken != null) ? (token as BlueTargetLockToken).Letter : ' '
                );
            }
            else
            {
                ConfirmDecision();
            }
        }

    }

}
