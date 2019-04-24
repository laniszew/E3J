using System.Threading.Tasks;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;

namespace E3J.ViewModels
{
    public class SampleViewModel : ViewModelBase
    {
        public int Counter { get; set; }

        [Command]
        public void StartCounter()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    Counter++;
                    await Task.Delay(100);
                }
            });
        }
    }
}
