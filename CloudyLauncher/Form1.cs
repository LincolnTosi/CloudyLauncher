/*-----------------------------------------------------------------------------*
  Project: GAM450 CloudyLauncher
  Author:  Lincoln Tosi (lincoln.t@digipen.edu)
 *----------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Security.Principal;

namespace CloudyLauncher
{
  public partial class Form1 : Form
  {
    #region Allows borderless window to be dragged with mouse cursor.

    private const int kWmNclbuttondown = 0xA1;
    private const int kHtCaption = 0x2;

    [DllImport("user32.dll")]
    public static extern int SendMessage(IntPtr h_wnd, int msg, int w_param, int l_param);

    [DllImport("user32.dll")]
    public static extern bool ReleaseCapture();

    private void Drag_Override(object sender, MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Left) return;

      ReleaseCapture();
      SendMessage(Handle, kWmNclbuttondown, kHtCaption, 0);
    }

    #endregion

    #region Helper functions for admin privileges and elevation status.

    private static bool CurrentProcessIsAdmin()
    {
      var id = WindowsIdentity.GetCurrent();
      var principal = new WindowsPrincipal(id);
      return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    #endregion

    //public vars
    //private vars
    private readonly WebClient client_;
    private readonly DownloadQueue download_queue_;
    private readonly Settings settings_;
    private Action download_complete_action_;
    private int download_curr_;
    private int download_max_;


    public Form1()
    {
      InitializeComponent();
      MouseDown += Drag_Override;
      customImageButton1.Click += customImageButton1_Click;
      customImageButton2.Click += customImageButton2_Click;
      customImageButton1.Hide();
      client_ = new WebClient();
      client_.DownloadProgressChanged += client_DownloadProgressChanged;
      client_.DownloadFileCompleted += client_DownloadFileCompleted;
      settings_ = new Settings();
      download_queue_ = new DownloadQueue(client_);
      download_curr_ = 1;
      download_max_ = 0;
      // transparency for background image
      BackColor = Color.Red;
      TransparencyKey = Color.Red;
      // background worker for async process
      backgroundWorker1.WorkerReportsProgress = true;
      backgroundWorker1.WorkerSupportsCancellation = true;
      backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
      backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
    }
    private void Form1_Load(object sender, EventArgs e)
    {
      BringToFront();
#if !DEBUG

      if (!ElevateSelf())
      {
        FinalDisplay();
        return;
      }

#endif

      if (File.Exists(settings_.GetPathLog()))
        File.Delete(settings_.GetPathLog());

      Console.WriteLine(settings_.GetPathLog());

      try
      {
        if (!CheckLauncher())
          UpdateLauncher();
        else if (!backgroundWorker1.IsBusy)
          backgroundWorker1.RunWorkerAsync();
      }
      catch (WebException w_ex)
      {
        MessageBox.Show(w_ex.Message, "WError");
        SafeClose();
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, "Error");
        SafeClose();
      }
    }

    private bool ElevateSelf()
    {
      // elevate the process if it is not currently elevated
      if (CurrentProcessIsAdmin()) return true;

      var proc = new ProcessStartInfo
      {
        UseShellExecute = true,
        WorkingDirectory = Environment.CurrentDirectory,
        FileName = Application.ExecutablePath,
        Verb = "runas"
      };

      try
      {
        Process.Start(proc);
      }
      catch
      {
        // elevation was refused by the user
        MessageBox.Show("The launcher requires administrator privileges to install patches.", "WARNING");
        return false;
      }

      Application.Exit();
      return true;
    }


    private void customImageButton1_Click(object sender, EventArgs e)
    {
      RunRelease();
    }

    private void SafeClose()
    {
      backgroundWorker1.CancelAsync();
      Close();
    }

    private void customImageButton2_Click(object sender, EventArgs e)
    {
      // button1 is only visible if updates are complete
      if (customImageButton1.Visible)
        SafeClose();

      var result = MessageBox.Show("Update in progress. Do you really want to quit?",
                                   "Confirmation",
                                   MessageBoxButtons.YesNo);

      if (result == DialogResult.Yes)
        SafeClose();
    }


    private bool CheckLauncher()
    {
#if DEBUG
      return true;
#endif
      // checks if the launcher hash matches that stored on the server
      // return: true if matches
      string launcher_hash_local = settings_.GetFileHash(Assembly.GetExecutingAssembly().Location);
      string launcher_hash_server = client_.DownloadString(settings_.GetUrlPathLauncherHash()).TrimEnd('\r', '\n');
      Log("launcher path: " + Assembly.GetExecutingAssembly().Location);
      Log("launcher hash local: " + launcher_hash_local);
      Log("launcher hash server: " + launcher_hash_server);

      //CHANGE
      if (launcher_hash_local == launcher_hash_server)
      {
        Log("launcher hash matches!");

        if (File.Exists(String.Format(@"{0}\update.bat", settings_.GetDirLocal())))
          File.Delete(String.Format(@"{0}\update.bat", settings_.GetDirLocal()));

        return true;
      }

      Log("launcher hash doesn't match!");
      return false;
    }

    private void UpdateLauncher()
    {
      Log("New version of Launcher available!");
      MessageBox.Show("New version of Launcher available!");
      WindowState = FormWindowState.Minimized;
      string executable_name = settings_.GetNameLauncher();
      Log(executable_name);
      // download new version of the launcher to TEMP dir
      download_complete_action_ = LauncherDownloadComplete;
      download_curr_ = 1;
      download_max_ = 1;
      download_queue_.AddDownload(settings_.GetUrlPathLauncher(),
                                  String.Format(@"{0}\{1}.exe", settings_.GetDirTemp(), executable_name));
      download_queue_.Download();
    }

    private void LauncherDownloadComplete()
    {
      Log("Launcher download complete!");
      Log("Attempting to delete self...");
      // create a .bat file that will replace the old CloudyLauncher.exe
      FileStream batch_file = File.Create(String.Format(@"{0}\update.bat", settings_.GetDirLocal()));
      TextWriter batch_file_writer = new StreamWriter(batch_file); //CHANGE
      batch_file_writer.WriteLine(@"timeout 3 & xcopy /s /y ""{0}\{2}.exe"" ""{1}\{2}.exe"" && del ""{0}\{2}.exe""",
                                  settings_.GetDirTemp(), settings_.GetDirLocal(), settings_.GetNameLauncher());
      batch_file_writer.WriteLine(@"start {0}.exe", settings_.GetNameLauncher());
      batch_file_writer.Close();
      var batch_process = new ProcessStartInfo
      {
        FileName = String.Format(@"{0}\update.bat", settings_.GetDirLocal()),
        UseShellExecute = true,
        RedirectStandardOutput = false,
        WindowStyle = ProcessWindowStyle.Hidden
      };
      Process.Start(batch_process);
      Environment.Exit(0);
    }

    private bool CheckManifest()
    {
      // checks if the release manifest hash matches that stored on the server
      // return: true if matches
      string manifest_path = settings_.GetPathManifest();

      if (!File.Exists(manifest_path))
      {
        Log("no local version of manifest");
        return false;
      }

      string manifest_hash_local = settings_.GetFileHash(manifest_path);
      string manifest_hash_server = client_.DownloadString(settings_.GetUrlPathManifestHash()).TrimEnd('\r', '\n');
      Log("manifest path: " + manifest_path);
      Log("manifest hash local: " + manifest_hash_local);
      Log("manifest hash server: " + manifest_hash_server);

      if (manifest_hash_local == manifest_hash_server)
      {
        Log("manifest hash matches!");
        return true;
      }

      Log("manifest hash doesn't match!");
      return false;
    }

    private void UpdateManifest()
    {
      Directory.CreateDirectory(settings_.GetDirReleaseWindows());
      client_.DownloadFile(settings_.GetUrlPathManifest(), settings_.GetPathManifest());
      Log("Manifest download complete!");
    }


    private List<string> VerifyRelease()
    {
      // checks each local release file's hash against the local manifest
      // return: list of (short) file paths to depreciated files
      var ret = new List<string>();
      var manifest_paths = new List<string>();
      var manifest_paths_full = new List<string>();
      var manifest_hashes = new List<string>();
      using(var r = new StreamReader(settings_.GetPathManifest()))
      {
        while (r.Peek() >= 0)
        {
          var read_line = r.ReadLine();

          if (read_line == null) continue;

          var man = read_line.Split(':');
          manifest_paths.Add(man[0]);
          manifest_paths_full.Add(String.Format(@"{0}\{1}", settings_.GetDirReleaseWindows(), man[0]));
          manifest_hashes.Add(man[1]);
          Log(manifest_paths.Last());
          Log(manifest_hashes.Last());
        }
      }
      Log("Done reading manifest!");

      for (var i = 0; i < manifest_paths.Count; ++i)
      {
        if (File.Exists(manifest_paths_full[i]))
        {
          var file_hash_local = settings_.GetFileHash(manifest_paths_full[i]);

          if (file_hash_local == manifest_hashes[i])
          {
            Log("Good: " + manifest_paths[i]);
            continue;
          }
        }

        Log("Bad: " + manifest_paths[i]);
        ret.Add(manifest_paths[i]);
      }

      Log("Done verifying manifest files!");
      var existing_paths = RecursiveFileSearch(settings_.GetDirReleaseWindows());

      foreach (var existing_path in existing_paths)
      {
        if (manifest_paths_full.Contains(existing_path)) continue;

        try
        {
          File.Delete(existing_path);
          Log("Removed: " + existing_path);
        }
        catch (Exception ex)
        {
          Log("Error: " + ex);
        }
      }

      Log("Done removing excess files!");
      Log("Done verifying files!");
      return ret;
    }

    private List<string> RecursiveFileSearch(string dir)
    {
      var ret = new List<string>();

      try
      {
        ret.AddRange(Directory.GetFiles(dir));

        foreach (var directory in Directory.GetDirectories(dir))
          ret.AddRange(RecursiveFileSearch(directory));
      }
      catch (Exception ex)
      {
        Log("Error: " + ex);
      }

      return ret;
    }

    private void UpdateRelease(IEnumerable<string> paths)
    {
      // downloads and overwrites the local version of each file in 'paths' from the server
      Log("Queuing up release downloads...");
      download_complete_action_ = ReleaseDownloadComplete;
      download_curr_ = 1;
      download_max_ = 0;

      foreach (var path in paths)
      {
        var full_path = String.Format(@"{0}\{1}", settings_.GetDirReleaseWindows(), path);
        var truncated_path = full_path.Remove(full_path.LastIndexOf(@"\", StringComparison.Ordinal));
        Directory.CreateDirectory(truncated_path);
        ++download_max_;
        download_queue_.AddDownload(String.Format(@"{0}/{1}", settings_.GetUrlDirReleaseWindows(), path),
                                    String.Format(@"{0}\{1}", settings_.GetDirReleaseWindows(), path));
      }

      download_queue_.Download();
      DisplayDownloadText();
    }

    private void ReleaseDownloadComplete()
    {
      Log("Release downloads complete!");
      FinalDisplay();
    }


    private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
      customProgressBar1.Value = (e.BytesReceived / (float) e.TotalBytesToReceive) * 100.0f;
    }

    private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
    {
      ++download_curr_;
      Log("Downloading... " + download_queue_.CurrentDownload);
      DisplayDownloadText();

      if (e.Error == null)
        Log("Download success!");
      else
        MessageBox.Show(e.Error.ToString(), "Error Downloading");

      if (!download_queue_.Download())
        download_complete_action_();
    }


    private void DisplayDownloadText()
    {
      Invoke((Action)(() =>
      {
        label1.Text = String.Format("downloading {0}/{1}", download_curr_, download_max_);
      }));
    }

    private void RunRelease()
    {
      var release_process = new ProcessStartInfo
      {
        FileName = settings_.GetPathReleaseExecutable(),
        UseShellExecute = true
      };
      Process.Start(release_process);
      Environment.Exit(0);
    }


    private void FinalDisplay()
    {
      Invoke((Action)(() =>
      {
        customProgressBar1.Value = 100;
        label1.Hide();
        customImageButton1.Show();
      }));
    }

    public void Log(string text)
    {
      using(var w = File.AppendText(settings_.GetPathLog()))
      {
        w.WriteLine(text);
      }
    }

    private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
    {
      var worker = sender as BackgroundWorker;

      try
      {
        if (!CheckManifest())
          UpdateManifest();

        if (worker.CancellationPending) { e.Cancel = true; return; }

        var paths = VerifyRelease();

        if (worker.CancellationPending) { e.Cancel = true; return; }

        if (paths.Count == 0)
          BeginInvoke((Action)(FinalDisplay));
        else
          UpdateRelease(paths);

        if (worker.CancellationPending)
          e.Cancel = true;
      }
      catch (WebException w_ex)
      {
        MessageBox.Show(w_ex.Message, "WError");
        SafeClose();
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, "Error");
        SafeClose();
      }
    }

    void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
    }

    void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      if (e.Cancelled)
        Log("BackgroundWorker canceled!");
      else if (e.Error != null)
        Log("BackgroundWorker error: " + e.Error.Message);
      else
        Log("BackgroundWorker done!");
    }
  }
}