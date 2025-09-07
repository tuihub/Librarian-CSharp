namespace Librarian.Common.Utils;

public static class HumanizeUtil
{
    // https://stackoverflow.com/a/4975942
    public static string SizeBytesToString(long byteCount)
    {
        string[] suf = { "B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB" }; //Longs run out around EB
        if (byteCount == 0)
            return "0" + suf[0];
        var bytes = Math.Abs(byteCount);
        var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
        var num = Math.Round(bytes / Math.Pow(1024, place), 2);
        return (Math.Sign(byteCount) * num) + " " + suf[place];
    }
}