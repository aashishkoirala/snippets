### The `INextProvider` LINQ Provider
##### by [Aashish Koirala](http://aashishkoirala.github.io)
---

This is a simple, mostly pointless LINQ provider that adds `IQueryable<T>` based LINQ support to something that doesn't really need it. The point is to try to understand and show how building a LINQ provider of your own actually works. An interface, `INextProvider` is provided that one can implement. It has one method, `GetNext` that is supposed to get the next one in a sequence of items. An example implementation that uses a simple array as the underlying store is also included. Once you have an instance of `INextProvider<T>`, say, called `nextProvider`, you can then extract an `IQueryable<T>` out of it with this call:

	var query = nextProvider.AsQueryable();

You can then use standard LINQ on top of it.

#### Supported Methods
The following methods are supported:

+ All
+ Any
+ Cast
+ Count
+ Distinct
+ ElementAt
+ ElementAtOrDefault
+ First
+ FirstOrDefault
+ Last
+ LastOrDefault
+ LongCount
+ OfType
+ Select
+ SelectMany
+ Single
+ SingleOrDefault
+ Skip
+ Take
+ Where


#### Not Supported Methods
The following methods are not supported - the only reason being I did not have the time to implement them.

+ Aggregate
+ Average
+ Concat
+ Contains
+ DefaultIfEmpty
+ Except
+ GroupBy
+ GroupJoin
+ Intersect
+ Join
+ Max
+ Min
+ OrderBy
+ OrderByDescending
+ Reverse
+ SequenceEqual
+ SkipWhile
+ Sum
+ TakeWhile
+ ThenBy
+ ThenByDescending
+ Union
+ Zip

#### How It Works
The entry point is `NextProviderQueryable` which implements `IQueryable<T>` and uses `NextProviderQueryProvider` as its `Provider` and returns a `NextProviderEnumerator` from its `GetEnumerator()` call. This means that whenever one of the LINQ methods are called on an instance of `NextProviderQueryable`, one of the following happens:

+ If the method is something that creates another queryable out of the existing one (e.g. `Where`, `Select`, `SelectMany`, `Cast`, etc.), `NextProviderQueryProvider.CreateQuery()` is called. That, in turn, creates a new instance of `NextProviderQueryable`, but with the `Expression` set to what has been passed in. Thus, every call to `CreateQuery` ends up creating a new queryable with the `Expression` property representing the complete call.

+ If the method is something that enumerates a queryable (e.g. `ToList`, `ToArray`, etc. or a `foreach` loop), the `GetEnumerator()` method is called and enumeration starts. This means that `NextProviderEnumerator` takes place. This object is initialized with the current value of `Expression` as of the time of enumeration, thus it has complete information to parse it, figure out what needs to be done, and then do it using the `INextProvider` that it is assigned. The class `ExpressionParser` is used to convert the expression into a series of "nodes" that act on each item in the underlying `INextProvider` and do the appropriate thing based on what it is (e.g. if it's a `WhereNode`, it will have a predicate that it will run on each item).

+ If the method is something that returns a scalar (e.g. `Any`, `All`, `First`, etc.), `NextProviderQueryProvider.Execute` is called. In our case, we simply pass control to `NextProviderEnumerator` to enumerate as mentioned in the previous point, and then perform the appropriate action. We do this by getting an `IEnumerable<T>` that uses `NextProviderEnumerator` as its enumerator (and that is the `NextProviderEnumerable` class), and then calling the appropriate `IEnumerable` version of the `IQueryable` method that has been called. All of this is handled by the `ExpressionExecutor` class.