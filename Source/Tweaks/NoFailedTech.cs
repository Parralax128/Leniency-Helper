using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

class NoFailedTech : AbstractTweak<NoFailedTech>
{
    static Timer ProtectedDashAttackTimer = new();
    [SaveState] static bool crouchDash;
    [SaveState] static bool downDiag;


    static ILHook dashCoroutineHook;

    [OnLoad]
    public static void LoadHooks()
    {
        On.Celeste.Player.Jump += JumpToTech;
        On.Celeste.Player.DashEnd += StartProtectionTimer;
        dashCoroutineHook = new ILHook(typeof(Player).GetMethod("DashCoroutine",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetStateMachineTarget(), GetDashDir);
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        On.Celeste.Player.Jump -= JumpToTech;
        On.Celeste.Player.DashEnd -= StartProtectionTimer;
        dashCoroutineHook.Dispose();
    }

    static void JumpToTech(On.Celeste.Player.orig_Jump orig, Player self, bool particles = true, bool playSfx = true)
    {
        if (Enabled && self.CanUnDuck && self.DashDir.X != 0 && self.OnGround() && ProtectedDashAttackTimer)
        {
            self.Ducking = crouchDash || downDiag;
            self.SuperJump();

            ProtectedDashAttackTimer.Abort();
            return;
        }

        orig(self, particles, playSfx);
    }

    static void GetDashDir(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);

        if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchCallvirt<Player>("CorrectDashPrecision")))
        {
            cursor.EmitDup();
            cursor.EmitDelegate(SaveDashDir);
        }


        static void SaveDashDir(Vector2 dir) => downDiag = dir.X != 0f && dir.Y > 0f; 
    }
    

    static void StartProtectionTimer(On.Celeste.Player.orig_DashEnd orig, Player self)
    {
        orig(self);

        ProtectedDashAttackTimer.Launch(GetSetting<Time>());
        crouchDash = self.Ducking;
    }
}