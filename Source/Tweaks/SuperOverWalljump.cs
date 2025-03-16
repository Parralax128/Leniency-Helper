using System;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using static Celeste.Mod.LeniencyHelper.CrossModSupport.GravityHelperImports;

namespace Celeste.Mod.LeniencyHelper.Tweaks
{
    public class SuperOverWalljump
    {
        public static void LoadHooks()
        {
            IL.Celeste.Player.DashUpdate += PreventStupidThing;
            IL.Celeste.Player.RedDashUpdate += PreventStupidThing;
        }
        public static void UnloadHooks()
        {
            IL.Celeste.Player.DashUpdate -= PreventStupidThing;
            IL.Celeste.Player.RedDashUpdate -= PreventStupidThing;
        }
        private static void PreventStupidThing(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            VariableDefinition wjCheckDir = new VariableDefinition(il.Import(typeof(int)));
            il.Body.Variables.Add(wjCheckDir);

            while (c.TryGotoNext(MoveType.Before, instr => instr.MatchCallvirt<Player>("WallJump")))
            {
                if (c.TryGotoPrev(MoveType.Before, instr => instr.MatchCallvirt<Player>("WallJumpCheck")))
                {
                    c.EmitDup();
                    c.EmitStloc(wjCheckDir);
                    c.Index++;

                    c.EmitLdarg0();
                    c.EmitLdloc(wjCheckDir);
                    c.EmitDelegate(CanWJ);
                    c.EmitAnd();
                }
                c.GotoNext(MoveType.After, instr => instr.MatchCallvirt<Player>("WallJump"));
            }
        }
        private static bool CanWJ(Player player, int dir)
        {
            var s = LeniencyHelperModule.Session;
            if (!s.TweaksEnabled["SuperOverWalljump"] || ClimbJumpCheck(player, dir) || Math.Abs(player.DashDir.Y) > 0.2f) return true;

            int trueSign = Math.Sign(player.Speed.X) == 0 ? Math.Sign(player.DashDir.X) : Math.Sign(player.Speed.X);
            Vector2 groundOffset = new Vector2(trueSign * s.wjDist, s.savedCornerCorrection * -currentGravity);

            foreach (LedgeBlocker component in player.Scene.Tracker.GetComponents<LedgeBlocker>())
                if (component.HopBlockCheck(player)) return true;  

            return (!player.onGround && !player.OnGround(player.Position + groundOffset, s.savedCornerCorrection)) || 
                player.OnGround(player.Position + groundOffset - Vector2.UnitY * currentGravity);
        }
        private static bool ClimbJumpCheck(Player player, int dir)
        {
            return ((int)player.Facing == dir && Input.GrabCheck && player.Stamina > 0f && player.Holding == null &&
                !ClimbBlocker.Check(player.Scene, player, player.Position + Vector2.UnitX * 3f * dir));
        }
    }
}