using Godot;
using Godot.Collections;
using System;

public class Player : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    private float speed = 2.0f;
    private Vector3 dir = new Vector3(1, 0, 0);
    private Vector3 velocity = new Vector3();


    private Array<Spatial> tail;


    private float time = 0;

    private PlayerLogic playerModel;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        playerModel = (PlayerLogic)GetNode("/root/Main/LogicWorld/PlayerLogic");
        playerModel.Connect(nameof(PlayerLogic.GrowTail), this, nameof(onGrowTail));
        tail = new Array<Spatial>();
        tail.Add(this);
    }



    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        var t = playerModel.tParam + (delta * playerModel.speed);
        Vector3 logicPos = new Vector3(playerModel.logicPos.x, 1.5f, playerModel.logicPos.y);

        Translation = logicPos.LinearInterpolate(new Vector3(playerModel.nextTargetPos.x, 1.5f, playerModel.nextTargetPos.y), t);


        for (int i = 1; i < playerModel.tail.Count - 1; i++)
        {
            var tailPos = calcPosTailPiece(i);
            tail[i].Translation = tailPos;
        }
    }

    //for every tail part the target is the one in front of it
    void onGrowTail(Vector2 pos)
    {
        var tailPos = calcPosTailPiece(playerModel.tail.Count - 1);

        var tailScn = (PackedScene)GD.Load("res://TailPiece.tscn");
        var tailPiece = (Spatial)tailScn.Instance();
        AddChild(tailPiece);
        tailPiece.SetAsToplevel(true);
        tailPiece.Translation = tailPos;
        tail.Add(tailPiece);


    }


    Vector3 calcPosTailPiece(int index)
    {
        var t = playerModel.tParam;
        Vector2 target;

        if (index == 0)
        {
            target = playerModel.logicPos;
        }
        else if (playerModel.tail.Count > 2)
        {
            target = playerModel.tail[index - 1];
        }
        else
        {
            target = playerModel.logicPos;
        }


        Vector3 logicPos = new Vector3(playerModel.tail[index].x, 1.5f, playerModel.tail[index].y);

        var tailPos = logicPos.LinearInterpolate(new Vector3(target.x, 1.5f, target.y), t);

        return tailPos;
    }





}
