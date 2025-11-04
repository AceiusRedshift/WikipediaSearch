using static System.Environment;
using static System.IO.Path;
namespace Common;

public static class IndexFile
{
    public static readonly string Path = Join(GetFolderPath(SpecialFolder.MyDocuments), "WikipediaSearch.Index");
}