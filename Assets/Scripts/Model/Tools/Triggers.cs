﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubPhases;

public enum TriggerTypes
{
    None,

    OnGameStart,
    OnSetupStart,
    OnSetupEnd,
    OnInitiativeSelection,
    OnShipIsPlaced,
    OnRoundStart,
    OnPlanningSubPhaseStart,
    OnSystemsPhaseStart,
    OnActionSubPhaseStart,
    OnActionDecisionSubPhaseEnd,
    OnActivationPhaseStart,
    OnActivationPhaseEnd,
    OnCombatPhaseStart,
    OnCombatPhaseEnd,
    OnEndPhaseStart,
    OnRoundEnd,
    OnEngagementInitiativeChanged,
    OnSelectTargetForAttackStart_System,

    OnMovementActivationStart,
    OnCombatActivation,
    OnCombatDeactivation,

    OnManeuver,
    OnManeuverIsReadyToBeRevealed,
    OnManeuverIsRevealed,
    OnManeuverIsSkipped,
    BeforeMovementIsExecuted,
    OnMovementStart,
    OnMovementExecuted,
    OnMovementFinish,
    OnPositionIsReadyToFinish,
    OnPositionFinish,
    OnMovementActivationFinish,

    OnFreeActionPlanned,
    OnFreeAction,
    BeforeActionIsPerformed,
    OnActionIsPerformed,
    OnActionIsPerformed_System,
    OnActionIsReadyToBeFailed,
    OnActionIsReallyFailed,
    OnBeforeTokenIsAssigned,
    OnTokenIsAssigned,
    OnTokenIsSpent,
    OnTokenIsRemoved,
    OnCoordinateTargetIsSelected,
    OnCoordinateMultiTargetsAreSelected,
    OnJamTargetIsSelected,
    OnProtectTargetIsSelected,
    OnTargetLockIsAcquired,
    OnRerollIsConfirmed,
    OnDieResultIsSpent,
    OnDecloak,
    OnSlam,

    OnAttackStart,
    OnShotStart,
    OnImmediatelyAfterRolling,
    OnImmediatelyAfterReRolling,
    OnDefenseStart,
    OnShotHit,
    OnTryDamagePrevention,
    OnAfterNeutralizeResults,
    OnAttackHit,
    OnAttackMissed,
    OnAttackFinish,
    OnCombatCheckExtraAttack,

    OnAfterModifyDefenseDiceStep,
    OnAtLeastOneCritWasCancelledByDefender,
    OnDamageIsDealt,
    OnDamageWasSuccessfullyDealt,
    OnDamageInstanceResolved,
    OnShieldIsLost,
    OnDamageCardSeverityIsChecked,
    OnDamageCardIsDealt,
    OnFaceupCritCardReadyToBeDealt,
    OnFaceupCritCardReadyToBeDealtUI,
    OnFaceupCritCardIsDealt,
    OnSelectDamageCardToExpose,
    OnFaceupDamageCardIsRepaired,
    OnShipIsDestroyedCheck,
    OnShipIsDestroyed,
    OnShipIsReadyToBeRemoved,
    OnShipIsRemoved,

    OnBombIsDetonated,
    OnBombIsRemoved,
    OnCheckPermissionToDetonate,
    OnCheckSufferBombDetonation,
    OnAfterSufferBombEffect,

    OnAbilityDirect,
    OnAbilityTargetIsSelected,
    OnMajorExplosionCrit,
    OnDiscard,
    OnFlipFaceUp,
    OnDiceAboutToBeRolled,
    OnAfterDiscard,
    OnAfterFlipFaceUp,
    OnSystemsAbilityActivation,
    OnForceTokensAreSpent,

    BeforeBombWillBeDropped,
    OnBombWillBeDropped,
    OnBombWasDropped,
    OnBombWasLaunched,
    OnRemoteWasDropped,
    OnRemoteWasLaunched,
    OnCheckDropOfSecondDevice,

    OnUndockingFinish,

    OnRedTokenGainedFromOverlappingObstacle
}

