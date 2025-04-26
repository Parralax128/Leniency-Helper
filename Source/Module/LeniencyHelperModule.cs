using Celeste.Mod.LeniencyHelper.CrossModSupport;
using Celeste.Mod.LeniencyHelper.Tweaks;
using MonoMod.ModInterop;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Celeste.Mod.LeniencyHelper.UI;
using System.Linq;
using System.Reflection;

namespace Celeste.Mod.LeniencyHelper.Module;
public class LeniencyHelperModule : EverestModule
{
    #region very important generated stuff

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
        { ("VivHelper", new Version(1,12,3)), false },
        { ("ExtendedVariantMode", new Version(0,35,0)), false }
    };
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

    public static string[] TweakList =
    {
        "AutoSlowfall",
        "BackwardsRetention",
        "BufferableClimbtrigger",
        "BufferableExtends",
        "ConsistentDashOnDBlockExit",
        "CornerWaveLeniency",
        "CustomBufferTime",
        "CustomDashbounceTiming",
        "DashCDIgnoreFFrames",
        "DirectionalReleaseProtection",
        "DisableBackboost",
        "DisableForceMove",
        "DynamicCornerCorrection",
        "DynamicWallLeniency",
        "ExtendBufferOnFreezeAndPickup",
        "ForceCrouchDemodash",
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

    public static SettingList DefaultSettings = new SettingList();

    public LeniencyHelperModule()
    {
        Instance = this;
        Logger.SetLogLevel(nameof(LeniencyHelperModule), LogLevel.Verbose);

        foreach (FieldInfo field in typeof(SettingList).GetFields())
            SettingMaster.SettingListFields[field.Name] = field;
    }
    public static void Log(object input)
    {
        Logger.Log(LogLevel.Info, "LeniencyHelper", input == null ? "null" : input.ToString());
    }

    public static bool CollideOnWJdist<T>(Monocle.Entity entity, int dir, Vector2? at) where T : Monocle.Entity
    {
        Vector2 savePos = entity.Position;
        if (at.HasValue) entity.Position = at.Value;
        Rectangle checkRect = entity.Collider.Bounds;
        if (at.HasValue) entity.Position = savePos;

        checkRect.Width += dir == 1 ? Session.wjDistR : Session.wjDistL;
        checkRect.X += (dir == 1 ? Session.wjDistR : Session.wjDistL) * dir;

        return entity.Scene.CollideCheck<T>(checkRect);
    }
    public static bool CollideOnWJdist(Monocle.Entity entity, Monocle.Entity with, int dir, Vector2? at)
    {
        Vector2 savePos = entity.Position;
        if (at.HasValue) entity.Position = at.Value;
        Rectangle checkRect = entity.Collider.Bounds;
        if (at.HasValue) entity.Position = savePos;

        checkRect.Width += dir == 1 ? Session.wjDistR : Session.wjDistL;
        checkRect.X += (dir == 1 ? Session.wjDistR : Session.wjDistL) * dir;

        return entity.Scene.CollideCheck(checkRect, with);
    }
    public override void Initialize()
    {
        base.Initialize();
        foreach ((string, Version) mod in ModsLoaded.Keys)
        {
            ModsLoaded[mod] = Everest.Loader.DependencyLoaded(new EverestModuleMetadata { Name = mod.Item1, Version = mod.Item2 });
        }
        if (ModsLoaded[("MaxHelpingHand", new Version(1, 30, 0))])
        {
            SolidBlockboostProtection.LoadSidewaysHook();
        }
        if (ModsLoaded[("VivHelper", new Version(1, 12, 3))])
        {
            BufferableClimbtrigger.LoadVivHelperHooks();
        }
    }

    public override void Load()
    {
        var loadHooksMethods = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypesSafe()).Where(x => x.IsClass)
        .SelectMany(x => x.GetMethods()).Where(x => x.GetCustomAttributes(typeof(OnLoad), false).FirstOrDefault() != null);

        foreach (MethodInfo method in loadHooksMethods)
        {
            method.Invoke(null, null);
        }

        Everest.Events.Level.OnCreatePauseMenuButtons += AddTweaksMenuButton;
        Everest.Events.LevelLoader.OnLoadingThread += AddStamp;

        typeof(GravityHelperImports).ModInterop();
        typeof(ExtendedVariantImports).ModInterop();
    }
    public override void Unload()
    {
        var loadHooksMethods = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypesSafe()).Where(x => x.IsClass)
        .SelectMany(x => x.GetMethods()).Where(x => x.GetCustomAttributes(typeof(OnUnload), false).FirstOrDefault() != null);

        foreach (MethodInfo method in loadHooksMethods)
        {
            method.Invoke(null, null);
        }

        Everest.Events.Level.OnCreatePauseMenuButtons -= AddTweaksMenuButton;
        Everest.Events.LevelLoader.OnLoadingThread -= AddStamp;
    }


    public override byte[] SerializeSession(int index)
    {
        return null;
    }
    public override void WriteSession(int index, byte[] data)
    {
        SessionSerialization.SaveSession(index, Session);
    }
    public override byte[] ReadSession(int index)
    {
        return null;
    }
    public override void DeserializeSession(int index, byte[] data)
    {
        _Session = SessionSerialization.LoadSession(index);
    }


    private void AddTweaksMenuButton(Level level, TextMenu menu, bool minimal)
    {
        if (!Settings.showSettings) return;

        int optionsIndex = menu.Items.FindIndex(item =>
            item.GetType() == typeof(TextMenu.Button) &&
            ((TextMenu.Button)item).Label == Dialog.Clean("MENU_PAUSE_SAVEQUIT"));

        if (optionsIndex == -1) return;

        TextMenu.Button button = MenuButtonManager.BuildMenuButton(menu);
        menu.Insert(optionsIndex, button);
    }

    private static void AddStamp(Level level)
    {
        level.Add(new StampRenderer(new Vector2(41f, 240f)));
    }
}