using Godot;

public class InputManager : Spatial
{



    public override void _Ready()
    {





    }


    [Signal]
    public delegate void NewInput(Vector2 dir);


    private Vector2 currentInput = Vector2.NegOne;


    public override void _Process(float delta)
    {

        var LEFT = Input.IsActionPressed("ui_left");
        var RIGHT = Input.IsActionPressed("ui_right");
        var UP = Input.IsActionPressed("ui_up");
        var DOWN = Input.IsActionPressed("ui_down");
        Vector2 direction = Vector2.NegOne;

        if (LEFT)
        {
            direction = new Vector2(1, 0);
        }
        else if (RIGHT)
        {
            direction = new Vector2(-1, 0);
        }
        else if (UP)
        {
            direction = new Vector2(0, 1);
        }
        else if (DOWN)
        {
            direction = new Vector2(0, -1);
        }
        else
        {
            currentInput = Vector2.NegOne;
            return;
        }

        if (currentInput == direction)
        {
            return;
        }
        currentInput = direction;
        EmitSignal(nameof(NewInput), direction);











    }



}