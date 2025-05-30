﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RulesList;
using SquadBuilderNS;

public static class Rules
{
    public static WinConditionsRule WinConditions { get; private set; }
    public static DistanceBonusRule DistanceBonus { get; private set; }
    public static EndPhaseCleanupRule EndPhase { get; private set; }
    public static StressRule Stress { get; private set; }
    public static StrainRule Strain { get; private set; }
    public static DepleteRule Deplete { get; private set; }
    public static OffTheBoardRule OffTheBoard { get; private set; }
    public static CollisionRules Collision { get; private set; }
    public static ActionsRule Actions { get; private set; }
    public static AsteroidLandedRule AsteroidLanded { get; private set; }
    public static ObstaclesHitRule AsteroidHit { get; private set; }
    public static MineHitRule MineHit { get; private set; }
    public static AsteroidObstructionRule AsteroidObstruction { get; private set; }
    public static InitiativeRule Initiative { get; private set; }
    public static TargetIsLegalForShotRule TargetIsLegalForShot { get; private set; }
    public static IonizationRule Ionization { get; private set; }
    public static DisabledRule Disabled { get; private set; }
    public static JamRule Jam { get; private set; }
    public static ProtectRule Protect { get; private set; }
    public static TargetLocksRule TargetLocks { get; private set; }
    public static WeaponsDisabledRule WeaponsDisabled { get; private set; }
    public static BullseyeArcRule BullseyeArc { get; private set; }
    public static DockingRule Docking { get; private set; }
    public static TractorBeamRule TractorBeam { get; private set; }
    public static ForceRule Force { get; private set; }
    public static ChargeRule Charge { get; private set; }
    public static DestructionRule Destruction { get; private set; }
    public static RemotesRule Remotes { get; private set; }
    public static FuseRule Fuse { get; private set; }
    public static PurpleManeuversRule PurpleManeuvers { get; private set; }


    public static void Initialize()
    {
        if (Global.IsCampaignGame)
        {
            WinConditions = CampaignLoader.WinCondition;
        }
        else
        {
            WinConditions = new WinConditionsStandardRule();
        }        
        DistanceBonus = new DistanceBonusRule();
        EndPhase = new EndPhaseCleanupRule();
        Stress = new StressRule();
        Strain = new StrainRule();
        Deplete = new DepleteRule();
        OffTheBoard = new OffTheBoardRule();
        Collision = new CollisionRules();
        Actions = new ActionsRule();
        AsteroidLanded = new AsteroidLandedRule();
        AsteroidHit = new ObstaclesHitRule();
        MineHit = new MineHitRule();
        AsteroidObstruction = new AsteroidObstructionRule();
        Initiative = new InitiativeRule();
        TargetIsLegalForShot = new TargetIsLegalForShotRule();
        Ionization = new IonizationRule();
        Disabled = new DisabledRule();
        Jam = new JamRule();
        Protect = new ProtectRule();
        TargetLocks = new TargetLocksRule();
        WeaponsDisabled = new WeaponsDisabledRule();
        BullseyeArc = new BullseyeArcRule();
        Docking = new DockingRule();
        TractorBeam = new TractorBeamRule();
        Force = new ForceRule();
        Charge = new ChargeRule();
        Destruction = new DestructionRule();
        Fuse = new FuseRule();
        Remotes = new RemotesRule();
        PurpleManeuvers = new PurpleManeuversRule();
    }

    public static void FinishGame()
    {
        Docking.Initialize();
        Phases.EndGame();
    }
}

