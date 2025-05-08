using Monocle;
using Microsoft.Xna.Framework;
using MonoMod.Cil;
using System.Reflection;
using Celeste.Mod.Entities;
using Celeste.Mod.LeniencyHelper.Module;

namespace Celeste.Mod.LeniencyHelper.Triggers;

[CustomEntity("LeniencyHelper/DisableAirMovementTrigger")]
public class DisableAirMovementTrigger : GenericTrigger
{
    [OnLoad]
    public static void LoadHooks()
    {
        IL.Celeste.Player.NormalUpdate += DisableAirMovementOnUpdate;
        IL.Celeste.Glider.Update += DisableSpriteChanges;
        On.Celeste.Player.ctor += ResetToggleOnPlayerRespawn;
        On.Celeste.LevelLoader.ctor += ResetToggleOnLevelLoad;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        IL.Celeste.Player.NormalUpdate -= DisableAirMovementOnUpdate;
        IL.Celeste.Glider.Update -= DisableSpriteChanges;
        On.Celeste.Player.ctor -= ResetToggleOnPlayerRespawn;
        On.Celeste.LevelLoader.ctor -= ResetToggleOnLevelLoad;
    }

    public DisableAirMovementTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {

    }
    public override void ApplySettings()
    {
        LeniencyHelperModule.Session.airMovementDisabled = true;
    }
    public override void UndoSettings()
    {
        LeniencyHelperModule.Session.airMovementDisabled = false;
    }

    private static VirtualIntegerAxis zero = new VirtualIntegerAxis(Settings.Instance.Up,
        Settings.Instance.UpMoveOnly, Settings.Instance.Down, Settings.Instance.DownMoveOnly, Input.Gamepad, 0.7f); 

    private static void ResetToggleOnPlayerRespawn(On.Celeste.Player.orig_ctor orig,
        Player self, Vector2 pos, PlayerSpriteMode spriteMode)
    {
        orig(self, pos, spriteMode);
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
        if (player == null || !LeniencyHelperModule.Session.airMovementDisabled || player.onGround) return orig;

        return zero;
    }

    public static void DisableAirMovementOnUpdate(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);

        while (cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdfld<Player>("moveX")))
        {
            cursor.EmitLdarg0();
            cursor.EmitDelegate(MoveXToZero);
        }
        cursor.Index = 0;

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

