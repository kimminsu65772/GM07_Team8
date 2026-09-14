using System;
using System.Collections.Generic;

public class GenericObjectPool<T> where T : class
{
    private readonly Stack<T> inactiveObjects;
    private readonly HashSet<T> inactiveSet;

    private readonly Func<T> createFunc;
    private readonly Action<T> actionOnGet;
    private readonly Action<T> actionOnRelease;
    private readonly Action<T> actionOnDestroy;

    private readonly bool collectionCheck;
    private readonly int maxSize;
    private int countAll;

    public int CountInactive => inactiveObjects.Count;
    public int CountAll => countAll;

    public GenericObjectPool(
        Func<T> createFunc,
        Action<T> actionOnGet = null,
        Action<T> actionOnRelease = null,
        Action<T> actionOnDestroy = null,
        bool collectionCheck = true,
        int defaultCapacity = 10,
        int maxSize = 10000)
    {
        if (createFunc == null)
            throw new ArgumentNullException(nameof(createFunc));

        if (defaultCapacity < 0)
            throw new ArgumentOutOfRangeException(nameof(defaultCapacity));

        if (maxSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxSize));

        this.createFunc = createFunc;
        this.actionOnGet = actionOnGet;
        this.actionOnRelease = actionOnRelease;
        this.actionOnDestroy = actionOnDestroy;
        this.collectionCheck = collectionCheck;
        this.maxSize = maxSize;

        inactiveObjects =
            new Stack<T>(Math.Min(defaultCapacity, maxSize));

        inactiveSet = new HashSet<T>();
    }

    public void Prewarm(int count)
    {
        if (count <= 0 || countAll >= count)
        {
            return;
        }

        int targetCount = Math.Min(count, maxSize);
        int createCount = targetCount - countAll;

        for (int i = 0; i < createCount; i++)
        {
            T item = createFunc();

            if (item == null)
            {
                throw new InvalidOperationException(
                    "풀 생성 함수가 null을 반환했습니다.");
            }

            countAll++;
            actionOnRelease?.Invoke(item);
            inactiveObjects.Push(item);
            inactiveSet.Add(item);
        }
    }

    public T Get()
    {
        T item;

        if (inactiveObjects.Count > 0)
        {
            item = inactiveObjects.Pop();
            inactiveSet.Remove(item);
        }
        else
        {
            item = createFunc();

            if (item == null)
            {
                throw new InvalidOperationException(
                    "풀 생성 함수가 null을 반환했습니다.");
            }

            countAll++;
        }

        actionOnGet?.Invoke(item);
        return item;
    }

    public void Release(T item)
    {
        if (item == null)
            return;

        if (collectionCheck && inactiveSet.Contains(item))
        {
            throw new InvalidOperationException(
                "이미 풀에 반환된 객체를 다시 반환했습니다.");
        }

        actionOnRelease?.Invoke(item);

        if (inactiveObjects.Count >= maxSize)
        {
            countAll--;
            actionOnDestroy?.Invoke(item);
            return;
        }

        inactiveObjects.Push(item);
        inactiveSet.Add(item);
    }

    public void Clear()
    {
        while (inactiveObjects.Count > 0)
        {
            T item = inactiveObjects.Pop();
            inactiveSet.Remove(item);
            countAll--;
            actionOnDestroy?.Invoke(item);
        }
    }
}
