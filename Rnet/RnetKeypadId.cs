﻿using System;

namespace Rnet
{

    /// <summary>
    /// Specifies an RNET KeypadID component of a <see cref="RnetDeviceId"/>.
    /// </summary>
    public struct RnetKeypadId : IComparable<RnetKeypadId>, IComparable
    {

        public static readonly RnetKeypadId Null = 0x00;
        public static readonly RnetKeypadId Controller = 0x7f;
        public static readonly RnetKeypadId Reserved = 0x7e;
        public static readonly RnetKeypadId AllZone = 0x7d;
        public static readonly RnetKeypadId RequestId = 0x7c;
        public static readonly RnetKeypadId External = 0x70;
        public static readonly RnetKeypadId External2 = 0x71;
        public static readonly RnetKeypadId SourceBroadcastDisplayFeedback = 0x79;

        /// <summary>
        /// Returns <c>true</c> if the keypad id is reserved.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsReserved(RnetKeypadId id)
        {
            if (id == Controller ||
                id == Reserved ||
                id == AllZone ||
                id == RequestId ||
                id == External ||
                id == External2 ||
                id == SourceBroadcastDisplayFeedback)
                return true;

            return false;
        }

        /// <summary>
        /// Implicitly converts a <see cref="RnetKeypadId"/> to a <see cref="Byte"/>.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static implicit operator byte(RnetKeypadId id)
        {
            return id.Value;
        }

        /// <summary>
        /// Implicitly converts a <see cref="Byte"/> to a <see cref="RnetKeypadId"/>.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static implicit operator RnetKeypadId(byte value)
        {
            return new RnetKeypadId(value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="Int32"/> to a <see cref="RnetKeypadId"/>.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static implicit operator RnetKeypadId(int value)
        {
            return new RnetKeypadId((byte)value);
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="value"></param>
        public RnetKeypadId(byte value)
            : this()
        {
            Value = value;
        }

        /// <summary>
        /// Gets or sets the underlying value of the keypad ID.
        /// </summary>
        public byte Value { get; set; }

        /// <summary>
        /// Indicates whether this instance and the specified object are equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is RnetKeypadId ? ((RnetKeypadId)obj).Value == Value : false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            if (this == Controller)
                return string.Format("{0} /* Controller */", Value);
            if (this == Reserved)
                return string.Format("{0} /* Reserved */", Value);
            if (this == AllZone)
                return string.Format("{0} /* AllZone */", Value);
            if (this == RequestId)
                return string.Format("{0} /* RequestId */", Value);
            if (this == External)
                return string.Format("{0} /* External */", Value);
            if (this == External2)
                return string.Format("{0} /* External2 */", Value);
            if (this == SourceBroadcastDisplayFeedback)
                return string.Format("{0} /* SourceBroadcastDisplayFeedback */", Value);

            return Value.ToString();
        }

        /// <summary>
        /// Compares this <see cref="RnetKeypadId"/> to another <see cref="RnetKeypadId"/>.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        int IComparable<RnetKeypadId>.CompareTo(RnetKeypadId other)
        {
            return Value.CompareTo(other.Value);
        }

        /// <summary>
        /// Compares the current object with another object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        int IComparable.CompareTo(object obj)
        {
            return ((IComparable<RnetKeypadId>)this).CompareTo((RnetKeypadId)obj);
        }

    }

}
