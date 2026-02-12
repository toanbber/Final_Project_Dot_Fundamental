using System;
using System.Collections.Generic;
using System.Text;

namespace Final_Project_Dot_Fundamental
{
    internal interface IDataProvider<T>
    {
        Task<IEnumerable<T>> ReadAsync(string path, CancellationToken cancellationToken);
    }
}
