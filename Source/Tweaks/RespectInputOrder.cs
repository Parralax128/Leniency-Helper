using Monocle;
using System.Collections.Generic;
using static Celeste.Mod.LeniencyHelper.Module.LeniencyHelperModule;
using Celeste.Mod.LeniencyHelper.Triggers;

namespace Celeste.Mod.LeniencyHelper.Tweaks;

public class RespectInputOrder : AbstractTweak<RespectInputOrder>
{
    [OnLoad]
    public static void LoadHooks()
    {
        On.Monocle.MInput.Update += OnInputUpdate;
        Everest.Events.Level.OnAfterUpdate += DequeueInputs;
    }
    [OnUnload]
    public static void UnloadHooks()
    {
        On.Monocle.MInput.Update -= OnInputUpdate;
        Everest.Events.Level.OnAfterUpdate -= DequeueInputs;
    }

    
    private static Queue<List<Inputs>> Queue = new();
    private static void OnInputUpdate(On.Monocle.MInput.orig_Update orig)
    {
        orig();
        if (Engine.Scene is not Level) return;

        List <Inputs> pressed = new();
        if(BindPressed(Input.Jump)) pressed.Add(Inputs.Jump);
        if(BindPressed(Input.Dash)) pressed.Add(Inputs.Dash);
        if(BindPressed(Input.CrouchDash)) pressed.Add(Inputs.Demo);
        if(BindPressed(Input.Grab) && GetSetting<bool>("affectGrab")) pressed.Add(Inputs.Grab);
        
        if(pressed.Count > 0) Queue.Enqueue(pressed);

        if (!Enabled || Queue.Count == 0) return;

        List<Inputs> current = Queue.Peek();

        Input.Jump.consumed = !current.Contains(Inputs.Jump);
        Input.Dash.consumed = !current.Contains(Inputs.Dash);
        Input.CrouchDash.consumed = !current.Contains(Inputs.Demo);
        if (GetSetting<bool>("affectGrab")) Input.Grab.consumed = !current.Contains(Inputs.Grab);
    }

    public static bool BindPressed(VirtualButton button)
    {
        if (button.Binding.Pressed(button.GamepadIndex, button.Threshold))
        {
            return true;
        }

        foreach (VirtualButton.Node node in button.Nodes)
        {
            if (node.Pressed)
                return true;
        }

        return false;
    }
    private static void DequeueInputs(Level level)
    {
        if (Queue.Count == 0) return;

        foreach(Inputs input in Queue.Peek())
        {
            if(InputRequiresBlockboostTrigger.EnumInputToGameInput(input).bufferCounter <= 0f)
            {
                Queue.Dequeue();
                return;
            }
        }
    }
}