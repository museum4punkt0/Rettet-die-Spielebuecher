public static class PlayerSettings
{
  private static int robot;
  private static float[] fogAlphas;

  public static int Robot
  {
      get
      {
          return robot;
      }
      set
      {
          robot = value;
      }
  }

  public static float[] FogAlphas
  {
    get
    {
      return fogAlphas;
    }
    set
    {
      fogAlphas = value;
    }
  }
}
