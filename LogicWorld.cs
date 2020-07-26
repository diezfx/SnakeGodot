using Godot;
using Godot.Collections;
using System;

enum FieldType
{
    Empty,
    Apple,
    Snake

}


enum GameState
{
    Running,
    Ended
}

public class LogicWorld : Node
{
    int worldSizeX = 20;
    int worldSizeY = 20;

    GameState state;
    FieldType[,] world;

    PlayerLogic player;

    Array<Vector2> tailCache = new Array<Vector2>();

    Timer timer;


    Random rnd = new Random();


    [Signal]
    public delegate void EatApple(Vector2 pos);


    [Signal]
    public delegate void SpawnApple(Vector2 pos);


    [Signal]
    public delegate void GameEnded();

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
        player.Connect(nameof(PlayerLogic.ChangedPosition), this, nameof(newPlayerPosition));


        state = GameState.Running;

    }
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

    void newPlayerPosition(Array<Vector2> tail)
    {

        //todo check hits wall
        // hits itself

        var headPos = tail[0];


        if (headPos.x > worldSizeX || headPos.x < 0 || headPos.y > worldSizeY || headPos.y < 0)
        {
            GD.Print("this");
            EndGame();
            return;
        }

        if (world[(int)Math.Round(headPos.x), (int)Math.Round(headPos.y)] == FieldType.Snake)
        {
            GD.Print("that");
            EndGame();
            return;
        }
        //eats apple
        if (world[(int)Math.Round(headPos.x), (int)Math.Round(headPos.y)] == FieldType.Apple)
        {
            world[(int)Math.Round(headPos.x), (int)Math.Round(headPos.y)] = FieldType.Empty;
            appleCounter--;
            EmitSignal(nameof(EatApple), headPos);
        }

        for (int i = 0; i < tailCache.Count; i++)
        {
            world[(int)Math.Round(tailCache[i].x), (int)Math.Round(tailCache[i].y)] = FieldType.Empty;
        }

        for (int i = 0; i < tail.Count; i++)
        {
            world[(int)Math.Round(tail[i].x), (int)Math.Round(tail[i].y)] = FieldType.Snake;
        }
        tailCache = tail;
    }

    private void EndGame()
    {
        state = GameState.Ended;
        EmitSignal(nameof(GameEnded));
        GetTree().Paused = true;
    }


    Vector2 LookForSpawnPos()
    {
        Vector2 foundPos = Vector2.NegOne;
        while (foundPos == Vector2.NegOne)
        {
            int x = rnd.Next(0, worldSizeX - 1);
            int y = rnd.Next(0, worldSizeY - 1);

            if (world[x, y] != FieldType.Empty)
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
