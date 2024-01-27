using System;

namespace AmogusCompanyMod.Helpers {
    public static class RandomSelection {
        public static T[] SelectRandomElements<T>(T[] array, int numberOfElements) {
            Random rng = new Random();
            int n = array.Length;

            // Fisher-Yates shuffle algorithm
            for (int i = n - 1; i > 0; i--) {
                int j = rng.Next(i + 1);
                (array[j], array[i]) = (array[i], array[j]);
            }

            // Select the first 'numberOfElements' elements
            T[] selectedElements = new T[numberOfElements];
            Array.Copy(array, selectedElements, numberOfElements);

            return selectedElements;
        }
    }
}
