using Celeste.Mod.LeniencyHelper.Controllers;
using Celeste.Mod.LeniencyHelper.CrossModSupport;
using Celeste.Mod.LeniencyHelper.Tweaks;
using Celeste.Mod.LeniencyHelper.UI;
using Microsoft.Xna.Framework;
using MonoMod.Cil;
using MonoMod.ModInterop;
using System;
using System.Collections.Generic;
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
        Demo,
        Grab
    }
    public enum Dirs
    {
        Up, Down,
        Left, Right,
        All, None
    }

    public static readonly string[] TweakList =
    {
        "AutoSlowfall",
        "BackboostProtection",
        "BackwardsRetention",
        "BufferableClimbtrigger",
        "BufferableExtends",
        "ConsistentDashOnDBlockExit",
        "ConsistentWallboosters",
        "CornerWaveLeniency",
        "CrouchOnBonk",
        "CustomBufferTime",
        "CustomDashbounceTiming",
        "CustomSnapDownDistance",
        "DashCDIgnoreFFrames",
        "DelayedClimbtrigger",
        "DirectionalReleaseProtection",
        "DisableBackboost",
        "DisableForcemovedTech",
        "DynamicCornerCorrection",
        "DynamicWallLeniency",
        "ExtendBufferOnFreezeAndPickup",
        "ExtendDashAttackOnPickup",
        "ForceCrouchDemodash",
        "GultraCancel",
        "IceWallIncreaseWallLeniency",
        "InstantAcceleratedJumps",
        "LateReverses",
        "ManualDreamhyperLeniency",
        "NoFailedTech",
        "RefillDashInCoyote",
        "RemoveDBlockCCorection",
        "RespectInputOrder",
        "RetainSpeedCornerboost",
        "SolidBlockboostProtection",
        "SuperdashSteeringProtection",
        "SuperOverWalljump",
        "WallAttraction",
        "WallCoyoteFrames",    

    };
    public static SettingList DefaultSettings = new SettingList();
    public static Player GetPlayer(Monocle.Scene scene) => scene.Tracker.GetEntity<Player>();

    public LeniencyHelperModule()
    {        
        Instance = this;
        Logger.SetLogLevel(nameof(LeniencyHelperModule), LogLevel.Verbose);
        foreach (FieldInfo field in typeof(SettingList).GetFields())
        {
            SettingMaster.SettingListFields.Add(field.Name, field);
        }
    }
    public static void Log(object input)
    {
        Logger.Log(LogLevel.Info, "LeniencyHelper", input.ToString() ?? "null");
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

    public static bool CollideOnWJdist(Monocle.Entity entity, Monocle.Entity with, int dir, Vector2? at = null)
    {
        Vector2 savePos = entity.Position;

        if (at.HasValue) entity.Position = at.Value;
        Rectangle checkRect = entity.Collider.Bounds;
        if (at.HasValue) entity.Position = savePos;

        checkRect.Width += dir == 1 ? Session.wjDistR : Session.wjDistL;
        if (dir == -1) checkRect.X -= Session.wjDistL;

        return entity.Scene.CollideCheck(checkRect, with);
    }

    public static event Action<Player> BeforePlayerUpdate;
    public static event Action<Player> OnPlayerUpdate;
    public static event Action OnUpdate;

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
            ConsistentCoreboostDirectionController.LoadVivHelperHooks();
        }

        Watermark = GFX.Gui["LeniencyHelper/Parralax/Watermark"];
    }

    public override void Load()
    {
        var loadHooksMethods = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypesSafe()).
            Where(x => x.IsClass).SelectMany(x => x.GetMethods()).
            Where(x => x.GetCustomAttributes(typeof(OnLoad), false).FirstOrDefault() != null);

        foreach (MethodInfo method in loadHooksMethods)
        {
            method.Invoke(null, null);
        }


        Everest.Events.Level.OnCreatePauseMenuButtons += AddTweaksMenuButton;
        On.Celeste.HudRenderer.RenderContent += RenderWatermark;
        Everest.Events.Level.OnEnter += ClearSessionOnEnter;

        IL.Celeste.Level.GiveUp += InsertSessionClear;

        On.Celeste.Player.Update += OnPlayerUpdateEventHook;
        On.Celeste.Level.Update += OnUpdateEventHook;

        typeof(GravityHelperImports).ModInterop();
        typeof(ExtendedVariantImports).ModInterop();

        typeof(ModInteropExports).ModInterop();
    }
    private static void OnPlayerUpdateEventHook(On.Celeste.Player.orig_Update orig, Player self)
    {
        BeforePlayerUpdate(self);
        orig(self);
        OnPlayerUpdate(self);
    }
    private static void OnUpdateEventHook(On.Celeste.Level.orig_Update orig, Level self)
    {
        orig(self);
        OnUpdate();
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
        On.Celeste.HudRenderer.RenderContent -= RenderWatermark;
        Everest.Events.Level.OnEnter -= ClearSessionOnEnter;

        IL.Celeste.Level.GiveUp -= InsertSessionClear;

        On.Celeste.Player.Update -= OnPlayerUpdateEventHook;
        On.Celeste.Level.Update -= OnUpdateEventHook;
    }

    #region settings && session
    
    private static void ClearSessionOnEnter(Session session, bool justEntered)
    {
        if (!justEntered)
        {
            SessionSerializer.ClearSessionFile(global::Celeste.SaveData.LoadedModSaveDataIndex);
        }
    }
    public override byte[] SerializeSession(int index) => null;
    public override void WriteSession(int index, byte[] data) => SessionSerializer.SaveSession(index, Session);
    public override byte[] ReadSession(int index) => null;
    public override void DeserializeSession(int index, byte[] data) =>_Session = SessionSerializer.LoadSession(index);
    private static void InsertSessionClear(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);

        if(cursor.TryGotoNext(MoveType.Before, instr => instr.MatchCallvirt<TextMenu.Item>("Pressed")))
        {
            cursor.EmitDelegate(ClearSessionOnButtonPress);
        }
    }
    private static Action ClearSessionOnButtonPress(Action orig) 
        => (() => SessionSerializer.ClearSessionFile(global::Celeste.SaveData.LoadedModSaveDataIndex)) + orig;
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
    #endregion

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

    private static Monocle.MTexture Watermark;
    private static void RenderWatermark(On.Celeste.HudRenderer.orig_RenderContent orig, HudRenderer self, Monocle.Scene scene)
    {
        orig(self, scene);

        if (Settings == null || Session == null || scene is not Level) return;

        foreach (string tweak in TweakList)
        {
            if (Settings.PlayerTweaks[tweak] == true)
            {
                Monocle.Draw.SpriteBatch.Begin();
                Watermark.Draw(new Vector2(27, 285), Vector2.Zero, Color.White * 0.15f, 0.5f);
                Monocle.Draw.SpriteBatch.End();

                break;
            }
        }
    }
}