using System.Collections;
using System.Reflection;
using Xunit;

namespace HappyKids.Test.Helper
{
    public static class AssertObjects
    {
        public static void PropertyValuesAreEquals(object actual, object expected)
        {
            PropertyInfo[] properties = expected.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                object expectedValue = property.GetValue(expected, null);
                object actualValue = property.GetValue(actual, null);

                if (actualValue is IList)
                    AssertListsAreEquals(property, (IList)actualValue, (IList)expectedValue);
                else if (!Equals(expectedValue, actualValue))
                    if (property.DeclaringType != null)
                        Assert.True(false,$"Property {property.DeclaringType.Name}.{property.Name} does not match. Expected: {expectedValue} but was: {actualValue}");
            }
        }

        public static void ListAreEquals(IList actual, IList expected)
        {
            if (actual.Count != expected.Count)
                Assert.True(false, $"List Count Not Equals");

            PropertyInfo[] properties = expected[0].GetType().GetProperties();

            for (int i = 0; i < expected.Count -1; i++)
            {
                foreach (PropertyInfo property in properties)
                {
                    object expectedValue = property.GetValue(expected[i], null);
                    object actualValue = property.GetValue(actual[i], null);

             
                    if (!Equals(expectedValue, actualValue))
                        if (property.DeclaringType != null)
                            Assert.True(false, $"Property {property.DeclaringType.Name}.{property.Name} does not match. Expected: {expectedValue} but was: {actualValue} at position:{i}");
                }
            }
        
        }

        private static void AssertListsAreEquals(PropertyInfo property, IList actualList, IList expectedList)
        {
            if (actualList.Count != expectedList.Count)
                Assert.True(false,$"Property {property.PropertyType.Name}.{property.Name} does not match. Expected IList containing {expectedList.Count} elements but was IList containing {actualList.Count} elements");

            for (int i = 0; i < actualList.Count; i++)
                if (!Equals(actualList[i], expectedList[i]))
                    Assert.True(false,$"Property {property.PropertyType.Name}.{property.Name} does not match. Expected IList with element {expectedList[i]} equals to {actualList[i]} but was IList with element {1} equals to {3}");
        }
    }
}
