namespace Santana
{
  public static class Namecheck
  {
    public static bool IsNameValid(string name)
    {
      if (name.StartsWith("["))
        return false;

      if (name.Contains("GM") || name.Contains("GS") || name.Contains("CM") ||
          name.Contains("PM") || name.Contains("CC"))
        return false;

      if (name.ToLower().Contains("admin"))
        return false;

      foreach (var letter in name)
      {
        var permitido = char.IsLetterOrDigit(letter) ||
                        letter == '_' || letter == '.' || letter == '-' ||
                        letter == '*' || letter == '+' || letter == '~' ||
                        letter == '/' || letter == '#';
        if (!permitido)
          return false;
      }

      return true;
    }
  }
}
