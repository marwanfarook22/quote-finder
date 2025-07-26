
using _16_QuoteFinder.DataAccess.Mock;
using System.Diagnostics;
using System.Text.Json;
using static SingleThreading;

new Application(new UserPrompts()
    , new SingleThreading(new UserPrompts()),
    new ProcessAsync(new UserPrompts())
    )
    .StartRun();

Console.ReadKey();


public class Application
{
    IUserPrompts UserPrompts { get; set; }
    ISingleThreading SingleThreading { get; set; }
    IProcessAsync MultiThreading { get; set; }
    Stopwatch stopwatch = new Stopwatch();

    public Application(IUserPrompts userPrompts,
        ISingleThreading singleThreading,
        IProcessAsync multiThreading)
    {
        UserPrompts = userPrompts;
        SingleThreading = singleThreading;
        MultiThreading = multiThreading;
    }

    private string Word { get; set; }
    private int Pages { get; set; }
    private int limits { get; set; }
    private bool parallel { get; set; }
    public void StartRun()
    {
        UserInteraction();
        UserPrompts.ShowDefaultMessage("Fetching Date...");


        stopwatch.Start();
        if (parallel)
            MultiThreading.muiltiThreading(Pages, limits, Word);
        else
            SingleThreading.singleThreading(Pages, limits, Word);


        UserPrompts.ShowDefaultMessage("Programed Is Finshed");

        stopwatch.Stop();
        UserPrompts.ShowDefaultMessage(" The Time For Your Excution Is : " + stopwatch.ElapsedMilliseconds);
    }

    private void UserInteraction()
    {
        UserPrompts.ShowDefaultMessage("What word are you looking for?");
        Word = Console.ReadLine()?.Trim();

        UserPrompts.ShowDefaultMessage("Number of pages you want:");
        Pages = GetValidInteger();

        UserPrompts.ShowDefaultMessage("How many quotes you want per page:");
        limits = GetValidInteger();

        UserPrompts.ShowDefaultMessage("Should processing be performed in parallel? (y/n)");
        parallel = GetYesNoChoice();

    }
    private int GetValidInteger()
    {
        int result;
        while (!int.TryParse(Console.ReadLine()?.Trim(), out result) || result <= 0)
        {
            UserPrompts.FalsePrompt("Invalid input. Please enter a positive number:");
        }
        return result;
    }

    private bool GetYesNoChoice()
    {
        string? input;
        while ((input = Console.ReadLine()?.Trim().ToLower()) != "y" && input != "n")
        {
            UserPrompts.FalsePrompt("Invalid input. Please enter 'y' or 'n':");
        }
        return input == "y";
    }

}

public interface IUserPrompts
{
    void ShowDefaultMessage(string Message);
    void FalsePrompt(string Message);
    void TruePrompt(string Message);
    void DataPrompt(List<Datum> Data, string WordRequire);
}

public class UserPrompts : IUserPrompts
{
    public void FalsePrompt(string Message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(Message);
    }
    public void TruePrompt(string Message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(Message);
    }
    public void ShowDefaultMessage(string Message)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(Message);
    }
    public void DataPrompt(List<Datum> Data, string WordRequire)
    {
        ShowDefaultMessage($"{nameof(DataPrompt)} Id :" + Thread.CurrentThread.ManagedThreadId);
        var DatumColliction = Data.Where(Words => Words.quoteText.Contains(WordRequire)).ToList();
        if (DatumColliction.Count() == 0)
            FalsePrompt("no quote is found on a page");
        else
            TruePrompt(DatumColliction.OrderByDescending(word => word.quoteText.Length).Last().quoteText);
    }


}

public interface ISingleThreading
{
    Task singleThreading(int Pages, int limits, string word);
}

public class SingleThreading(IUserPrompts userPrompts) : ISingleThreading
{
    MockQuotesApiDataReader MockQuotesApiDataReader = new MockQuotesApiDataReader();

    public async Task singleThreading(int Pages, int limits, string word)
    {
        for (int i = 1; i <= Pages; i++)
        {
            {
                try
                {
                    var JsonData = await MockQuotesApiDataReader.ReadAsync(i, limits);
                    var stringsData = JsonSerializer.Deserialize<Root>(JsonData);

                    if (stringsData == null)
                    {
                        throw new NullReferenceException();
                    }
                    userPrompts.DataPrompt(stringsData.data, word);

                }
                catch (NullReferenceException ex)
                {
                    userPrompts.FalsePrompt(ex.Message);
                }
            }
        }
    }

    public interface IProcessAsync
    {
        void muiltiThreading(int Pages, int Limits, string word);
    }

    public class ProcessAsync(IUserPrompts userPrompts) : IProcessAsync
    {
        MockQuotesApiDataReader MockQuotesApiDataReader = new MockQuotesApiDataReader();

        public void muiltiThreading(int Pages, int limits, string word)
        {
            for (int currentPage = 1; currentPage <= Pages; currentPage++)
            {
                var task = Task.Run(() => MockQuotesApiDataReader.ReadAsync(currentPage, limits))
                    .ContinueWith(jsonResponse => JsonSerializer.Deserialize<Root>(jsonResponse.Result));

                task.ContinueWith(completedTask =>
                {
                    if (completedTask.Result?.data != null)
                    {
                        userPrompts.DataPrompt(completedTask.Result.data, word);
                    }
                }, TaskContinuationOptions.OnlyOnRanToCompletion);


            }

        }

    }
}