using System;
using System.Linq;

namespace Models;

public static class AuxMath
{
    /// <summary>
    /// Extension method to find the maximum value in a double array.
    /// </summary>
    /// <param name="args">The array of doubles.</param>
    /// <returns>The maximum value in the array.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the array is empty.</exception>
    public static double Max(this double[] args)
    {
        if (args is null || args.Length == 0)
            throw new InvalidOperationException("The array must not be empty.");

        var max = args[0]; // Initialize with the first element
        return args.Prepend(max).Max();
    }
}