using System;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Monocle;
using Celeste.Mod.ShroomHelper.Entities;
using System.Linq;
using Celeste.Mod.LeniencyHelper.Module;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class IceWallIncreaseWallLeniency : AbstractTweak
{
    [OnLoad]
    public static void LoadHooks()
    {
        IL.Celeste.Player.WallJumpCheck += CustomWJCheck;
        //On.Celeste.Player.Render += Debug;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        IL.Celeste.Player.WallJumpCheck -= CustomWJCheck;
    }

    private static int GetNewLeniency(int defaultValue, Player player, int dir, bool returnOrig)
    {
        if (returnOrig)
        {
            if (BothDisabled()) SetWjDist(defaultValue, dir);
            return defaultValue;
        }

        int newValue = defaultValue;

        if (Enabled("DynamicWallLeniency") && (Math.Sign(player.Speed.X) != dir || (player.DashAttacking && player.SuperWallJumpAngleCheck)))
        {
            newValue = DynamicWallLeniency.GetDynamicLeniency(player, defaultValue);
        }

        SetWjDist(newValue, dir);

        if (!Enabled("IceWallIncreaseWallLeniency"))
            return newValue;

        int iceLeni = GetSetting<int>("iceWJLeniency");
        for (int c = 0; c < newValue + iceLeni + 1; c++)
        {
            Vector2 at = player.Position + Vector2.UnitX * dir * c;
            if (player.CollideCheck<WallBooster>(at))
            {
                if ((int)player.CollideFirst<WallBooster>(player.Position + Vector2.UnitX * dir * c).Facing == dir)
                {
                    SetWjDist(iceLeni + newValue, dir);
                    return iceLeni + newValue;
                }
            }
            else if (LeniencyHelperModule.ModLoaded("ShroomHelper"))
            {
                if (CollidingAttachedIceWall(player, at, dir, c))
                {
                    SetWjDist(iceLeni + newValue, dir);
                    return iceLeni + newValue;
                }
            }
        }
        return newValue;
    }
    private static bool CollidingAttachedIceWall(Player player, Vector2 at, int dir, int c)
    {
        if (!LeniencyHelperModule.ModLoaded("ShroomHelper")) return false;

        return (player.CollideCheck<AttachedIceWall>(at) &&
            (int)player.CollideFirst<AttachedIceWall>(player.Position + Vector2.UnitX * dir * c).Facing == dir);
    }
    private static void SetWjDist(int value, int dir)
    {
        LeniencyHelperModule.Log($"set {(dir == 1 ? "right" : "left")} wj dist: {value}");

        if (dir == 1) LeniencyHelperModule.Session.wjDistR = value;
        else LeniencyHelperModule.Session.wjDistL = value;
    }

    private static void CustomWJCheck(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);

        ILLabel origStart = il.DefineLabel();
        ILLabel skipRestart = il.DefineLabel();
        ILLabel getWbLeni = il.DefineLabel();

        VariableDefinition trueFlag = new VariableDefinition(il.Import(typeof(bool)));
        VariableDefinition origDist = new VariableDefinition(il.Import(typeof(int)));
        VariableDefinition orig = new VariableDefinition(il.Import(typeof(bool)));
        VariableDefinition savePos = new VariableDefinition(il.Import(typeof(Vector2)));     

        il.Body.Variables.Add(origDist);
        il.Body.Variables.Add(orig);
        il.Body.Variables.Add(trueFlag);
        il.Body.Variables.Add(savePos);

        cursor.EmitDelegate(BothDisabled);
        cursor.EmitStloc(orig);
        cursor.MarkLabel(origStart);        

        if (cursor.TryGotoNext(MoveType.Before, i => i.MatchStloc0()))
        {
            cursor.EmitDup();
            cursor.EmitStloc(origDist);

            cursor.EmitLdarg0();
            cursor.EmitLdarg1();
            cursor.EmitLdloc(orig);
            cursor.EmitDelegate(GetNewLeniency);
            
            cursor.Index++;
        }
        if (cursor.TryGotoNext(MoveType.After, i => i.MatchStloc1()))
        {
            cursor.EmitLdloc1();
            cursor.EmitStloc(trueFlag);
        }
        if (cursor.TryGotoNext(MoveType.Before, i => i.MatchStloc0()))
        {
            cursor.EmitDup();
            cursor.EmitStloc(origDist);

            cursor.EmitLdarg0();
            cursor.EmitLdarg1();
            cursor.EmitLdloc(orig);
            cursor.EmitDelegate(GetNewLeniency);


            cursor.GotoPrev(MoveType.After, i => i.MatchLdcI4(5));
            cursor.MarkLabel(getWbLeni);

            cursor.EmitPop();
            cursor.EmitLdcI4(0);
            cursor.EmitStloc(trueFlag);
            cursor.EmitLdcI4(5);
        }

        cursor.GotoNext(MoveType.Before, i => i.MatchLdarg0(), i => i.MatchLdarg1(), i => i.MatchCallvirt<Player>("ClimbBoundsCheck"));
        cursor.Index++;

        cursor.EmitLdloc(orig);
        cursor.EmitBrtrue(skipRestart);

        cursor.EmitLdloc(trueFlag);
        cursor.EmitBrtrue(getWbLeni);

        cursor.EmitLdloc0();
        cursor.EmitLdloc(origDist);
        cursor.EmitLdarg1();
        cursor.EmitDelegate(MovePlayer);
        cursor.EmitStloc(savePos);

        cursor.EmitLdcI4(1);
        cursor.EmitStloc(orig);
        cursor.EmitBr(origStart);

        cursor.MarkLabel(skipRestart);

        while (cursor.TryGotoNext(MoveType.Before, i => i.MatchRet()))
        {
            cursor.EmitLdarg0();
            cursor.EmitLdloc(savePos);
            cursor.EmitDelegate(ReturnOrigPos);
            cursor.Index++;
        }
    }
    private static bool BothDisabled()
    {
        return (!Enabled("IceWallIncreaseWallLeniency") && !Enabled("DynamicWallLeniency"));
    }
    private static void ReturnOrigPos(Player player, Vector2 pos)
    {
        if (Enabled("IceWallIncreaseWallLeniency") || Enabled("DynamicWallLeniency"))
        {
            player.Position = pos;
        }
    }
    private static Vector2 MovePlayer(Player player, int newWjDist, int origWjDist, int dir)
    {
        Vector2 savePos = player.Position;

        origWjDist--;
        foreach (Solid s in player.Scene.Tracker.GetEntities<Solid>())
        {
            if (s.Collider is Hitbox hb)
            {
                if (player.Collider.AbsoluteTop > hb.AbsoluteBottom || hb.AbsoluteTop > player.Collider.AbsoluteTop) continue;

                
                float leftDist = hb.AbsoluteLeft - player.Collider.AbsoluteRight;
                float rightDist = player.Collider.AbsoluteLeft - hb.AbsoluteRight;

                if (dir>0 && leftDist > 0 && leftDist < newWjDist)
                {
                    player.Position.X += (leftDist - origWjDist);
                    break;
                }
                else if (dir<0 && rightDist > 0 && rightDist < newWjDist)
                {
                    player.Position.X -= (rightDist - origWjDist);
                    break;
                }
            }
            else if (s.Collider is Grid grid)
            {
                Rectangle rect = new Rectangle(
                    (int)player.Collider.AbsoluteLeft - (dir<0? newWjDist : 0),
                    (int)player.Collider.AbsoluteTop,
                    ((int)player.Collider.Width + newWjDist),
                    (int)player.Collider.Height);


                bool isNotSolidTiles = !(s is SolidTiles);

                if (isNotSolidTiles && !rect.Intersects(grid.Bounds)) continue;

                int x = (int)((rect.Left - grid.AbsoluteLeft) / grid.CellWidth);
                int y = (int)((rect.Top - grid.AbsoluteTop) / grid.CellHeight);
                int width = grid.CellsX - (x + (int)((grid.AbsoluteRight - rect.Right) / 8));
                int height = grid.CellsY - (y + (int)((grid.AbsoluteBottom - rect.Bottom) / 8));

                if (isNotSolidTiles)
                {
                    width = (int)((rect.Right - grid.AbsoluteLeft - 1f) / grid.CellWidth) - x + 1;
                    height = (int)((rect.Bottom - grid.AbsoluteTop - 1f) / grid.CellHeight) - y + 1;

                    if (x < 0)
                    {
                        width += x;
                        x = 0;
                    }

                    if (y < 0)
                    {
                        height += y;
                        y = 0;
                    }

                    if (x + width > grid.CellsX)
                        width = grid.CellsX - x;

                    if (y + height > grid.CellsY)
                        height = grid.CellsY - y;
                }

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        if (grid.Data[x + i, y + j])
                        {
                            if (dir > 0)
                            {
                                player.Position.X += (int)((int)(grid.AbsoluteLeft + grid.CellWidth * (x + i)) - player.Collider.AbsoluteRight) - origWjDist;
                                return savePos;
                            }
                            else player.Position.X -= (int)(player.Collider.AbsoluteLeft - ((int)(grid.AbsoluteLeft + grid.CellWidth * (x + i)) + grid.CellWidth)) - origWjDist;

                            break;
                        }
                    }
                }
            }
        }
        return savePos;
    }
    private static (Rectangle, Color)[] debugRects = Array.Empty<(Rectangle, Color)>();
    public static void AddRect(Rectangle rect, Color color)
    {
        debugRects = debugRects.Append((rect, color)).ToArray();
    }
    private static void Debug(On.Celeste.Player.orig_Render orig, Player self)
    {
        orig(self);

        if (debugRects.Length > 0)
            foreach ((Rectangle r, Color c) in debugRects)
                Draw.HollowRect(r, c);

        if(Input.Grab.Pressed)
            debugRects = Array.Empty<(Rectangle, Color)>();
    }
}