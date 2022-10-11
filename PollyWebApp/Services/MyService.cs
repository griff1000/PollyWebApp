using PollyWebApp.Models;

namespace PollyWebApp.Services
{
    /// <summary>
    /// This could be anything that returns some content and maybe throws an exception.  E.g. Service bus message receiver, Cosmos repository etc.
    /// </summary>
    public class MyService : IMyService
    {
        private static int _counter;
        private static int _previous = -1;

        private static string[] names = { "Fred", "Barney", "Wilma", "Betty", "Pebbles", "BamBam", "Dino", "Mr Slate" };

        public async Task<SomeDtoModel> GetSome(string content)
        {
            _counter++;
            int index;
            do
            {
                index = Random.Shared.Next(0, 8);

            } while (index == _previous);
            _previous = index;
            var returnContent = $"{content} {names[index]}";
            await Task.Delay(100); // doing 'something'

            // Enable either of the next two lines to incur a 'transient' failure
            if (_counter is > 3 and < 6) throw new ApplicationException("Oops");
            if (_counter > 12 && _counter < 16) return new SomeDtoModel { Content = returnContent, Status = 11 };
            return new SomeDtoModel{Content = returnContent, Status = index + 1};
        }
    }
}
