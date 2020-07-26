using Godot;
using System;
using Godot.Collections;

enum PlayerState
{
    Idle,
    Moving,

}


public class PlayerLogic : Node
{
    //saves the next direction the snake will go

    [Signal]
    public delegate void ChangedPosition(Vector2 newPos);

    [Signal]
    public delegate void GrowTail(Vector2 tailEnd);


    Vector2 nextCommand = Vector2.NegOne;


    public Vector2 logicPos = new Vector2(0, 0);
    public Vector2 nextTargetPos = new Vector2(0, 0);

    //first is head; last is only used to remove edge cases; not actual object
    public Array<Vector2> tail;

    PlayerState state = PlayerState.Idle;

    public float tParam;

    Vector2 currentDir = new Vector2(1, 0);

    public float speed = 2f;

    InputManager inputManager;

    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var node = (InputManager)GetNode("/root/Main/Input");
        inputManager = node;
        inputManager.Connect(nameof(InputManager.NewInput), this, nameof(changeNextCommand));

        tail = new Array<Vector2>();
        tail.Add(logicPos);
        tail.Add(Vector2.NegOne);

        var world = (LogicWorld)GetNode("/root/Main/LogicWorld");
        world.Connect(nameof(LogicWorld.EatApple), this, nameof(EatApple));
    }

    // set everything to the pos of successor
    void updateTail(Vector2 newHeadPos)
    {
        tail[0] = newHeadPos;
        var oldTailPos = tail[tail.Count - 1];

        for (int i = tail.Count - 1; i > 0; i--)
        {
            tail[i] = tail[i - 1];
        }


    }




    private void changeNextCommand(Vector2 dir)
    {
        nextCommand = dir;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        tParam = tParam + delta * speed;
        if (tParam >= 1)
        {
            onTargetReached();
        }

        if (state == PlayerState.Idle)
        {
            setNextTarget();
        }

    }

    private void onTargetReached()
    {
        state = PlayerState.Idle;
        updateTail(nextTargetPos);
        logicPos = nextTargetPos;
        EmitSignal(nameof(ChangedPosition), logicPos);

    }

    public void EatApple(Vector2 pos)
    {
        //grow snake
        tail.Add(Vector2.NegOne);
        EmitSignal(nameof(GrowTail), tail[tail.Count - 2]);
        GD.Print("old", tail[tail.Count - 2]);
        GD.Print("last", tail);
    }


    private void setNextTarget()
    {

        if (nextCommand != Vector2.NegOne && nextCommand != -currentDir)
        {
            currentDir = nextCommand;
        }

        nextTargetPos = logicPos + currentDir;
        tParam = 0;
        state = PlayerState.Moving;
    }

}
