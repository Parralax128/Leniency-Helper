using Celeste;
using Monocle;
using Microsoft.Xna.Framework;
using static Celeste.Mod.LeniencyHelper.LeniencyHelperModule;
using System;
using static Celeste.TrackSpinner;
using System.Xml.Schema;
using MonoMod.Cil;
using static Celeste.Mod.Helpers.ILCursorExtensions;
using Mono.Cecil.Cil;
using System.Reflection;
using AsmResolver.DotNet.Signatures;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework.Input;

namespace Celeste.Mod.LeniencyHelper.Triggers;

[CustomEntity("LeniencyHelper/DisableAirMovementTrigger")]
public class DisableAirMovementTrigger : GenericTrigger
{
    public DisableAirMovementTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
            
    }
    public override void GetOldSettings()
    {
        wasEnabled = LeniencyHelperModule.Session.airMovementDisabled;
    }
    public override void ApplySettings()
    {
        if(LeniencyHelperModule.Session.airMovementDisabled != enabled)
        {
            wasEnabled = LeniencyHelperModule.Session.airMovementDisabled;
            LeniencyHelperModule.Session.airMovementDisabled = enabled;
        }
    }
    public override void UndoSettings()
    {
        LeniencyHelperModule.Session.airMovementDisabled = wasEnabled;
    }
    
    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        var s = LeniencyHelperModule.Session;
    }

    private static VirtualIntegerAxis zero = 
        new VirtualIntegerAxis(Settings.Instance.Up,
            Settings.Instance.UpMoveOnly, Settings.Instance.Down,
            Settings.Instance.DownMoveOnly, Input.Gamepad, 0.7f);
    public static void LoadHooks()
    {
        IL.Celeste.Player.NormalUpdate += DisableAirMovementOnUpdate;
        IL.Celeste.Glider.Update += DisableSpriteChanges;
        On.Celeste.Player.ctor += ResetToggleOnPlayerRespawn;
        On.Celeste.LevelLoader.ctor += ResetToggleOnLevelLoad;
    }
    public static void UnloadHooks()
    {
        IL.Celeste.Player.NormalUpdate -= DisableAirMovementOnUpdate;
        IL.Celeste.Glider.Update -= DisableSpriteChanges;
        On.Celeste.Player.ctor -= ResetToggleOnPlayerRespawn;
        On.Celeste.LevelLoader.ctor -= ResetToggleOnLevelLoad;
    }
    private static void ResetToggleOnPlayerRespawn(On.Celeste.Player.orig_ctor orig,
        Player self, Vector2 pos, PlayerSpriteMode spriteMode)
    {
        orig(self,pos,spriteMode);
        LeniencyHelperModule.Session.airMovementDisabled = false;
    }
    private static void ResetToggleOnLevelLoad(On.Celeste.LevelLoader.orig_ctor orig,
        LevelLoader self, Session session, Vector2? startPos)
    {
        orig(self, session, startPos);
        LeniencyHelperModule.Session.airMovementDisabled = false;
    }
    private static int MoveXToZero(int orig, Player player)
    {
        if (player is null) return 0;

        var s = LeniencyHelperModule.Session;
        if (!s.airMovementDisabled) return orig;

        if (player.onGround) return orig;

        return 0;
    }
    private static VirtualIntegerAxis MoveYToZero(VirtualIntegerAxis orig, Player player)
    {
        zero.Value = 0;
        if (player is null) return zero;

        var s = LeniencyHelperModule.Session;
        if (!s.airMovementDisabled) return orig;
        if (player.onGround) return orig;

        return zero;
    }
    
    public static void DisableAirMovementOnUpdate(ILContext il)
    {

        ILCursor cursor = new ILCursor(il);

        while(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdfld<Player>("moveX")))
        {
            cursor.EmitLdarg0();
            cursor.EmitDelegate(MoveXToZero);
        }
        cursor.Index = 0;
        
        while(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdsfld(
                typeof(Input).GetField("MoveY", BindingFlags.Public | BindingFlags.Static))))
        {
            cursor.EmitLdarg0();
            cursor.EmitDelegate(MoveYToZero);
        }
        cursor.Index = 0;
        
        while(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdsfld(
                typeof(Input).GetField("GliderMoveY", BindingFlags.Public | BindingFlags.Static))))
        {
            cursor.EmitLdarg0();
            cursor.EmitDelegate(MoveYToZero);
        }
    }
    private static void DisableSpriteChanges(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);


        while (cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdsfld(
                typeof(Input).GetField("MoveY", BindingFlags.Public | BindingFlags.Static))))
        {
            cursor.EmitLdarg0();
            cursor.EmitDelegate(MoveYToZero);
        }
        cursor.Index = 0;

        while (cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdsfld(
        typeof(Input).GetField("GliderMoveY", BindingFlags.Public | BindingFlags.Static))))
        {
            cursor.EmitLdarg0();
            cursor.EmitDelegate(MoveYToZero);
        }
    }
}

