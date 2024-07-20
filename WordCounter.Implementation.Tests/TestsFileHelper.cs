﻿namespace WordCounter.Implementation.Tests;

public static class TestsFileHelper
{
    private const string TestDirectory = "TestDirectory";  
    public static void CreateDirectory()
    {
        if(Directory.Exists(TestDirectory))
            Directory.Delete(TestDirectory, true);

        Directory.CreateDirectory(TestDirectory);
    }

    public static string CreateFileAndGetPath(string fileName)
    {
        var filePath = GetFilePath(fileName);
        using var _ = File.Create(Path.Combine(TestDirectory, fileName));
        return filePath;
    }
    
    public static async Task<string> CreateFileAndGetPath(string fileName, string content)
    {
        var filePath = GetFilePath(fileName);
        await using var fileStream = File.Create(Path.Combine(TestDirectory, fileName));
        await using var streamWriter = new StreamWriter(fileStream);
        await streamWriter.WriteAsync(content);
        return filePath;
    }

    public static void DeleteIfExists(string fileName)
    {
        var path = GetFilePath(fileName);
        if (File.Exists(path))
            File.Delete(path);
    }
    
    public static string GetFilePath(string fileName)
        => Path.Combine(TestDirectory, fileName);
}