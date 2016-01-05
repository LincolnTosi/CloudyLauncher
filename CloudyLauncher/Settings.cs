/*-----------------------------------------------------------------------------*
  Project: GAM450 CloudyLauncher
  Author:  Lincoln Tosi (lincoln.t@digipen.edu)
 *----------------------------------------------------------------------------*/

using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;

namespace CloudyLauncher
{
  class Settings
  {
    private const string kUrlBase = "http://mc.beachey.net:82/GAM4XX";
    private const string kNameLog = "UpdateLog";
    private const string kNameLauncher = "CloudyLauncher";
    private const string kNameExecutable = "Prock";
    private const string kNameLauncherHash = "LauncherHash";
    private const string kNameReleaseManifest = "ReleaseManifest";
    private const string kNameReleaseManifestHash = "ReleaseManifestHash";

    private const string kUrlDirLauncher = kUrlBase + "/Launcher";
    private const string kUrlDirRelease = kUrlBase + "/Release";
    private const string kUrlDirReleaseWindows = kUrlDirRelease + "/ProckWindows";

    public Settings()
    {
    }

    public string GetFileHash(string file_path)
    {
      using(var file_stream = File.OpenRead(file_path))
      {
        using(var md5 = MD5.Create())
        {
          return BitConverter.ToString(md5.ComputeHash(file_stream)).Replace("-", String.Empty);
        }
      }
    }

    // --- Inconsistent Names

    public string GetNameLauncher()
    {
      return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);
    }

    // --- Online Directories

    public string GetUrlDirReleaseWindows()
    {
      return kUrlDirReleaseWindows;
    }

    // --- Online File Paths

    public string GetUrlPathLauncher()
    {
      return String.Format(@"{0}/{1}.exe", kUrlDirLauncher, kNameLauncher);
    }

    public string GetUrlPathLauncherHash()
    {
      return String.Format(@"{0}/{1}.txt", kUrlDirLauncher, kNameLauncherHash);
    }

    public string GetUrlPathManifest()
    {
      return String.Format(@"{0}/{1}.txt", kUrlDirRelease, kNameReleaseManifest);
    }

    public string GetUrlPathManifestHash()
    {
      return String.Format(@"{0}/{1}.txt", kUrlDirRelease, kNameReleaseManifestHash);
    }

    // --- Local Directories

    public string GetDirLocal()
    {
      return Directory.GetCurrentDirectory();
    }

    public string GetDirTemp()
    {
      return Environment.GetEnvironmentVariable("TEMP");
    }

    public string GetDirAppData()
    {
      return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    }

    public string GetDirMyDocuments()
    {
      return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    }

    public string GetDirReleaseWindows()
    {
      return String.Format(@"{0}\{1}", GetDirLocal(), "ProckWindows");
    }

    public string GetDirReleaseExecutable()
    {
      return String.Format(@"{0}\{1}", GetDirReleaseWindows(), @"Prock\Binaries\Win32");
    }

    // --- Local File Paths

    public string GetPathManifest()
    {
      return String.Format(@"{0}\{1}.txt", GetDirReleaseWindows(), kNameReleaseManifest);
    }

    public string GetPathLog()
    {
      Directory.CreateDirectory(String.Format(@"{0}\{1}", GetDirAppData(), kNameExecutable));
      return String.Format(@"{0}\{1}\{2}.txt", GetDirAppData(), kNameExecutable, kNameLog);
    }

    public string GetPathReleaseExecutable()
    {
      return String.Format(@"{0}\{1}.exe", GetDirReleaseExecutable(), kNameExecutable);
    }
  }
}