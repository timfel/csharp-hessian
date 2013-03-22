using System;

namespace hessiancsharp.io
{
  [AttributeUsage(AttributeTargets.Class)]
  public class CTypeNameAttribute : Attribute
  {
    public CTypeNameAttribute(string name)
    {
      if (string.IsNullOrEmpty(name))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "name");
      }


      Name = name;
    }

    public string Name { get; private set; }
  }
}
