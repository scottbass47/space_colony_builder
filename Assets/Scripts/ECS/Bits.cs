using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteNetLib.Utils;

namespace ECS
{
    // @Todo Make serialization smaller
    public class Bits : IEquatable<Bits>, INetSerializable, IEnumerable, IEnumerator
    {
        private int[] words = { 0 };
        public int[] Words => words;
        private readonly int wordSize = 32;
        public int Nbits { get; private set; }

        public bool Empty
        {
            get
            {
                foreach (var word in words)
                {
                    if (word != 0) return false;
                }
                return true;
            }
        }

        private int position = -1;

        public bool Current 
        {
            get
            {
                try
                {
                    return Get(position);
                }
                catch(IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        object IEnumerator.Current => Current;

        public Bits(int nbits)
        {
            EnsureCapacity(nbits);
        }

        public Bits()
        {
        }

        public void Set(int idx, bool on)
        {
            EnsureCapacity(idx);

            int wordIdx = idx / wordSize;
            int bitIdx = idx % wordSize;
            
            if(on)
            {
                words[wordIdx] |= (1 << bitIdx);
            }
            else
            {
                words[wordIdx] &= ~(1 << bitIdx);
            }
        }

        public bool Get(int idx)
        {
            int wordIdx = idx / wordSize;
            int bitIdx = idx % wordSize;
            return (words[wordIdx] & (1 << bitIdx)) != 0;
        }

        public bool Intersects(Bits other)
        {
            for(int i = 0; i < Math.Min(Words.Length, other.Words.Length); i++)
            {
                if ((Words[i] & other.Words[i]) != 0) return true;
            }
            return false;
        }

        public bool ContainsAll(Bits other)
        {
            for(int i = Words.Length; i < other.Words.Length; i++)
            {
                if (other.Words[i] != 0) return false;
            }
            for(int i = 0; i < Math.Min(Words.Length, other.Words.Length); i++)
            {
                if ((Words[i] & other.Words[i]) != other.Words[i]) return false;
            }
            return true;
        }

        private void EnsureCapacity(int nbits)
        {
            int numWords = nbits / wordSize + 1;

            if(numWords > words.Length)
            {
                int[] newWords = new int[numWords];
                Array.Copy(words, newWords, words.Length);
                words = newWords;
            }
            Nbits = nbits;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Bits);
        }

        public bool Equals(Bits other)
        {
            if (other == null) return false;

            if (words.Length != other.words.Length) return false;

            for(int i = 0; i < words.Length; i++)
            {
                if (words[i] != other.words[i]) return false;
            }

            return true; 
        }

        public override int GetHashCode()
        {
            var hashCode = 1152762283;

            for (int i = 0; i < words.Length; ++i)
            {
                hashCode = unchecked(hashCode * 314159 + words[i]);
            }
            return hashCode;
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Nbits);
            writer.PutArray(words);
        }

        public void Deserialize(NetDataReader reader)
        {
            Nbits = reader.GetInt();
            words = reader.GetIntArray();
        }

        public IEnumerator GetEnumerator()
        {
            return this;
        }

        public bool MoveNext()
        {
            position++;
            return position < Nbits;
        }

        public void Reset()
        {
            position = -1;
        }

        public static bool operator ==(Bits bits1, Bits bits2)
        {
            return EqualityComparer<Bits>.Default.Equals(bits1, bits2);
        }

        public static bool operator !=(Bits bits1, Bits bits2)
        {
            return !(bits1 == bits2);
        }
    }
}
