using Godot;
using System;
using System.Collections.Generic;

public class AppleManager : Node
{

    LogicWorld world;

    Timer timer;

    Dictionary<Vector2, AppleGO> appleList;

    public override void _Ready()
    {

        world = (LogicWorld)GetNode("/root/Main/LogicWorld");

        world.Connect(nameof(LogicWorld.EatApple), this, nameof(onEatApple));
        world.Connect(nameof(LogicWorld.SpawnApple), this, nameof(onSpawnApple));
        appleList = new Dictionary<Vector2, AppleGO>();


    }


    void onSpawnApple(Vector2 pos)
    {

        var appleScene = (PackedScene)GD.Load("res://Apple.tscn");

        var apple = (AppleGO)appleScene.Instance();
        apple.Translation = (new Vector3(pos.x, 1.5f, pos.y));

        GetNode("/root/Main/AppleManager").AddChild(apple);
        appleList.Add(pos, apple);
    }


    void onEatApple(Vector2 pos)
    {
        var a = appleList[pos];
        appleList.Remove(pos);
        a.QueueFree();
    }



}