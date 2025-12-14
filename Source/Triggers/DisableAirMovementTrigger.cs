using Monocle;
using Microsoft.Xna.Framework;
using MonoMod.Cil;
using System.Reflection;
using Celeste.Mod.Entities;

namespace Celeste.Mod.LeniencyHelper.Triggers;

[CustomEntity("LeniencyHelper/DisableAirMovementTrigger")]
class DisableAirMovementTrigger : GenericTrigger
{
    [OnLoad]
    public static void LoadHooks()
    {
        IL.Celeste.Player.NormalUpdate += DisableAirMovementOnUpdate;
        IL.Celeste.Glider.Update += DisableSpriteChanges;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        IL.Celeste.Player.NormalUpdate -= DisableAirMovementOnUpdate;
        IL.Celeste.Glider.Update -= DisableSpriteChanges;
    }

    public DisableAirMovementTrigger(EntityData data, Vector2 offset) : base(data, offset) { }
    protected override void Apply(Player player)
    {
        SetAirMovementDisabled(player, true);
    }
    protected override void Undo(Player player)
    {
        SetAirMovementDisabled(player, false);
    }

    static readonly VirtualIntegerAxis zero = new VirtualIntegerAxis(Settings.Instance.Up,
        Settings.Instance.UpMoveOnly, Settings.Instance.Down, Settings.Instance.DownMoveOnly, Input.Gamepad, 0.7f); 


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


        static int MoveXToZero(int orig, Player player)
        {
            if (player == null) return 0;
            if (player.onGround || !player.Get<Components.DisableAirMovementComponent>().Activated) return orig;
            return 0;
        }
    }
    
    static VirtualIntegerAxis MoveYToZero(VirtualIntegerAxis orig, Player player)
    {
        zero.Value = 0;
        if (player == null || !AirMovementDisabled(player) || player.onGround) return orig;

        return zero;
    }
    static void DisableSpriteChanges(ILContext il)
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

    static bool AirMovementDisabled(Player For) => For.Get<Components.DisableAirMovementComponent>().Activated;
    static void SetAirMovementDisabled(Player For, bool value) => For.Get<Components.DisableAirMovementComponent>().Activated = value;
}