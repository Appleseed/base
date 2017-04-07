using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Appleseed.Services.Base.Model;

namespace Appleseed.Services.Base.Data.Queue
{
    /// <summary>
    /// This is the super type for Queues in Base. This is what will be powering all Queues. Including Message, Storage, etc. Queues.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBaseQueue<T>
    {
        //int Count { get; }
        //bool IsEmpty { get; }
        void Enqueue(T item);
        void Enqueue(List<T> items);
        T Dequeue();
        T Peek();
        //bool Contains(T item);

        //void Clear();
        IEnumerator<T> GetEnumerator();
    }
}
