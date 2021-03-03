using System;
using System.Net.Http;

var numberOfSuccessRequestToPerform = 1000;
var numberOfFailureRequestToPerform = 500;
var successSwitch = true;
var successCount = 0;
var failureCount = 0;

var client = new HttpClient { BaseAddress = new Uri("https://reliableappmh.azurewebsites.net/") };

Console.Clear();

Console.SetCursorPosition(2, 4);
Console.ForegroundColor = ConsoleColor.Gray;
Console.WriteLine(String.Format("{0,-10} :", "Success Count"));
Console.SetCursorPosition(2, 6);
Console.ForegroundColor = ConsoleColor.Gray;
Console.WriteLine(String.Format("{0,-10} :", "Failure Count"));

while (!Console.KeyAvailable)
{
    if(Console.KeyAvailable && Console.ReadKey(true).Key  == ConsoleKey.Escape)
    {
        break;
    }

    var response = await client.SendAsync(new HttpRequestMessage(
        HttpMethod.Get, successSwitch ? "/api/values" : "/api/values/raiseError"));
    if (successSwitch)
    {
        ++successCount;
        Console.SetCursorPosition(20, 4);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(String.Format(" {0,5}", successCount));
        if (successCount % numberOfSuccessRequestToPerform == 0)
        {
            successSwitch = false;
        }
    }
    else
    {
        ++failureCount;
        Console.SetCursorPosition(20, 6);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(String.Format(" {0,5}", failureCount));
        if (failureCount % numberOfFailureRequestToPerform == 0)
        {
            successSwitch = true;
        }
    }
}
Console.ResetColor();


