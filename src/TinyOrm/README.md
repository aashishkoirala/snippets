### Tiny ORM
##### by [Aashish Koirala](http://aashishkoirala.github.io)
---

This is a very simple example implementation of an ORM for SQL Server that is meant to illustrate how to write your own LINQ provider. It lets you create an `IQueryable<T>` out of a `SqlConnection` instance and then do LINQ on top of it. A simple fluent mapping interface is also provided.

Here's an example of usage:

	Mapper.For<MyThing>("my_thing_tbl")
	    .Member(x => x.Id, "id")
	    .Member(x => x.Name "thing_name")
	    .Member(x => x.Date "thing_date");
	
	using (var conn = new SqlConnection("..."))
	{
	    conn.Profile(Console.WriteLine); // Write generated query to console.
	    conn.Open();

	    var query = conn.Query<MyThing>();
	
	    var things = query
	        .Where(x => x.Id < 1000)
	        .OrderBy(x => x.Name)
	        .Select(x => new {x.Id, x.Name, x.Date})
	        .ToArray();
	
		// OR:

		things = (from item in query
					where item.Id < 1000
					orderby item.Name
					select new { item.Id, item.Name, item.Date }).ToArray();

	    // ...
	}

#### Limitations
As this is supposed to be just an example and not meant for production, there are quite a few limitations: 

+ Only reads are supported
+ Only the following methods are supported (and only some variants of them in some cases):
+ Any
+ Count
+ First
+ FirstOrDefault
+ Select
+ Single
+ SingleOrDefault
+ Where
+ OrderBy
+ OrderByDescending
+ ThenBy
+ ThenByDescending

#### How It Works
This section in progress.
