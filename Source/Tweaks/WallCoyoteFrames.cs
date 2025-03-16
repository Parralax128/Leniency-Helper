using Monocle;
using Celeste.Mod.LeniencyHelper.Components;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class WallCoyoteFrames
{
    public static void LoadHooks()
    {
        On.Celeste.Player.WallJumpCheck += CoyoteWallJumpCheck;
        On.Celeste.Player.Update += CoyoteWallUpdate;

        On.Celeste.Player.WallJump += CoyoteWallJump;
        On.Celeste.Player.ClimbJump += CoyoteWallClimbJump;
        On.Celeste.Player.SuperWallJump += CoyoteWallSuperWallJump;
    }
    public static void UnloadHooks()
    {
        On.Celeste.Player.WallJumpCheck -= CoyoteWallJumpCheck;
        On.Celeste.Player.Update -= CoyoteWallUpdate;

        On.Celeste.Player.WallJump -= CoyoteWallJump;
        On.Celeste.Player.ClimbJump -= CoyoteWallClimbJump;
        On.Celeste.Player.SuperWallJump -= CoyoteWallSuperWallJump;
    }

    public static void CoyoteWallUpdate(On.Celeste.Player.orig_Update orig, Player self)
    {
        var s = LeniencyHelperModule.Session;
        var settings = LeniencyHelperModule.Settings;

        orig(self);
        if (!LeniencyHelperModule.Session.TweaksEnabled["WallCoyoteFrames"] || s.WCFcomponent == null) return;

        if (s.wallCoyoteTimer > 0)
            s.wallCoyoteTimer -= Engine.DeltaTime;
        else s.wallCoyoteTimer = 0;

        float trueTime = (bool)settings.GetSetting("WallCoyoteFrames", "countWallCoyoteTimeInFrames") ?
            (float)settings.GetSetting("WallCoyoteFrames", "wallCoyoteTime") / Engine.FPS :
            (float)settings.GetSetting("WallCoyoteFrames", "wallCoyoteTime");

        useOrigWJCheck = true;
        if (self.WallJumpCheck(-1) && self.WallJumpCheck(1))
        {
            s.wallCoyoteTimer = trueTime;
            s.WCFcomponent.currentWallCoyoteType = WallCoyoteFramesComponent.WallCoyoteTypes.Both;
        }
        else if (self.WallJumpCheck(-1) && !self.WallJumpCheck(1))
        {
            s.wallCoyoteTimer = trueTime;
            s.WCFcomponent.currentWallCoyoteType = WallCoyoteFramesComponent.WallCoyoteTypes.Right;
        }
        else if (self.WallJumpCheck(1) && !self.WallJumpCheck(-1))
        {
            s.wallCoyoteTimer = trueTime;
            s.WCFcomponent.currentWallCoyoteType = WallCoyoteFramesComponent.WallCoyoteTypes.Left;
        }
        useOrigWJCheck = false;
    }

    public static void CoyoteWallJump(On.Celeste.Player.orig_WallJump orig, Player self, int dir)
    {
        var s = LeniencyHelperModule.Session;
        if (!LeniencyHelperModule.Session.TweaksEnabled["WallCoyoteFrames"]|| s.WCFcomponent == null)
        {
            orig(self, dir);
            return;
        }

        orig(self, dir);

        if (s.wallCoyoteTimer > 0 &&
            s.WCFcomponent.currentWallCoyoteType == WallCoyoteFramesComponent.WallCoyoteTypes.Both)
            self.Speed.X = 0f;

        s.wallCoyoteTimer = 0f;
    }
    public static void CoyoteWallClimbJump(On.Celeste.Player.orig_ClimbJump orig, Player self)
    {
        var s = LeniencyHelperModule.Session;
        if (!LeniencyHelperModule.Session.TweaksEnabled["WallCoyoteFrames"] || s.WCFcomponent is null)
        {
            orig(self);
            return;
        }
        orig(self);
        s.wallCoyoteTimer = 0f;
    }
    public static void CoyoteWallSuperWallJump(On.Celeste.Player.orig_SuperWallJump orig, Player self, int dir)
    {
        var s = LeniencyHelperModule.Session;
        if (!LeniencyHelperModule.Session.TweaksEnabled["WallCoyoteFrames"]|| s.WCFcomponent == null)
        {
            orig(self, dir);
            return;
        }

        orig(self, dir);

        if (s.wallCoyoteTimer > 0 &&
            s.WCFcomponent.currentWallCoyoteType == WallCoyoteFramesComponent.WallCoyoteTypes.Both)
            self.Speed.X = 0f;

        s.wallCoyoteTimer = 0f;
    }

    public static bool useOrigWJCheck = false;
    public static bool CoyoteWallJumpCheck(On.Celeste.Player.orig_WallJumpCheck orig, Player self, int dir)
    {
        var s = LeniencyHelperModule.Session;
        if (!LeniencyHelperModule.Session.TweaksEnabled["WallCoyoteFrames"] || useOrigWJCheck)
        {
            return orig(self, dir);
        }
        
        switch (dir)
        {
            case -1:
                return orig(self, -1) || (s.wallCoyoteTimer > 0 &&
                    (s.WCFcomponent.currentWallCoyoteType == WallCoyoteFramesComponent.WallCoyoteTypes.Right ||
                    s.WCFcomponent.currentWallCoyoteType == WallCoyoteFramesComponent.WallCoyoteTypes.Both));
            case 1:
                return orig(self, 1) || (s.wallCoyoteTimer > 0 &&
                    (s.WCFcomponent.currentWallCoyoteType == WallCoyoteFramesComponent.WallCoyoteTypes.Left ||
                    s.WCFcomponent.currentWallCoyoteType == WallCoyoteFramesComponent.WallCoyoteTypes.Both));
        }
        return false;
    }
}