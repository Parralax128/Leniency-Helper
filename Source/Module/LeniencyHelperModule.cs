using Celeste.Mod.LeniencyHelper.Controllers;
using Celeste.Mod.LeniencyHelper.CrossModSupport;
using Celeste.Mod.LeniencyHelper.TweakSettings;
using Celeste.Mod.LeniencyHelper.UI;
using Microsoft.Xna.Framework;
using MonoMod.ModInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Celeste.Mod.LeniencyHelper.Module;
class LeniencyHelperModule : EverestModule
{
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


    #region very important generic stuff

    public static LeniencyHelperModule Instance { get; set; }

    public override Type SettingsType => typeof(LeniencyHelperSettings);
    public static LeniencyHelperSettings Settings => (LeniencyHelperSettings)Instance._Settings;

    public override Type SessionType => typeof(LeniencyHelperSession);
    public static LeniencyHelperSession Session => (LeniencyHelperSession)Instance._Session;

    public override Type SaveDataType => typeof(LeniencyHelperSaveData);
    public static LeniencyHelperSaveData SaveData => (LeniencyHelperSaveData)Instance._SaveData;
    #endregion

    #region loaded mods handling
    static Dictionary<string, (Version, bool)> ModsLoaded = new Dictionary<string, (Version, bool)>
    {
        { "MaxHelpingHand", (new Version(1,30,0), false) },
        { "ShroomHelper", (new Version(1,0,0), false) },
        { "VivHelper", (new Version(1,12,3), false) },
        { "ExtendedVariantMode", (new Version(0,35,0), false) }
    };
    public static bool ModLoaded(string mod) => ModsLoaded[mod].Item2;
    
    #endregion

    public static string Name => Instance.Metadata.Name;

    public static readonly Tweak[] TweakList = Enum.GetValues<Tweak>();
    public static Player GetPlayer(Monocle.Scene scene) => scene.Tracker.GetEntity<Player>();

    public LeniencyHelperModule()
    {        
        Instance = this;
        Logger.SetLogLevel(nameof(LeniencyHelperModule), LogLevel.Verbose);
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
    
    public override void Initialize()
    {
        base.Initialize();

        foreach (string mod in ModsLoaded.Keys)
        {
            ModsLoaded[mod] = ModsLoaded[mod] with 
            { Item2 = Everest.Loader.DependencyLoaded(new EverestModuleMetadata { Name = mod, Version = ModsLoaded[mod].Item1 }) };
        }
        
        if (ModLoaded("MaxHelpingHand"))
        {
            Tweaks.SolidBlockboostProtection.LoadSidewaysHook();
        }
        if (ModLoaded("VivHelper"))
        {
            Tweaks.BufferableClimbtrigger.LoadVivHelperHooks();
            ConsistentCoreboostDirectionController.LoadVivHelperHooks();
        }

        Watermark = GFX.Gui["LeniencyHelper/Parralax/Watermark"];
    }
    public override void Load()
    {
        TweakData.SetupIndices();
        Instance.LoadSettings();


        var loadHooksMethods = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypesSafe()).
            Where(x => x.IsClass).SelectMany(x => x.GetMethods()).
            Where(x => x.GetCustomAttributes(typeof(OnLoad), false).FirstOrDefault() != null);

        foreach (MethodInfo method in loadHooksMethods)
        {
            method.Invoke(null, null);
        }


        Everest.Events.Level.OnCreatePauseMenuButtons += AddTweaksMenuButton;
        On.Celeste.HudRenderer.RenderContent += RenderWatermark;

        Everest.Events.GameLoader.OnLoadThread += WebScrapper.LoadInfo;

        typeof(GravityHelperImports).ModInterop();
        typeof(ExtendedVariantImports).ModInterop();
        typeof(ModInteropExports).ModInterop();
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
        Everest.Events.GameLoader.OnLoadThread -= WebScrapper.LoadInfo;
    }

    void AddTweaksMenuButton(Level level, TextMenu menu, bool minimal)
    {
        if (!Settings.showSettings) return;

        int optionsIndex = menu.Items.FindIndex(item =>
            item.GetType() == typeof(TextMenu.Button) &&
            ((TextMenu.Button)item).Label == Dialog.Clean("MENU_PAUSE_SAVEQUIT"));

        if (optionsIndex == -1) return;

        TextMenu.Button button = TweakMenuManager.BuildMenuButton(menu);
        menu.Insert(optionsIndex, button);
    }

    static Monocle.MTexture Watermark;
    static void RenderWatermark(On.Celeste.HudRenderer.orig_RenderContent orig, HudRenderer self, Monocle.Scene scene)
    {
        orig(self, scene);

        if (Settings == null || Session == null || scene is not Level) return;

        if(TweakData.Tweaks == null)
        {
            return;
        }

        foreach (TweakState tweakState in TweakData.Tweaks) 
        {
            if (tweakState.Get(SettingSource.Player) == true)
            {
                Monocle.Draw.SpriteBatch.Begin();
                Watermark.Draw(new Vector2(27, 285), Vector2.Zero, Color.White * 0.15f, 0.5f);
                Monocle.Draw.SpriteBatch.End();

                break;
            }
        }
    }
}