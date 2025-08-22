using Celeste.Mod.LeniencyHelper.Components;
using MonoMod.Cil;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class WallCoyoteFrames : AbstractTweak<WallCoyoteFrames>
{
    public enum WallCoyoteTypes
    {
        Left,
        Right,
        Both
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

    private static void ConsumeOnWJ(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);

        while (cursor.TryGotoNext(MoveType.Before, i => i.MatchRet()))
        {
            cursor.EmitLdarg0();
            cursor.EmitDelegate(ConsumeCoyoteTime);
            cursor.Index++;
        }
    }
    private static void ConsumeCoyoteTime(Player player)
    {
        if (!Enabled) return;

        WallCoyoteFramesComponent component = player.Get<WallCoyoteFramesComponent>();
        if (component == null) return;

        if (component.wallCoyoteTimer > 0f && component.currentWallCoyoteType == WallCoyoteTypes.Both)
            player.Speed.X = 0f;
        component.wallCoyoteTimer = 0f;
    }

    public static bool CoyoteWallJumpCheck(On.Celeste.Player.orig_WallJumpCheck orig, Player self, int dir)
    {
        if (!Enabled || useOrigWJCheck)
        {
            return orig(self, dir);
        }

        WallCoyoteFramesComponent component = self.Get<WallCoyoteFramesComponent>();
        switch (dir)
        {
            case -1:
                return orig(self, -1) || (component.wallCoyoteTimer > 0 &&
                    (component.currentWallCoyoteType == WallCoyoteTypes.Right ||
                    component.currentWallCoyoteType == WallCoyoteTypes.Both));
            case 1:
                return orig(self, 1) || (component.wallCoyoteTimer > 0 &&
                    (component.currentWallCoyoteType == WallCoyoteTypes.Left ||
                    component.currentWallCoyoteType == WallCoyoteTypes.Both));
        }
        return false;
    }
}