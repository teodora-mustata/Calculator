using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    class Memory
    {
        private List<double> memoryValues = new List<double>();

        public void AddToMemory(double value) // M+
        {
            if (memoryValues.Count > 0)
                memoryValues[memoryValues.Count - 1] += value;
            else
                memoryValues.Add(value);
        }

        public void SubtractFromMemory(double value) // M-
        {
            if (memoryValues.Count > 0)
                memoryValues[memoryValues.Count - 1] -= value;
        }

        public void StoreInMemory(double value) // MS
        {
            memoryValues.Add(value);
        }

        public double RecallMemory() // MR
        {
            if (memoryValues.Count > 0)
                return memoryValues[memoryValues.Count - 1];
            return 0;
        }

        public List<double> GetMemoryStack() // M>
        {
            return memoryValues;
        }

        public void ClearMemory() // MC
        {
            memoryValues.Clear();
        }

    }
}
