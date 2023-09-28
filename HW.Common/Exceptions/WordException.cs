namespace HW.Common.Exceptions; 

public class WordException : Exception {
    /// <summary>
    /// Constructor
    /// </summary>
    public WordException() {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public WordException(string message)
        : base(message) {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public WordException(string message, Exception inner)
        : base(message, inner) {
    }
}