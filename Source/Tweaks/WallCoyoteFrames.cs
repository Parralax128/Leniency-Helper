using Celeste.Mod.LeniencyHelper.Components;
using MonoMod.Cil;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

class WallCoyoteFrames : AbstractTweak<WallCoyoteFrames>
{
    public enum WallSides
    {
        Left = -1,
        Right = 1,
        Both = 0
    }

    [OnLoad]
    public static void LoadHooks()
    {
        On.Celeste.Player.WallJumpCheck += CoyoteWallJumpCheck;

        IL.Celeste.Player.WallJump += ConsumeOnWJ;
        IL.Celeste.Player.ClimbJump += ConsumeOnWJ;
        IL.Celeste.Player.SuperWallJump += ConsumeOnWJ;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        On.Celeste.Player.WallJumpCheck -= CoyoteWallJumpCheck;

        IL.Celeste.Player.WallJump -= ConsumeOnWJ;
        IL.Celeste.Player.ClimbJump -= ConsumeOnWJ;
        IL.Celeste.Player.SuperWallJump -= ConsumeOnWJ;
    }

    public static bool useOrigWJCheck = false;

    static void ConsumeOnWJ(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);

        while (cursor.TryGotoNext(MoveType.Before, i => i.MatchRet()))
        {
            cursor.EmitLdarg0();
            cursor.EmitDelegate(ConsumeCoyoteTime);
            cursor.Index++;
        }

        static void ConsumeCoyoteTime(Player player)
        {
            if (!Enabled) return;

            WallCoyoteFramesComponent component = player.Get<WallCoyoteFramesComponent>();
            if (component == null) return;

            if (component.NullHorizontal) player.Speed.X = 0f;
            component.AbortTimer();
        }
    }
    

    public static bool CoyoteWallJumpCheck(On.Celeste.Player.orig_WallJumpCheck orig, Player self, int dir)
    {
        if (!Enabled || useOrigWJCheck) return orig(self, dir);


        WallCoyoteFramesComponent component = self.Get<WallCoyoteFramesComponent>();
        return component.CheckWall(dir);
    }
}