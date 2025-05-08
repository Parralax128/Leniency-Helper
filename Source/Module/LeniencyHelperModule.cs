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
    #region very important generic stuff

    public static LeniencyHelperModule Instance { get; private set; }

    public override Type SettingsType => typeof(LeniencyHelperSettings);
    public static LeniencyHelperSettings Settings => (LeniencyHelperSettings)Instance._Settings;

    public override Type SessionType => typeof(LeniencyHelperSession);
    public static LeniencyHelperSession Session => (LeniencyHelperSession)Instance._Session;

    public override Type SaveDataType => typeof(LeniencyHelperSaveData);
    public static LeniencyHelperSaveData SaveData => (LeniencyHelperSaveData)Instance._SaveData;
    #endregion

    private static Dictionary<string, (Version, bool)> ModsLoaded = new Dictionary<string, (Version, bool)>
    {
        { "MaxHelpingHand", (new Version(1,30,0), false) },
        { "ShroomHelper", (new Version(1,0,0), false) },
        { "VivHelper", (new Version(1,12,3), false) },
        { "ExtendedVariantMode", (new Version(0,35,0), false) }
    };
    public static bool ModLoaded(string mod) => ModsLoaded[mod].Item2;

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
        "DisableForcemovedTech",
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

        foreach (FieldInfo field in typeof(SettingList).GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
        {
            SettingMaster.SettingListFields.Add(field.Name, field);
        }
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
        foreach (string mod in ModsLoaded.Keys)
        {
            ModsLoaded[mod] = ModsLoaded[mod] with 
            { 
                Item2 = Everest.Loader.DependencyLoaded(new EverestModuleMetadata { Name = mod, Version = ModsLoaded[mod].Item1 }) 
            };
        }
        if (ModLoaded("MaxHelpingHand"))
        {
            SolidBlockboostProtection.LoadSidewaysHook();
        }
        if (ModLoaded("VivHelper"))
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
        Everest.Events.Level.OnEnter += ClearSessionOnEnter;

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
        Everest.Events.Level.OnEnter -= ClearSessionOnEnter;
    }

    private static void ClearSessionOnEnter(Session session, bool justEntered)
    {
        if (!justEntered)
        {
            SessionSerializer.ClearSession(global::Celeste.SaveData.LoadedModSaveDataIndex);
        }
    }
    public override byte[] SerializeSession(int index)
    {
        return null;
    }
    public override void WriteSession(int index, byte[] data)
    {
        SessionSerializer.SaveSession(index, Session);
    }
    public override byte[] ReadSession(int index)
    {
        return null;
    }
    public override void DeserializeSession(int index, byte[] data)
    {
        _Session = SessionSerializer.LoadSession(index);
    }

    public override void LoadSettings()
    {
        base.LoadSettings();

        foreach(string tweak in TweakList)
        {
            if (!Settings.PlayerTweaks.ContainsKey(tweak))
            {
                Settings.PlayerTweaks.Add(tweak, null);
            }
        }
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