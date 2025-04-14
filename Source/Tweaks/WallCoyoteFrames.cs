using Monocle;
using Celeste.Mod.LeniencyHelper.Components;
using static Celeste.Mod.LeniencyHelper.SettingMaster;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class WallCoyoteFrames
{
    [OnLoad]
    public static void LoadHooks()
    {
        On.Celeste.Player.WallJumpCheck += CoyoteWallJumpCheck;
        On.Celeste.Player.Update += CoyoteWallUpdate;

        On.Celeste.Player.WallJump += CoyoteWallJump;
        On.Celeste.Player.ClimbJump += CoyoteWallClimbJump;
        On.Celeste.Player.SuperWallJump += CoyoteWallSuperWallJump;
    }
    [OnUnload]
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
        if (!LeniencyHelperModule.Session.Tweaks["WallCoyoteFrames"].Enabled || s.WCFcomponent == null) return;

        if (s.wallCoyoteTimer > 0)
            s.wallCoyoteTimer -= Engine.DeltaTime;
        else s.wallCoyoteTimer = 0;

        float trueTime = GetSetting<bool>("countWallCoyoteTimeInFrames") ? GetSetting<float>("wallCoyoteTime") / Engine.FPS : GetSetting<float>("wallCoyoteTime");

        useOrigWJCheck = true;
        bool wallLeft = self.WallJumpCheck(-1);
        bool wallRight = self.WallJumpCheck(1);
        useOrigWJCheck = false;

        if (wallLeft && wallRight)
        {
            s.wallCoyoteTimer = trueTime;
            s.WCFcomponent.currentWallCoyoteType = WallCoyoteFramesComponent.WallCoyoteTypes.Both;
        }
        else if (wallLeft && !wallRight)
        {
            s.wallCoyoteTimer = trueTime;
            s.WCFcomponent.currentWallCoyoteType = WallCoyoteFramesComponent.WallCoyoteTypes.Right;
        }
        else if (wallRight && !wallLeft)
        {
            s.wallCoyoteTimer = trueTime;
            s.WCFcomponent.currentWallCoyoteType = WallCoyoteFramesComponent.WallCoyoteTypes.Left;
        }
    }

    public static void CoyoteWallJump(On.Celeste.Player.orig_WallJump orig, Player self, int dir)
    {
        var s = LeniencyHelperModule.Session;
        if (!LeniencyHelperModule.Session.Tweaks["WallCoyoteFrames"].Enabled || s.WCFcomponent == null)
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
        if (!LeniencyHelperModule.Session.Tweaks["WallCoyoteFrames"].Enabled || s.WCFcomponent is null)
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
        if (!LeniencyHelperModule.Session.Tweaks["WallCoyoteFrames"].Enabled || s.WCFcomponent == null)
        {
            orig(self, dir);
            return;
        }

        orig(self, dir);

        if (s.wallCoyoteTimer > 0 && s.WCFcomponent.currentWallCoyoteType == WallCoyoteFramesComponent.WallCoyoteTypes.Both)
            self.Speed.X = 0f;

        s.wallCoyoteTimer = 0f;
    }

    public static bool useOrigWJCheck = false;
    public static bool CoyoteWallJumpCheck(On.Celeste.Player.orig_WallJumpCheck orig, Player self, int dir)
    {
        var s = LeniencyHelperModule.Session;
        if (!LeniencyHelperModule.Session.Tweaks["WallCoyoteFrames"].Enabled || useOrigWJCheck)
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