// ---
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;


namespace CloudyLauncher
{
  internal class DownloadQueue
  {
    //public vars
    private readonly WebClient _client;
    private readonly Queue<Tuple<string, string>> _download_queue;
    public string CurrentDownload;

    public DownloadQueue(WebClient client)
    {
      _client = client;
      _download_queue = new Queue<Tuple<string, string>>();
      CurrentDownload = "DEBUG_NAME";
    }

    public List<string> GetDirectoryList(string directory_url, string path)
    {
      var files = new List<string>();
      var request = (HttpWebRequest) WebRequest.Create(directory_url + path);
      using(var response = (HttpWebResponse) request.GetResponse())
      {
        // ReSharper disable once AssignNullToNotNullAttribute
        using(var reader = new StreamReader(response.GetResponseStream()))
        {
          var html = reader.ReadToEnd();
          var regex = new Regex("<a href=\".*\">(?<name>.*)</a>");
          var matches = regex.Matches(html);

          if (matches.Count > 0)
            files.AddRange(from Match match in matches where match.Success select match.Groups["name"].ToString().Substring(1));
        }
      }
      var ret = new List<string>();

      foreach (var file in files.Where(file => !file.Equals("Parent Directory")))
      {
        if (file.Contains("/"))
        {
          // it's a directory, recurse
          ret.AddRange(GetDirectoryList(directory_url, path + file));
        }
        else
          ret.Add(path + file);
      }

      return ret;
    }

    public bool Download()
    {
      if (!_download_queue.Any()) return false;

      var current_tuple = _download_queue.Dequeue();
      _client.DownloadFileAsync(new Uri(current_tuple.Item1), current_tuple.Item2);
      CurrentDownload = current_tuple.Item2;
      return true;
    }

    public void AddDownload(string url, string path)
    {
      _download_queue.Enqueue(new Tuple<string, string>(url, path));
    }
  }
}