using System.Threading.Tasks;
using System.Windows.Forms;

namespace Meadow
{
    public class GtkSample
    {
        public static async Task Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ApplicationConfiguration.Initialize();

            await MeadowOS.Start();
        }

    }
}
