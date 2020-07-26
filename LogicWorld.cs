using Godot;
using System;

enum FieldType
{
    Empty,
    Apple

}

public class LogicWorld : Node
{
    int worldSizeX = 20;
    int worldSizeY = 20;
    FieldType[,] world;

    PlayerLogic player;

    Timer timer;


    Random rnd = new Random();


    int appleCounter = 0;

    int maxApples = 30;

    // Declare member variables here. Examples:

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        world = new FieldType[worldSizeX, worldSizeY];

        for (int x = 0; x < world.GetLength(0); x++)
        {
            for (int y = 0; y < world.GetLength(0); y++)
            {
                world[x, y] = FieldType.Empty;
            }
        }


        timer = (Timer)GetNode("Timer");
        timer.Connect("timeout", this, nameof(onTick));

        player = (PlayerLogic)GetNode("/root/Main/LogicWorld/PlayerLogic");
        player.Connect(nameof(PlayerLogic.ChangedPosition), this, nameof(newHeadPosition));

    }


    [Signal]
    public delegate void EatApple(Vector2 pos);


    [Signal]
    public delegate void SpawnApple(Vector2 pos);

    void onTick()
    {
        if (appleCounter >= maxApples)
        {
            return;
        }
        var spawnPos = LookForSpawnPos();

        EmitSignal(nameof(SpawnApple), spawnPos);
        appleCounter++;
        world[(int)Math.Round(spawnPos.x), (int)Math.Round(spawnPos.y)] = FieldType.Apple;
    }

    /* 3 possibilities
        hits wall
        eats apple
        nothing
    */

    void newHeadPosition(Vector2 pos)
    {
        if (world[(int)Math.Round(pos.x), (int)Math.Round(pos.y)] == FieldType.Apple)
        {
            EmitSignal(nameof(EatApple), pos);
            world[(int)Math.Round(pos.x), (int)Math.Round(pos.y)] = FieldType.Empty;
            appleCounter--;
        }
    }


    Vector2 LookForSpawnPos()
    {
        Vector2 foundPos = Vector2.NegOne;
        while (foundPos == Vector2.NegOne)
        {
            int x = rnd.Next(0, worldSizeX - 1);
            int y = rnd.Next(0, worldSizeY - 1);

            if (world[x, y] == FieldType.Apple)
            {
                continue;
            }

            foundPos = new Vector2(x, y);

        }
        return foundPos;

    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
