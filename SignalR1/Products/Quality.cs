using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Web;

namespace SignalR1.Products
{
    public interface IQuality 
    {
        Quality Quality { get; set; } 
    }
    
    public class QualityUpdate<TValue>
    {
        public QualityUpdate(Quality oldQuality, Quality newQuality, TValue value)
        {
            this.OldQuality = oldQuality;
            this.NewQuality = newQuality;
            this.Value = value;
        }

        Quality OldQuality { get; private set; } 
        Quality NewQuality { get; private set; } 
        TValue Value { get; private set; } 
    }

    public class QualityBasedDictionary<TKey, TValue>
        where TValue : IQuality
    {
        private readonly Dictionary<Quality, ConcurrentDictionary<TKey, TValue>> data =
            new Dictionary<Quality, ConcurrentDictionary<TKey, TValue>>();

        private readonly ConcurrentDictionary<TKey, TValue> masterList = new ConcurrentDictionary<TKey, TValue>();

        private readonly Subject<QualityUpdate<TValue>> updates = new Subject<QualityUpdate<TValue>>();

        public QualityBasedDictionary()
        {
            var values = Enum.GetValues(typeof(Quality));
            foreach (var value in values)
            {
                this.data.Add((Quality)value, new ConcurrentDictionary<TKey, TValue>());
            }
        }

        public IObservable<QualityUpdate<TValue>> Updates
        {
            get
            {
                return this.updates;
            }
        }
    

        public int Count { get { return this.masterList.Count; } }

        public IEnumerable<TValue> Values
        {
            get
            {
                return this.masterList.Select(l => l.Value);
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                return this.masterList[key];
            }
        }

        public void Add(TKey key, TValue value)
        {
            this.masterList.TryAdd(key, value);
            this.data[value.Quality].TryAdd(key, value);
        }



        public void Transition(TKey key, TValue value, Quality newQuality)
        {
            var oldQuality = value.Quality;
            if (oldQuality == newQuality)
            {
                return;
            }

            TValue ignore;
            this.data[oldQuality].TryRemove(key, out ignore);
            value.Quality = newQuality;
            this.data[newQuality].TryAdd(key, value);
            this.updates.OnNext(new QualityUpdate<TValue>(oldQuality, newQuality, value));
        }
    }

    public enum Direction { In, Out }
    public enum Quality
    {
        Good = 0,
        Warning,
        Error,
        OutOfHours,
        Pulled
    };

    public class DataUpdate
    {
        public DataUpdate()
        {
            this.Values = new Dictionary<string, string>();
        }

        public Direction Direction { get; set; }
        public int Id { get; set; }
        public Dictionary<string, string> Values { get; private set; }
    }

    public class DataUpdates : List<DataUpdate>
    {

        public DataUpdates(IEnumerable<DataUpdate> updates)
            : base(updates)
        {
        }


    }

}
