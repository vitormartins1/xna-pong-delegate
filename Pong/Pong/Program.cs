using System;

namespace Pong
{
#if WINDOWS || XBOX
    static class Program
    {
    
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Game1 game = new Game1())
            {
                Game1.StartGame();
                //Game1.TestMenuSprites();
                //Game1.TestSinglePlayerGame();
                //Game1.TestBallCollisions();
            } // Main(args)
        }
    }
#endif

}

