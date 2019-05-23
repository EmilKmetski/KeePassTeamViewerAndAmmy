using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using KeePass.Forms;
using KeePass.Plugins;
using KeePass.Resources;
using KeePass.UI;
using KeePassLib;
using KeePassLib.Security;
using KeePassLib.Utility;
using Newtonsoft.Json;

namespace TeamViewerAndAmmyy
{
    public sealed class TeamViewerAndAmmyyExt : Plugin
    {
        private IPluginHost m_host = null;
        private PlugInApplicationPaths appPaths;
        public override bool Initialize(IPluginHost host)
        {
            if (host == null) return false; // Fail; we need the host
            m_host = host;
            appPaths = GetPaths();

            return true; // Initialization successful
        }
        public override ToolStripMenuItem GetMenuItem(PluginMenuType t)
        {
            // Our menu item below is intended for the main location(s),
            // not for other locations like the group or entry menus
            if (t != PluginMenuType.Entry) return null;
            ToolStripMenuItem teamViewerAndAmmyyMenu = new ToolStripMenuItem("TeamViewer and Ammyy");

            // Add menu item 'Connect with TeamViewer'
            ToolStripMenuItem openWithTeamViewer = new ToolStripMenuItem();
            openWithTeamViewer.Text = "Connect with TeamViewer";
            openWithTeamViewer.Click += this.OnMenuTeamViewer;
            teamViewerAndAmmyyMenu.DropDownItems.Add(openWithTeamViewer);

            // Add menu item 'Connect with Ammyy'
            ToolStripMenuItem openWithAmmyyMenu = new ToolStripMenuItem();
            openWithAmmyyMenu.Text = "Connect with Ammyy";
            openWithAmmyyMenu.Click += this.OnMenuAmmyy;
            teamViewerAndAmmyyMenu.DropDownItems.Add(openWithAmmyyMenu);
            teamViewerAndAmmyyMenu.Enabled = appPaths.TeamViewerExists || appPaths.AmmyyExists;
            
            // By using an anonymous method as event handler, we do not
            // need to remember menu item references manually, and
            // multiple calls of the GetMenuItem method (to show the
            // menu item in multiple places) are no problem
            teamViewerAndAmmyyMenu.DropDownOpening += delegate (object sender, EventArgs e)
            {
            
                PwDatabase pd = m_host.Database;
                bool bOpen = ((pd != null) && pd.IsOpen);

                openWithTeamViewer.Enabled = bOpen && appPaths.TeamViewerExists;
                openWithAmmyyMenu.Enabled = bOpen && appPaths.AmmyyExists;
            };

            return teamViewerAndAmmyyMenu;
        }
        private void OnMenuTeamViewer(object sender, EventArgs e)
        {
            PwDatabase pd = m_host.Database;
            if ((pd == null) || !pd.IsOpen) { Debug.Assert(false); return; }

            PwEntry cureentSelectedItem = m_host.MainWindow.GetSelectedEntry(true);
            RemoteAppCredentials tw = new RemoteAppCredentials();
            ProtectedString protectedStringRemoteID = cureentSelectedItem.Strings.Get("TeamViewerID");
            ProtectedString protectedStringRemotePass = cureentSelectedItem.Strings.Get("TeamViewerPass");
            tw.RemoteID = protectedStringRemoteID == null ? null: protectedStringRemoteID.ReadString();
            tw.RemotePass = protectedStringRemotePass == null ? null : protectedStringRemotePass.ReadString();

            if (tw.RemoteID != null && tw.RemotePass != null)
            {
                StartProcess(appPaths.TeamViewerFullPath, " --id " + tw.RemoteID + " --Password " + tw.RemotePass);
            }
            else
            {
                MessageBox.Show("No saved Team Viewer credentials for this entry!", "Error", MessageBoxButtons.OK,MessageBoxIcon.Error);
            }

        }
        private void OnMenuAmmyy(object sender, EventArgs e)
        {
            PwDatabase pd = m_host.Database;
            if ((pd == null) || !pd.IsOpen) { Debug.Assert(false); return; }

            PwEntry cureentSelectedItem = m_host.MainWindow.GetSelectedEntry(true);
            RemoteAppCredentials ammyy = new RemoteAppCredentials();
            ProtectedString protectedStringRemoteID = cureentSelectedItem.Strings.Get("AmmyyID");
            ProtectedString protectedStringRemotePass = cureentSelectedItem.Strings.Get("AmmyyPass");
            ammyy.RemoteID = protectedStringRemoteID == null ? null : protectedStringRemoteID.ReadString();
            ammyy.RemotePass = protectedStringRemotePass == null ? null : protectedStringRemotePass.ReadString();
           
            if (ammyy.RemoteID != null && ammyy.RemotePass != null)
            {
                Clipboard.SetText(ammyy.RemotePass);
                StartProcess(appPaths.AmmyyFullPath, " -connect " + ammyy.RemoteID);
            }
            else
            {
                MessageBox.Show("No saved Ammyy credentials for this entry!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void StartProcess(string appFullPaths, string commandlineParams)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = appFullPaths;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = commandlineParams;
            Process.Start(startInfo);
        }

        private PlugInApplicationPaths GetPaths()
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + @"\Plugins\TeamViewerAndAmmyy.json";
            string jsonString = string.Empty;
            if (File.Exists(filePath))
            {
                jsonString = File.ReadAllText(filePath);
            }
            
            var configAppPaths = JsonConvert.DeserializeObject<PlugInApplicationPaths>(jsonString);

            if (File.Exists(configAppPaths.TeamViewerFullPath))
            {
                configAppPaths.TeamViewerExists = true;
            }
            else
            {
                configAppPaths.TeamViewerExists = false;
            }
            if (File.Exists(configAppPaths.AmmyyFullPath))
            {
                configAppPaths.AmmyyExists = true;
            }
            else
            {
                configAppPaths.AmmyyExists = false;
            }

            return configAppPaths;
        }
    }
}
