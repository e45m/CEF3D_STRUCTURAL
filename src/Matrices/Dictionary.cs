using System.Runtime.CompilerServices;
namespace CEF.Dictionary;
class cefDictionary
{
    public List<long> Keys;
    internal class CEFBucket
    {
        public double value;
        public long key;
        public CEFBucket next;
        public bool used;
        public CEFBucket() { value = 0d; used = false; next = null; key = -1; }
    }
    public CEFBucket[] buckets;
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
        CEFBucket Node = buckets[b];
        if (Node == null)
            return false;
        while (Node != null)
        {
            if (Node.used && Node.key == key)
            {
                value = Node.value;
                return true;
            }
            Node = Node.next;
        }
        return false;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryAdd(in long key, in double value)
    {
        long b = getBucket(key);
        if (buckets[b] == null)
            buckets[b] = new CEFBucket();
        CEFBucket Node = buckets[b];
        CEFBucket prev = null;
        while (true)
        {
            if (!Node.used)
            {
                Node.key = key;
                Node.value = value;
                Node.used = true;
                Keys.Add(key);
                return true;
            }
            if (Node.key == key)
            {
                Node.value = value;
                return true;
            }
            if (Node.next == null)
            {
                Node.next = new CEFBucket();
            }
            prev = Node;
            Node = Node.next;
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