using Celeste.Mod.LeniencyHelper.CrossModSupport;
using Celeste.Mod.LeniencyHelper.Tweaks;
using MonoMod.ModInterop;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Celeste.Mod.LeniencyHelper.Components;
using Celeste.Mod.LeniencyHelper.Triggers;
using Celeste.Mod.LeniencyHelper.UI;
using IL.Monocle;

namespace Celeste.Mod.LeniencyHelper;
public class LeniencyHelperModule : EverestModule
{
    #region very important stuff
    public static LeniencyHelperModule Instance { get; private set; }

    public override Type SettingsType => typeof(LeniencyHelperModuleSettings);
    public static LeniencyHelperModuleSettings Settings => (LeniencyHelperModuleSettings)Instance._Settings;

    public override Type SessionType => typeof(LeniencyHelperModuleSession);
    public static LeniencyHelperModuleSession Session => (LeniencyHelperModuleSession)Instance._Session;

    public override Type SaveDataType => typeof(LeniencyHelperModuleSaveData);
    public static LeniencyHelperModuleSaveData SaveData => (LeniencyHelperModuleSaveData)Instance._SaveData;
    #endregion

    public static Dictionary<(string, Version), bool> ModsLoaded = new Dictionary<(string, Version), bool>
    {
        { ("MaxHelpingHand", new Version(1,30,0)), false },
        { ("ShroomHelper", new Version(1,0,0)), false },
        { ("VivHelper", new Version(1,12,3)), false }
    };

    public Dictionary<string, bool> TweaksEnabled = LeniencyHelperModuleSession.EmptyDict();

    public enum Inputs
    {
        Jump, 
        Dash,
        Demo
    }
    public enum Dirs
    {
        Up, Down,
        Left, Right,
        All, None
    }

    public static string[] Tweaks =
    {
        "AutoSlowfall",
        "BackwardsRetention",
        "BufferableClimbtrigger",
        "BufferableExtends",
        "ConsistentDashOnDBlockExit",
        "CornerWaveLeniency",
        "CustomBufferTime",
        "DashCDIgnoreFFrames",
        "DirectionalReleaseProtection",
        "DisableBackboost",
        "DisableForceMove",
        "DynamicCornerCorrection",
        "DynamicWallLeniency",
        "ExtendBufferOnFreezeAndPickup",
        "ForceCrouchDemodash",
        "FrozenReverses",
        "IceWallIncreaseWallLeniency",
        "InstantAcceleratedJumps",
        "InstantClimbHop",
        "NoFailedTech",
        "RefillDashInCoyote",
        "RemoveDBlockCCorection",
        "RetainSpeedCornerboost",
        "SolidBlockboostProtection",
        "SuperOverWalljump",
        "WallAttraction",
        "WallCoyoteFrames"
    };
    public LeniencyHelperModule()
    {
        Instance = this;
        Logger.SetLogLevel(nameof(LeniencyHelperModule), LogLevel.Verbose);

        TweaksEnabled = LeniencyHelperModuleSession.EmptyDict();
    }
    public static void Log(object input)
    {
        Logger.Log(LogLevel.Info, "LeniencyHelper", input == null ? "null" : input.ToString());
    }
    
    public override void Initialize()
    {
        base.Initialize();
        foreach((string, Version) mod in ModsLoaded.Keys)
        {
            ModsLoaded[mod] = Everest.Loader.DependencyLoaded(new EverestModuleMetadata { Name = mod.Item1, Version = mod.Item2 });
        }
        if (ModsLoaded[("MaxHelpingHand", new Version(1, 30, 0))])
        {
            SolidBlockboostProtection.LoadSidewaysHook();
        }
    }

