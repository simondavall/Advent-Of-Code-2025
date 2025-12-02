using System.Text;

namespace AocHelper;

public static class Helper
{
  public static List<T> List<T>(List<T> list)
  {
    return list;
  }

  public static IEnumerable<int> Range(int n)
  {
    return Enumerable.Range(0, n);
  }

  public static IEnumerable<int> Range(int start, int n)
  {
    return Enumerable.Range(start, n);
  }

  public static int ToInt(this string str)
  {
    if (int.TryParse(str, out var value))
      return value;

    throw new InvalidCastException($"Not a valid integer: {str}");
  }

  public static long ToLong(this string str)
  {
    if (long.TryParse(str, out var value))
      return value;

    throw new InvalidCastException($"Not a valid integer: {str}");
  }

  public static int[] ToIntArray(this string[] array)
  {
    var intArray = new int[array.Length];
    for (var i = 0; i < array.Length; i++)
    {
      intArray[i] = array[i].ToInt();
    }

    return intArray;
  }

  public static int[] ToIntArray(this Span<string> array)
  {
    var intArray = new int[array.Length];
    for (var i = 0; i < array.Length; i++)
    {
      intArray[i] = array[i].ToInt();
    }

    return intArray;
  }

  public static long[] ToLongArray(this string[] array)
  {
    var longArray = new long[array.Length];
    for (var i = 0; i < array.Length; i++)
    {
      longArray[i] = array[i].ToLong();
    }

    return longArray;
  }

  public static char[][] ToCharArray(this string[] array)
  {
    var charArr = new char[array.Length][];
    for (var i = 0; i < array.Length; i++)
      charArr[i] = array[i].ToCharArray();

    return charArr;
  }

  public static int[][] To2DIntArray(this string[] array)
  {
    var intArr = new int[array.Length][];
    for (var i = 0; i < array.Length; i++)
    {
      intArr[i] = new int[array[i].Length];
      for (var j = 0; j < array[i].Length; j++)
      {
        intArr[i][j] = array[i][j] - '0';
      }
    }

    return intArr;
  }

  public static T[] CreateArray<T>(int size, T defaultValue)
  {
    T[] array = new T[size];
    for (var i = 0; i < size; i++) array[i] = defaultValue;
    return array;
  }

  public static (T first, T second) ToTuplePair<T>(this T[] array)
  {
    return array.Length switch
    {
      > 2 => throw new ArgumentException(
          $" Too many array members.{array.Length} This method requires an array of length 2."),
      < 2 => throw new ArgumentException(
          $" Too few array members.{array.Length} This method requires an array of length 2."),
      _ => (array[0], array[1])
    };
  }

  public static (int first, int second) ToIntTuplePair(this string[] array)
  {
    if (array.Length > 2)
      throw new ArgumentException(
          $" Too many array members.{array.Length} This method requires an array of length 2.");
    if (array.Length < 2)
      throw new ArgumentException(
          $" Too few array members.{array.Length} This method requires an array of length 2.");

    return (array[0].ToInt(), array[1].ToInt());
  }

  public static (int first, int second, int third) ToIntTupleTriple(this string[] array)
  {
    return array.Length switch
    {
      > 3 => throw new ArgumentException(
          $" Too many array members.{array.Length} This method requires an array of length 3."),
      < 3 => throw new ArgumentException(
          $" Too few array members.{array.Length} This method requires an array of length 3."),
      _ => (array[0].ToInt(), array[1].ToInt(), array[2].ToInt())
    };
  }

  public static (long first, long second, long third) ToLongTupleTriple(this string[] array)
  {
    return array.Length switch
    {
      > 3 => throw new ArgumentException(
          $" Too many array members.{array.Length} This method requires an array of length 3."),
      < 3 => throw new ArgumentException(
          $" Too few array members.{array.Length} This method requires an array of length 3."),
      _ => (array[0].ToLong(), array[1].ToLong(), array[2].ToLong())
    };
  }

  public static List<T> ToSortedList<T>(this HashSet<T> set)
  {
    var list = set.ToList();
    list.Sort();
    return list;
  }

  public static List<T> Sorted<T>(this List<T> list)
  {
    list.Sort();
    return list;
  }

  public static List<T> SortedDesc<T>(this List<T> list)
  {
    return list.OrderDescending().ToList();
  }

  public static string Print<T>(this T[] arr, int max = 10)
  {
    if (arr.Length > max)
    {
      return $"[{string.Join(", ", arr[..max])} ... ]";
    }
    else
    {
      return $"[{string.Join(", ", arr)}]";
    }
  }

  public static string Print<TKey, TValue>(this Dictionary<TKey, TValue> dict, int max = 10) where TKey: notnull
  {
    var str = new StringBuilder();
    foreach(var (k, v) in dict){
     str.Append($"[{k}:{v}], ");
     if (--max == 0){
       str.Append($"... max {max} shown");
       break;
     }
    }
    return str.ToString();
  }
}
