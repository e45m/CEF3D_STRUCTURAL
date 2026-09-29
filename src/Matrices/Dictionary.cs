using System.Runtime.CompilerServices;
namespace CEF.Dictionary;
class cefDictionary
{
    public List<long> Keys;
    internal class bucket
    {
        public double value;
        public long key;
        public bucket next;
        public bool used;
        public bucket() { value = 0d; used = false; next = null; key = -1; }
    }
    public bucket[] buckets;
    public long sizeBuckets;
    internal static long MixHash(long k)
    {
        ulong x = (ulong)k;
        x ^= x >> 33;
        x *= 0xff51afd7ed558ccdUL;
        x ^= x >> 33;
        return (long)x;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryGetValue(in long key, out double value)
    {
        long b = getBucket(key);
        value = 0d;
        bucket node = buckets[b];
        if (node == null)
            return false;
        while (node != null)
        {
            if (node.used && node.key == key)
            {
                value = node.value;
                return true;
            }
            node = node.next;
        }
        return false;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryAdd(in long key, in double value)
    {
        long b = getBucket(key);
        if (buckets[b] == null)
            buckets[b] = new bucket();
        bucket node = buckets[b];
        bucket prev = null;
        while (true)
        {
            if (!node.used)
            {
                node.key = key;
                node.value = value;
                node.used = true;
                Keys.Add(key);
                return true;
            }
            if (node.key == key)
            {
                node.value = value;
                return true;
            }
            if (node.next == null)
            {
                node.next = new bucket();
            }
            prev = node;
            node = node.next;
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    internal static long makeKey(int a, int b)
    {
        return ((long)a << 24) | (uint)b;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    internal static (int i, int j) decodeKey(long k)
    {
        int i = (int)(k >> 24);
        int j = (int)(k & ((1L << 24) - 1));
        return (i, j);
    }
    internal static int getRowFromKey(long k)
   => (int)(k >> 24);
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    internal static int getColFromKey(long k)
        => (int)(k & ((1L << 24) - 1));
    internal long getBucket(long key) => MixHash(key) & (sizeBuckets - 1);
    internal void setBucketSize(long capacity)
    {
        capacity--;
        capacity |= capacity >> 1;
        capacity |= capacity >> 2;
        capacity |= capacity >> 4;
        capacity |= capacity >> 8;
        capacity |= capacity >> 16;
        capacity |= capacity >> 32;
        sizeBuckets = capacity + 1;
    }
    public double this[int n, int m]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get { 
            TryGetValue(makeKey(n, m), out double r); 
            return r; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            TryAdd(makeKey(n, m), value);
        }
    }
    public double this[long key]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            TryGetValue(key, out double r);
            return r;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            TryAdd(key, value);
        }
    }
}