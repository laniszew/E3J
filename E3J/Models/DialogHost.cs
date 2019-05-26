using System.Threading;

namespace E3J.Models
{
    public class DialogHost
    {

        private readonly CancellationTokenSource cancellationTokenSource;
  
        public DialogHost()
        {
            cancellationTokenSource = new CancellationTokenSource();
        }

     
        public void Cancel()
        {
            cancellationTokenSource.Cancel();
        }

        public string CurrentAction { get; set; }
     
        public string CurrentProgress { get; set; }
      
        public string Message { get; set; }
      
        public CancellationToken CancellationToken => cancellationTokenSource.Token;

    }
}