public class Trigger
{
    public string Name;
    public Players.PlayerNo TriggerOwner;
    public TriggerTypes TriggerType;
    public EventHandler EventHandler;
    public object Sender;
    public EventArgs EventArgs;
    public bool Skippable;

    public bool IsCurrent;

    public bool IsPriority;

    public void Fire()
    {
        IsCurrent = true;
        EventHandler(Sender, EventArgs);
    }
}

public class StackLevel
{
    private List<Trigger> triggers = new List<Trigger>();
    public int level;
    public bool IsActive;
    public Action CallBack;
    public TriggerTypes TriggerType { get; private set; }
    
    public StackLevel(TriggerTypes triggerType)
    {
        TriggerType = triggerType;
        level = Triggers.TriggersStack.Count();
    }

    public int GetSize()
    {
        return triggers.Count;
    }

    public bool Empty()
    {
        return GetSize() == 0;
    }

    public Trigger GetFirst()
    {
        return triggers[0];
    }

    public void AddTrigger(Trigger trigger)
    {
        triggers.Add(trigger);
    }

    public void RemoveTrigger(Trigger trigger)
    {
        triggers.Remove(trigger);
    }

    public List<Trigger> GetTriggersByPlayer(Players.PlayerNo playerNo)
    {
        return triggers.Where(n => n.TriggerOwner == playerNo).ToList<Trigger>();
    }

    public List<Trigger> GetTrigersList()
    {
        return triggers;
    }

    public Trigger GetCurrentTrigger()
    {
        return triggers.Where(n => n.IsCurrent).First();
    }

}

public static partial class Triggers
{
    public static Trigger CurrentTrigger { get; private set; }

    public static List<StackLevel> TriggersStack { get; private set; }

    // PUBLIC

    public static void Initialize()
    {
        TriggersStack = new List<StackLevel>();
    }

    public static void RegisterTrigger(Trigger trigger)
    {
        if (NewLevelIsRequired())
        {
            CreateTriggerInNewLevel(trigger);
        }
        else
        {
            AddTriggerToCurrentStackLevel(trigger);
        }
    }

    public static void ResolveTriggers(TriggerTypes triggerType, Action callBack = null)
    {
        if (triggerType == TriggerTypes.OnDamageIsDealt && callBack != null) DamageNumbers.UpdateSavedHP();

        StackLevel currentLevel = GetCurrentLevel();

        if (currentLevel == null || currentLevel.IsActive)
        {
            CreateNewLevelOfStack(triggerType, callBack);
            currentLevel = GetCurrentLevel();
        }

        if (!currentLevel.IsActive)
        {
            SetStackLevelCallBack(callBack);

            List<Trigger> currentTriggersList = currentLevel.GetTriggersByPlayer(Phases.PlayerWithInitiative);
            Players.PlayerNo currentPlayer = (currentTriggersList.Count > 0) ? Phases.PlayerWithInitiative : Roster.AnotherPlayer(Phases.PlayerWithInitiative);
            currentTriggersList = currentLevel.GetTriggersByPlayer(currentPlayer);

            if (currentTriggersList.Count != 0)
            {
                currentLevel.IsActive = true;
                if ((currentTriggersList.Count == 1) || (IsAllSkippable(currentTriggersList)))
                {
                    FireTrigger(currentTriggersList[0]);
                }
                //TODO fix this kludgey crap
                else if(currentTriggersList.Where(n=>n.IsPriority).ToList<Trigger>().Count >0)
                {
                    FireTrigger(currentTriggersList.Where(n => n.IsPriority).ToList<Trigger>()[0]);
                }
                else
                {
                    RunDecisionSubPhase();
                }
            }
            else
            {
                if (triggerType == TriggerTypes.OnDamageIsDealt) DamageNumbers.ShowChangedHP();
                DoCallBack();
            }
        }
        
    }

    public static void FireTrigger(Trigger trigger)
    {
        Console.Write($"Trigger {trigger.Name} is fired ({trigger.TriggerType})", color: "yellow");

        CurrentTrigger = trigger;
        trigger.Fire();
    }

