using PollyWebApp.Models;

namespace PollyWebApp.Services
{
    /// <summary>
    /// This could be anything that returns some content and maybe throws an exception.  E.g. Service bus message receiver, Cosmos repository etc.
    /// </summary>
    public class MyService : IMyService
    {
        private static int _counter = 0;

        private static string[] names = new[]
            { "Fred", "Barney", "Wilma", "Betty", "Pebbles", "BamBam", "Dino", "Mr Slate" };

        public async Task<SomeDtoModel> GetSome(string content)
        {
            _counter++;
            var index = Random.Shared.Next(0, 8);
            var returnContent = $"{content} {names[index]}";
            await Task.Delay(100); // doing 'something'

            // Enable either of the next two lines to incur a 'transient' failure
            if (_counter >3 && _counter < 6) throw new ApplicationException("Oops");
            //if (_counter > 3 && _counter < 6) return new SomeDtoModel { Content = returnContent, Status = 11 };
            return new SomeDtoModel{Content = returnContent, Status = index};
        }
    }
}
