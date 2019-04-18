using System;

namespace BattleBot {
  static class Program {
    static void Main(string[] args) {
      Console.WriteLine("Battle Bot");

      // set up launch parameters
      // any additional parameters should be added in a similar fashion,
      // however it's probably best to do this via the config file
      // as to remove clutter on the command line

      // initialize the configuration from the path we supplied

      new BattleBot().Start().GetAwaiter().GetResult();
    }
  }
}