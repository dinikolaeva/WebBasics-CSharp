﻿using _01.Chronometer;

var chronometer = new Chronometer();

string line;

while ((line = Console.ReadLine()) != "exit")
{
    if (line == "start")
    {
        Task.Run(() =>
        {
            chronometer.Start();
        });
    }
    else if (line == "stop")
    {
        Task.Run(() =>
        {
            chronometer.Stop();
        });
    }
    else if (line == "lap")
    {
        Task.Run(() =>
        {
            Console.WriteLine(chronometer.Lap());
        });
    }
    else if (line == "laps")
    {
        if (chronometer.Laps.Count == 0)
        {
            Console.WriteLine("Laps: no laps");
            continue;
        }

        Console.WriteLine("Laps: ");

        for (int i = 1; i < chronometer.Laps.Count; i++)
        {
            Console.WriteLine($"{i}. {chronometer.Laps[i]}");
        }
    }
    else if (line == "reset")
    {
        chronometer.Reset();
    }
    else if (line == "time")
    {
        Console.WriteLine(chronometer.GetTime);
    }
}

chronometer.Stop();
