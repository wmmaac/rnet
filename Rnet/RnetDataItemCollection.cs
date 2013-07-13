﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Rnet
{

    /// <summary>
    /// Stores a set of <see cref="Item"/>s.
    /// </summary>
    public class RnetDataItemCollection : IEnumerable<RnetDataItem>
    {

        Dictionary<RnetPath, RnetDataItem> items = new Dictionary<RnetPath, RnetDataItem>();

        /// <summary>
        /// Gets the data item at the specified path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        RnetDataItem GetData(RnetPath path)
        {
            RnetDataItem item;
            return items.TryGetValue(path, out item) ? item : null;
        }

        /// <summary>
        /// Gets the data at the specified path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public RnetDataItem this[RnetPath path]
        {
            get
            { return GetData(path); }
        }

        /// <summary>
        /// Begins a write of new data to the specified path.
        /// </summary>
        /// <param name="path"></param>
        public void WriteBegin(RnetPath path)
        {
            var item = GetData(path);
            if (item == null)
                item = items[path] = new RnetDataItem(path);

            item.WriteBegin();
        }

        /// <summary>
        /// Appends the data to the specified path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        public void Write(RnetPath path, byte[] buffer)
        {
            var item = GetData(path);
            if (item == null)
                throw new NullReferenceException();

            item.Write(buffer);
        }

        /// <summary>
        /// Finalizes writing to the path and makes the data available.
        /// </summary>
        /// <param name="path"></param>
        public void WriteEnd(RnetPath path)
        {
            var item = GetData(path);
            if (item == null)
                throw new NullReferenceException();

            item.WriteEnd();
        }

        /// <summary>
        /// Removes the data item at the given path.
        /// </summary>
        /// <param name="path"></param>
        public void Remove(RnetPath path)
        {
            if (items.ContainsKey(path))
                items.Remove(path);
        }

        public IEnumerator<RnetDataItem> GetEnumerator()
        {
            return items.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }

}