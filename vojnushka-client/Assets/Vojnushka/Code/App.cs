using Vojnushka.Infrastructure;

namespace Vojnushka
{
    public class App
    {
        private static AppContext _context;

        public static AppContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = new AppContext();
                    _context.RegisterDependencies();
                    _context.Run();
                }
                return _context;
            }
        }

        public static bool HasContext => _context != null;

        private App()
        {
        }
    }
}