    public override void Load()
    {
        DisbleTweaksOnStartHookLoad();

        InstantAcceleratedJumps.LoadHooks();
        AutoSlowfall.LoadHooks();
        BufferableClimbtrigger.LoadHooks();
        DirectionalReleaseProtection.LoadHooks();
        BufferableExtends.LoadHooks();
        ConsistentDashOnDBlockExit.LoadHooks();
        ConsistentTheoSpinnerBounceTrigger.LoadHooks();
        CornerWaveLeniency.LoadHooks();
        CustomBufferTime.LoadHooks();
        DashCDIgnoreFFrames.LoadHooks();
        DisableBackboost.LoadHooks();
        DisableForceMove.LoadHooks();
        BackwardsRetention.LoadHooks();
        DynamicCornerCorrection.LoadHooks();
        ExtendBufferOnFreezeAndPickup.LoadHooks();
        ForceCrouchDemodash.LoadHooks();
        FrozenReverses.LoadHooks();
        IceWallIncreaseWallLeniency.LoadHooks();
        InstantClimbHop.LoadHooks();
        NoFailedTech.LoadHooks();
        RefillDashInCoyote.LoadHooks();
        RemoveDBlockCCorection.LoadHooks();
        RetainSpeedCornerboost.LoadHooks();
        SolidBlockboostProtection.LoadHooks();
        InputRequiresBlockboostTrigger.LoadHooks();
        WallAttraction.LoadHooks();
        WallCoyoteFrames.LoadHooks();
        SuperOverWalljump.LoadHooks();

        ClearBlockBoostTrigger.LoadHooks();
        DisableAirMovementTrigger.LoadHooks();

        MenuButtonManager.LoadHooks();
        ComponentManager.LoadHooks();

        Everest.Events.Level.OnCreatePauseMenuButtons += AddTweaksMenuButton;
        Everest.Events.LevelLoader.OnLoadingThread += AddStamp;

        typeof(GravityHelperImports).ModInterop();
    }
    public override void Unload()
    {
        DisbleTweaksOnStartHookUnload();

        InstantAcceleratedJumps.UnloadHooks();
        AutoSlowfall.UnloadHooks();
        BufferableClimbtrigger.UnloadHooks();
        DirectionalReleaseProtection.UnloadHooks();
        BufferableExtends.UnloadHooks();
        ConsistentDashOnDBlockExit.UnloadHooks();
        ConsistentTheoSpinnerBounceTrigger.UnloadHooks();
        CornerWaveLeniency.UnloadHooks();
        CustomBufferTime.UnloadHooks();
        DashCDIgnoreFFrames.UnloadHooks();
        DisableBackboost.UnloadHooks();
        DisableForceMove.UnloadHooks();
        BackwardsRetention.UnloadHooks();
        DynamicCornerCorrection.UnloadHooks();
        ExtendBufferOnFreezeAndPickup.UnloadHooks();
        ForceCrouchDemodash.UnloadHooks();
        FrozenReverses.UnloadHooks();
        IceWallIncreaseWallLeniency.UnloadHooks();
        InstantClimbHop.UnloadHooks();
        NoFailedTech.UnloadHooks();
        RefillDashInCoyote.UnloadHooks();
        RemoveDBlockCCorection.UnloadHooks();
        RetainSpeedCornerboost.UnloadHooks();
        SolidBlockboostProtection.UnloadHooks();
        SuperOverWalljump.UnloadHooks();
        WallAttraction.UnloadHooks();
        WallCoyoteFrames.UnloadHooks();

        ClearBlockBoostTrigger.UnloadHooks();
        DisableAirMovementTrigger.UnloadHooks();
        InputRequiresBlockboostTrigger.UnloadHooks();

        MenuButtonManager.UnloadHooks();
        ComponentManager.UnloadHooks();

        Everest.Events.Level.OnCreatePauseMenuButtons -= AddTweaksMenuButton;
        Everest.Events.LevelLoader.OnLoadingThread -= AddStamp;

    }

    public override void LoadSettings()
    {
        base.LoadSettings();

        foreach (string tweak in Tweaks)
        {
            if(!Settings.TweaksByPlayer.ContainsKey(tweak))
            {
                Settings.TweaksByPlayer.Add(tweak, null);
            }
        }
    }

    private void AddTweaksMenuButton(Level level, TextMenu menu, bool minimal)
    {
        bool showInGame = true;
        if (!showInGame) return;

        int optionsIndex = menu.Items.FindIndex(item =>
            item.GetType() == typeof(TextMenu.Button) &&
            ((TextMenu.Button)item).Label == Dialog.Clean("MENU_PAUSE_SAVEQUIT"));

        TextMenu.Button button = MenuButtonManager.BuildMenuButton(menu);

        menu.Insert(optionsIndex, button);
    }

    #region reset tweaks on start
    public static void DisbleTweaksOnStartHookLoad()
    {
        On.Celeste.Player.ctor += DisableTweaksOnRespawn;
        On.Celeste.LevelLoader.ctor += DisableTweaksOnLoad;
    }
    public static void DisbleTweaksOnStartHookUnload()
    {
        On.Celeste.Player.ctor -= DisableTweaksOnRespawn;
        On.Celeste.LevelLoader.ctor -= DisableTweaksOnLoad;
    }

    public static void DisableTweaksOnRespawn(On.Celeste.Player.orig_ctor orig,
        Player self, Vector2 pos, PlayerSpriteMode spriteMode)
    {
        orig(self, pos, spriteMode);

        DisableTweaks();
    }
    public static void DisableTweaksOnLoad(On.Celeste.LevelLoader.orig_ctor orig,
        LevelLoader self, Session session, Vector2? startPos)
    {
        orig(self, session, startPos);

        DisableTweaks();
    }
    private static void DisableTweaks()
    {
        var s = Session;

        foreach (string tweakName in s.TweaksEnabled.Keys)
        {
            s.TweaksByMap[tweakName] = false;
            s.UpdateTweak(tweakName);
        }

        if (s is not null)
        {
            if (s.BindList is not null)
                s.BindList = Array.Empty<InputRequiresBlockboostTrigger.BindInfo>();

            s.airMovementDisabled = false;
            s.clearBlockBoostActivated = false;
        }
    }
    #endregion


    private static void AddStamp(Level level)
    {
        level.Add(new StampRenderer(new Vector2(41f, 240f)));
        Session.TweaksByMap = LeniencyHelperModuleSession.EmptyDict();
    }
}