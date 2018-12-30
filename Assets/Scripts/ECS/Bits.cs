using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public class Bits
    {
        private int[] words = { 0 };
        public int[] Words => words;
        private readonly int wordSize = 32;
        public int Nbits { get; private set; }

        public bool Empty
        {
            get
            {
                foreach(var word in words)
                {
                    if (word != 0) return false;
                }
                return true;
            }
        }

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
    }
}
