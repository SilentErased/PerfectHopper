using ImGuiNET;
using RenderThing;
using SilentExternal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Numerics;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
// @ pls ignore bad / shit code :pleading-face:
public class PerfectHopper
{
    static HttpClient Client = new HttpClient();
    static void killeveryrobloxprocess()
    {
        List<string> tokill = new List<string>{ "RobloxPlayerBeta", "Bloxstrap", "Fishstrap", "Voidstrap" };
        foreach (Process TargetProcess in Process.GetProcesses())
        {
            if (tokill.Contains(TargetProcess.ProcessName))
            {
                TargetProcess.Kill();
            }
        }
    }
    static async Task<List<string>> GetServers()
    {
        List<string> servers = new List<string>();
        while (true)
        {
            try
            {
                var response = await Client.GetAsync("https://games.roblox.com/v1/games/1537690962/servers/Public?cursor=&sortOrder=Asc&excludeFullGames=true");
                if ((int)response.StatusCode == 429)
                {
                    await Task.Delay(5000);
                    continue;
                }
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var root = JsonSerializer.Deserialize<Root>(json);
                foreach (var server in root.data)
                {
                    servers.Add(server.id);
                }
                break;
            }
            catch
            {
                await Task.Delay(5000);
            }
        }
        return servers;
    }
    static void Main(string[] args)
    {
        Console.Title = "PerfectHopper v0.1";
        Console.WriteLine("PerfectHopper Console (Logs)");
        Console.WriteLine("----------------------------");
        Renderring Render = new Renderring();
        Thread ohio = new Thread(() => Render.Start().Wait());
        ohio.Start();
        while (true)
        {
            if (Render.HopId > 0 && Render.HopId != 3)
            {
                killeveryrobloxprocess();
                string TargetName = "";
                if (Render.HopId == 1)
                {
                    TargetName = "Vicious Bee";
                }
                else if (Render.HopId == 2)
                {
                    TargetName = "Windy Bee";
                }
                Console.WriteLine($"Starting new {TargetName} hop cycle.");
                Console.WriteLine("Fetching Servers");
                var Servers = GetServers().GetAwaiter().GetResult();
                Console.WriteLine("Servers fetched: " + Servers.Count);

                foreach (string Id in Servers)
                {
                    if (Render.HopId <= 0 || Render.HopId == 3) return;
                    Console.WriteLine($"Current Server (JOBID): {Id}");
                    string Uri = "roblox://experiences/start?placeId=1537690962&gameInstanceId=" + Id;
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = Uri,
                        UseShellExecute = true
                    });
                    Thread.Sleep(3000);
                    if (Render.HopId <= 0 || Render.HopId == 3) return;
                    var sw = Stopwatch.StartNew();
                    bool loaded = false;
                    while (sw.Elapsed.TotalSeconds < 15)
                    {
                        if (Render.HopId <= 0 || Render.HopId == 3) return;
                        Process[] procs = Process.GetProcessesByName("RobloxPlayerBeta");
                        if (procs.Length == 0)
                        {
                            Thread.Sleep(500);
                            continue;
                        }
                        try
                        {
                            RobloxObject DataModel = new RobloxObject(RobloxMemController.GetDataModel());
                            if (DataModel == null || DataModel.Address == UIntPtr.Zero)
                            {
                                Thread.Sleep(500);
                                continue;
                            }
                            var workspacePtr = DataModel.FindFirstChild("Workspace");
                            if (workspacePtr == UIntPtr.Zero)
                            {
                                Thread.Sleep(500);
                                continue;
                            }
                            loaded = true;
                            break;
                        }
                        catch
                        {
                            Thread.Sleep(500);
                            continue;
                        }
                    }
                    if (!loaded)
                    {
                        Console.WriteLine("Failed. Roblox not found");
                        continue;
                    }
                    Thread.Sleep(5000);
                    if (Render.HopId <= 0 || Render.HopId == 3) return;
                    Console.WriteLine($"Checking if {TargetName} on server.");
                    Console.WriteLine("Step 1/3 (Finding workspace)");
                    RobloxObject DataModel2 = new RobloxObject(RobloxMemController.GetDataModel());
                    Console.WriteLine(DataModel2.Address.ToString() + " - DataModel Address");
                    RobloxObject Workspace = new RobloxObject(DataModel2.FindFirstChild("Workspace"));
                    Console.WriteLine(Workspace.Address.ToString() + " - Workspace Address");
                    Console.WriteLine("Step 2/3 (Findong Monsters Folder)");
                    RobloxObject Monsters = new RobloxObject(Workspace.FindFirstChild("Monsters"));
                    Console.WriteLine(Monsters.Address.ToString() + " - Monsters Folder Address");
                    Console.WriteLine($"Step 3/3 (Checking if {TargetName} in monsters folder)");
                    try
                    {
                        foreach (RobloxObject CurrentObject in Monsters.GetChildren())
                        {
                            try
                            {
                                Console.WriteLine(CurrentObject.Name);
                                if (CurrentObject.Name.IndexOf(TargetName, StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    Console.WriteLine($"{TargetName} found! Stopping hop.");
                                    Render.HopId = 0;
                                    break;
                                }
                            }
                            catch
                            {
                                Console.WriteLine("Error while reading object");
                                continue;
                            }
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Failed getting childrens");
                        continue;
                    }
                    if (Render.HopId == 0)
                    {
                        break;
                    }
                    Thread.Sleep(5000);
                    foreach (var TargetProcess in Process.GetProcessesByName("RobloxPlayerBeta"))
                    {
                        TargetProcess.Kill();
                    }
                }
            }
            if (Render.HopId == 3)
            {
                killeveryrobloxprocess();
                Console.WriteLine($"Starting new Searcher hop cycle.");
                Console.WriteLine("Fetching Servers");
                var Servers = GetServers().GetAwaiter().GetResult();
                Console.WriteLine("Servers fetched: " + Servers.Count);

                foreach (string Id in Servers)
                {
                    if (Render.HopId != 3) break;
                    Console.WriteLine($"Current Server (JOBID): {Id}");
                    string Uri = "roblox://experiences/start?placeId=1537690962&gameInstanceId=" + Id;
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = Uri,
                        UseShellExecute = true
                    });
                    Thread.Sleep(3000);
                    if (Render.HopId <= 0 || Render.HopId == 3) return;
                    var sw = Stopwatch.StartNew();
                    bool loaded = false;
                    while (sw.Elapsed.TotalSeconds < 15)
                    {
                        if (Render.HopId <= 0 || Render.HopId == 3) return;
                        Process[] procs = Process.GetProcessesByName("RobloxPlayerBeta");
                        if (procs.Length == 0)
                        {
                            Thread.Sleep(500);
                            continue;
                        }
                        try
                        {
                            RobloxObject DataModelz = new RobloxObject(RobloxMemController.GetDataModel());
                            if (DataModelz == null || DataModelz.Address == UIntPtr.Zero)
                            {
                                Thread.Sleep(500);
                                continue;
                            }
                            var workspacePtr = DataModelz.FindFirstChild("Workspace");
                            if (workspacePtr == UIntPtr.Zero)
                            {
                                Thread.Sleep(500);
                                continue;
                            }
                            loaded = true;
                            break;
                        }
                        catch
                        {
                            Thread.Sleep(500);
                            continue;
                        }
                    }
                    if (!loaded)
                    {
                        Console.WriteLine("Failed. Roblox not found");
                        continue;
                    }
                    Thread.Sleep(5000);
                    if (Render.HopId != 3) break;
                    Process[] Test = Process.GetProcessesByName("RobloxPlayerBeta");
                    if (Test.Length == 0) continue;
                    Console.WriteLine("Step 1/3 (Finding workspace)");
                    RobloxObject DataModel = new RobloxObject(RobloxMemController.GetDataModel());
                    Console.WriteLine(DataModel.Address.ToString() + " - DataModel Address");
                    RobloxObject Workspace = new RobloxObject(DataModel.FindFirstChild("Workspace"));
                    Console.WriteLine(Workspace.Address.ToString() + " - Workspace Address");
                    Console.WriteLine("Step 2/3 (Findong Monsters Folder)");
                    RobloxObject Monsters = new RobloxObject(Workspace.FindFirstChild("Monsters"));
                    Console.WriteLine(Monsters.Address.ToString() + " - Monsters Folder Address");
                    Console.WriteLine($"Step 3/3 (Checking if windy bee or vicious bee in monsters folder)");
                    try
                    {
                        foreach (RobloxObject CurrentObject in Monsters.GetChildren())
                        {
                            try
                            {
                                Console.WriteLine(CurrentObject.Name);
                                if (CurrentObject.Name.IndexOf("Vicious Bee", StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    Console.WriteLine($"Vicious bee found!");
                                    DiscordLogger.SendEmbedAsync(Render.webhook, "Vicious Bee", "Join Method: " + Uri, "#000000", "https://static.wikia.nocookie.net/bee-swarm-simulator/images/1/1f/Vicious_Bee.png/revision/latest/scale-to-width-down/150?cb=20200404000135");
                                }
                                if (CurrentObject.Name.IndexOf("Windy Bee", StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    Console.WriteLine($"Windy Bee found!");
                                    DiscordLogger.SendEmbedAsync(Render.webhook, "Windy Bee", "Join Method: " + Uri, "#FFFFFF", "https://static.wikia.nocookie.net/bee-swarm-simulator/images/8/85/Windy_Bee.png/revision/latest/scale-to-width-down/150?cb=20230415210459");
                                }
                            }
                            catch
                            {
                                Console.WriteLine("Error while reading object");
                                continue;
                            }
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Empty");
                        continue;
                    }
                    if (Render.HopId == 0)
                    {
                        break;
                    }
                    Thread.Sleep(2000);
                }
            }
            Thread.Sleep(100);
        }
    }
}

class Root
{
    public List<Server> data { get; set; }
}

class Server
{
    public string id { get; set; }
}