    public static void FinishTrigger()
    {
        StackLevel currentStackLevel = GetCurrentLevel();

        if (currentStackLevel == null || currentStackLevel.GetTrigersList() == null || currentStackLevel.GetTrigersList().Count == 0)
        {
            Debug.Log("Ooops! You want to finish trigger, but it is already finished");
        }

        Trigger currentTrigger = currentStackLevel.GetCurrentTrigger();

        currentStackLevel.RemoveTrigger(currentTrigger);
            currentStackLevel.IsActive = false;
            CurrentTrigger = null;

            ResolveTriggers(currentTrigger.TriggerType);
    }

    // PRIVATE

    private static bool NewLevelIsRequired()
    {
        return ((TriggersStack.Count == 0) || (Triggers.GetCurrentLevel().IsActive));
    }

    private static void SetStackLevelCallBack(Action callBack)
    {
        if (callBack != null)
        {
            GetCurrentLevel().CallBack = callBack;
        }
    }

    private static void RunDecisionSubPhase()
    {
        Phases.StartTemporarySubPhaseOld("Triggers Order", typeof(TriggersOrderSubPhase));
    }

    private static void DoCallBack()
    {
        Action callBack = GetCurrentLevel().CallBack;
        RemoveLastLevelOfStack();

        if (GetCurrentLevel() == null)
        {
            //
        }
        else
        {
            string triggerTypesInStack = "";
            foreach (var level in TriggersStack)
            {
                triggerTypesInStack += level.TriggerType;
                if (level != TriggersStack.Last()) triggerTypesInStack += ", ";
            }
        }

        callBack();
    }

    private static void RemoveLastLevelOfStack()
    {
        TriggersStack.Remove(GetCurrentLevel());
    }

    private static StackLevel GetCurrentLevel()
    {
        StackLevel result = null;
        if (TriggersStack.Count > 0)
        {
            result = TriggersStack[TriggersStack.Count - 1];
        }
        return result;
    }

    private static void CreateTriggerInNewLevel(Trigger trigger)
    {
        CreateNewLevelOfStack(trigger.TriggerType);
        AddTriggerToCurrentStackLevel(trigger);
    }

    private static void AddTriggerToCurrentStackLevel(Trigger trigger)
    {
        TriggersStack[TriggersStack.Count - 1].AddTrigger(trigger);
    }

    private static void CreateNewLevelOfStack(TriggerTypes triggerType, Action callBack = null)
    {
        TriggersStack.Add(new StackLevel(triggerType));
        GetCurrentLevel().CallBack = callBack ?? delegate () { ResolveTriggers(TriggerTypes.None); };
    }

    private static bool IsAllSkippable(List<Trigger> currentTriggersList)
    {
        foreach (var trigger in currentTriggersList)
        {
            if (!trigger.Skippable) return false;
        }
        return true;
    }

    // SUBPHASE

    private class TriggersOrderSubPhase : DecisionSubPhase
    {

        public override void PrepareDecision(System.Action callBack)
        {
            DescriptionShort = "Select a trigger to resolve";

            List<Trigger> currentTriggersList = Triggers.GetCurrentLevel().GetTriggersByPlayer(Phases.PlayerWithInitiative);
            Players.PlayerNo currentPlayer = (currentTriggersList.Count > 0) ? Phases.PlayerWithInitiative : Roster.AnotherPlayer(Phases.PlayerWithInitiative);
            currentTriggersList = Triggers.GetCurrentLevel().GetTriggersByPlayer(currentPlayer);

            foreach (var trigger in currentTriggersList)
            {
                if (trigger.TriggerOwner == currentPlayer)
                {
                    AddDecision(trigger.Name, delegate {
                        Phases.FinishSubPhase(this.GetType());
                        FireTrigger(trigger);
                    });
                }
            }

            DecisionOwner = Roster.GetPlayer(currentPlayer);
            DefaultDecisionName = GetDecisions().First().Name;

            callBack();
        }

    }

}

