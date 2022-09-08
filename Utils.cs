using System;

namespace HazeMonitoring;

public static class Utils
{
    public static string GetApplicationStage() => Environment.GetEnvironmentVariable("ApplicationStage") ?? "development";
}