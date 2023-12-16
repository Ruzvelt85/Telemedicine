namespace Telemedicine.Common.Infrastructure.Patterns.Command
{
    /// <summary>
    /// Interface to use with Commands. Each command should implement this interface
    /// </summary>
    public interface ICommand
    {
    }

    /// <summary>
    /// Interface to use with Commands with result
    /// </summary>
    /// <typeparam name="TResult">The type of result</typeparam>
    // ReSharper disable once UnusedTypeParameter // We retrieve command handler by TResult
    public interface ICommand<TResult> : ICommand
    {
    }
}
