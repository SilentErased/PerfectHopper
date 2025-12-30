using ClickableTransparentOverlay;
using ImGuiNET;
using System;
using System.Numerics;

namespace RenderThing
{
    internal class Renderring : Overlay
    {
        int CurrentTab = 0;
        public int HopId = 0;
        Vector3 ThemeColor = new Vector3(0.2f, 0.5f, 0.85f);
        public string webhook = "";
        bool searcherStarted = false;

        protected override void Render()
        {
            Vector4 windowBg = new Vector4(ThemeColor.X * 0.12f, ThemeColor.Y * 0.12f, ThemeColor.Z * 0.12f, 0.95f);
            Vector4 borderCol = new Vector4(ThemeColor.X * 0.6f, ThemeColor.Y * 0.6f, ThemeColor.Z * 0.6f, 1f);
            Vector4 titleBar = new Vector4(ThemeColor.X, ThemeColor.Y, ThemeColor.Z, 1f);
            Vector4 tabCol = new Vector4(ThemeColor.X * 0.18f, ThemeColor.Y * 0.18f, ThemeColor.Z * 0.18f, 1f);
            Vector4 tabActiveCol = titleBar;
            Vector4 tabHoverCol = new Vector4(MathF.Min(ThemeColor.X * 1.15f, 1f), MathF.Min(ThemeColor.Y * 1.15f, 1f), MathF.Min(ThemeColor.Z * 1.15f, 1f), 1f);
            Vector4 buttonCol = titleBar;
            Vector4 buttonHover = new Vector4(MathF.Min(buttonCol.X + 0.12f, 1f), MathF.Min(buttonCol.Y + 0.12f, 1f), MathF.Min(buttonCol.Z + 0.12f, 1f), 1f);

            ImGui.PushStyleColor(ImGuiCol.WindowBg, windowBg);
            ImGui.PushStyleColor(ImGuiCol.Border, borderCol);
            ImGui.PushStyleColor(ImGuiCol.TitleBg, titleBar);
            ImGui.PushStyleColor(ImGuiCol.TitleBgActive, titleBar);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 8f);
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 6f);
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(6f, 6f));

            ImGui.Begin("PerfectHopper v0.1");

            if (ImGui.BeginTabBar("MainTabs"))
            {
                ImGui.PushStyleColor(ImGuiCol.Tab, tabCol);
                ImGui.PushStyleColor(ImGuiCol.TabActive, tabActiveCol);
                ImGui.PushStyleColor(ImGuiCol.TabHovered, tabHoverCol);

                if (ImGui.BeginTabItem("Vicious Bee"))
                {
                    CurrentTab = 1;
                    ImGui.TextUnformatted("Find servers with Vicious Bee fast");
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Windy Bee"))
                {
                    CurrentTab = 2;
                    ImGui.TextUnformatted("Find servers with Windy Bee fast");
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Searcher"))
                {
                    CurrentTab = 3;
                    ImGui.TextUnformatted(@"Find servers with Vicious
or Windy Bee and send to
Discord webhook");
                    ImGui.InputText("Webhook", ref webhook, 1000);
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Choose Color"))
                {
                    ImGui.TextUnformatted("Adjust theme color (R / G / B)");
                    ImGui.Dummy(new Vector2(0, 6f));
                    ImGui.PushItemWidth(180f);
                    ImGui.SliderFloat("R##theme", ref ThemeColor.X, 0f, 1f);
                    ImGui.SliderFloat("G##theme", ref ThemeColor.Y, 0f, 1f);
                    ImGui.SliderFloat("B##theme", ref ThemeColor.Z, 0f, 1f);
                    ImGui.PopItemWidth();
                    ImGui.Dummy(new Vector2(0, 6f));
                    ImGui.TextUnformatted("Preview:");
                    ImGui.SameLine();
                    ImGui.PushStyleColor(ImGuiCol.Button, buttonCol);
                    ImGui.Button("    ");
                    ImGui.PopStyleColor();
                    ImGui.EndTabItem();
                }

                ImGui.PopStyleColor(3);
                ImGui.EndTabBar();
            }

            ImGui.Dummy(new Vector2(0, 4));

            if (HopId == 0)
            {
                ImGui.PushStyleColor(ImGuiCol.Button, buttonCol);
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, buttonHover);
                if (ImGui.Button("Start", new Vector2(110f, 26f)))
                {
                    HopId = CurrentTab;
                    if (HopId == 3 && !searcherStarted)
                    {
                        searcherStarted = true;
                        DiscordLogger.SendEmbedAsync(webhook, "Started Searching", "PerfectHopper v0.1", "#2bfb45");
                    }
                }
                ImGui.PopStyleColor(2);
            }
            else
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.85f, 0.35f, 0.35f, 1f));
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.95f, 0.45f, 0.45f, 1f));
                if (ImGui.Button("Stop", new Vector2(110f, 26f)))
                {
                    HopId = 0;
                    searcherStarted = false;
                }
                ImGui.PopStyleColor(2);
            }
            ImGui.SameLine();
            ImGui.TextUnformatted(HopId == 0 ? "Target : nil" : HopId == 1 ? "Target : Vicious Bee" : HopId == 2 ? "Target : Windy Bee" : "Searcher Mode");
            ImGui.End();
            ImGui.PopStyleVar(3);
            ImGui.PopStyleColor(4);
        }
    }
}
