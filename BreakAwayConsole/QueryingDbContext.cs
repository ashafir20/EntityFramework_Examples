using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using DataAccess;
using Model;

namespace BreakAwayConsole
{
    internal class QueryingDbContext
    {
        private static void Main(string[] args)
        {
            Database.SetInitializer(new InitializeBagaDatabaseWithSeedData());

            // Call the latest example method here

            // NOTE: Some examples will change data in the database. Ensure that you only call the 
            //       latest example method. The InitializeBagaDatabaseWithSeedData database initializer 
            //       (registered above) will take care of resetting the database before each run.
        }

/*      Entity Framework queries are written using a .NET Framework feature known as Language
        Integrated Query, or LINQ for short. As the name suggests, LINQ is tightly
        integrated with the .NET programming experience and provides a strongly typed query
        language over your model. Strongly typed simply means that the query is defined using
        the classes and properties that make up your model. This provides a number of benefits
        such as compile-time checks to ensure your queries are valid and the ability to provide
        IntelliSense as you write your queries.
        For Entity Framework this provider is known as LINQ to Entities and is responsible for taking
        your LINQ query and translating it into a SQL query against the database you are
        targeting. The information you supplied to Entity Framework about the shape of your
        model and how it maps to the database is used to perform this translation. Once the
        query returns, Entity Framework is responsible for copying the data into instances of
        the classes that make up your model. */

        //----------------------------------------------
        //Chapter 2 - Querying With DBContext
        //----------------------------------------------

        // Add example methods here
        private static void PrintAustralianDestinations()
        {
            /*  A DbContext (and its underlying ObjectContext) are responsible for managing and
        tracking changes to instances of the classes in its model. These classes are also responsible
        for managing a connection to the database. It’s important to ensure that any resources
        used to perform these operations are cleaned up when the DbContext instance
        is no longer needed. DbContext implements the standard .NET IDisposable interface,
        which includes a Dispose method that will release any such resources.
        The examples in this book will make use of the using pattern, which will take care of
        disposing the context when the using block completes*/
            using (var context = new BreakAwayContext())
            {
                var query = from d in context.Destinations
                    where d.Country == "Australia"
                    select d;

/*              The query is sent to the database when the first result is requested by the application:
                that’s during the first iteration of the foreach loop. Entity Framework doesn’t pull back
                all the data at once, though. The query remains active and the results are read from the
                database as they are needed. By the time the foreach loop is completed, all the results
                have been read from the database.
                One important thing to note is that Entity Framework will query the database every
                time you trigger an iteration over the contents of a DbSet. This has performance implications
                if you are continually querying the database for the same data. To avoid this,
                you can use a LINQ operator such as ToList to copy the results into a list. You can then
                iterate over the contents of this list multiple times without causing multiple trips to the
                database.*/
                foreach (var destination in query)
                {
                    Console.WriteLine(destination.Name);
                }
            }
        }

        private static void SameWithSortingByName()
        {
            using (var context = new BreakAwayContext())
            {
                var query = from d in context.Destinations
                    where d.Country == "Australia"
                    orderby d.Name
                    select d;

                //Method syntax
                var query1 = context.Destinations
                    .Where(d => d.Country == "Australia")
                    .OrderBy(d => d.Name);

                foreach (var destination in query)
                {
                    Console.WriteLine(destination.Name);
                }
            }
        }


        //Query syntax for combining filter and sort and select
        private static void PrintDestinationNameOnlyWithProjection()
        {
            using (var context = new BreakAwayContext())
            {
                var query = from d in context.Destinations
                    where d.Country == "Australia"
                    orderby d.Name
                    select d.Name;

                //Method syntax
                var query1 = context.Destinations
                    .Where(d => d.Country == "Australia")
                    .OrderBy(d => d.Name)
                    .Select(d => d.Name);

                foreach (var name in query)
                {
                    Console.WriteLine(name);
                }
            }
        }


        //Finding a Single Object
        private static void FindDestination()
        {
            Console.Write("Enter id of Destination to find: ");
            var id = int.Parse(Console.ReadLine());
            using (var context = new BreakAwayContext())
            {
/*              Find has another benefit. While the SingleOrDefault query above will always query the
                database, Find will first check to see if that particular person is already in memory,
                being tracked by the context. If so, that’s what will be returned. If not, it will make the
                trip to the database.*/
                var destination = context.Destinations.Find(id);
                if (destination == null)
                {
                    Console.WriteLine("Destination not found!");
                }
                else
                {
                    Console.WriteLine(destination.Name);
                }
            }
        }

        private static void FindGreatBarrierReef()
        {
            using (var context = new BreakAwayContext())
            {
                var query = from d in context.Destinations
                    where d.Name == "Great Barrier Reef"
                    select d;

                var reef1 = query.Single();
                //Throws exception if no one was found or found more than 1

                var reef = query.SingleOrDefault();
                //Throws exception if found more than 1

/*              If two rows are found, Single and SingleOrDefault will throw because there is not a
                single result. If you just want the first result, and aren’t concerned if there is more than
                one result, you can use First or FirstOrDefault.*/

/*              One important thing to remember is that LINQ queries against a DbSet always send a
                query to the database to find the data. So, if the Great Barrier Reef was a newly added
                Destination that hadn’t been saved to the database yet, the queries in Example 2-15
                and Example 2-16 won’t be able to locate it.*/

                if (reef == null)
                {
                    Console.WriteLine("Can't find the reef!");
                }
                else
                {
                    Console.WriteLine(reef.Description);
                }

            }
        }


        /*  So far you’ve used LINQ to query a DbSet directly, which always results in a SQL query
            being sent to the database to load the data. You’ve also used the Find method, which
            will look for in-memory data before querying that database. Find will only query based
            on the key property though, and there may be times when you want to use a more
            complex query against data that is already in memory and being tracked by your
            DbContext.*/

/*          One of the reasons you may want to do this is to avoid sending multiple queries to the
            database when you know that all the data you need is already loaded into memory.
            Back in Example 2-5, we saw one way to do this was to use ToList to copy the results
            of a query into a list. While this works well if we are using the data within the same
            block of code, things get a little messy if we need to start passing that list around our
            application. For example, we might want to load all Destinations from the database
            when our application loads. Different areas of our application are then going to want
            to run different queries against that data. In some places we might want to display all
            Destinations, in others we might want to sort by Name, and in others we might want
            to filter by Country. Rather than passing around a list of Destination objects, we can
            take advantage of the fact that our context is tracking all the instances and query its
            local data.
 * 
            Another reason may be that you want the results to include newly added data, which
            doesn’t yet exist in the database. Using ToList on a LINQ query against a DbSet will
            always send a query to the database. This means that any new objects that don’t yet
            exist in the database won’t be included in the results. Local queries, however, will
            include newly created objects in the results.*/


/*          The in-memory data for a DbSet is available via the Local property. Local will return all
            the data that has been loaded from the database plus any newly added data. Any data
            that has been marked as deleted but hasn’t been deleted from the database yet will be
            filtered out for you. More information on how entities get into these different states is
            available in Chapter 3.
            Let’s start with the very simple task of finding out how many Destinations are in memory
            and available to be queried*/

        private static void GetLocalDestinationCount()
        {
            using (var context = new BreakAwayContext())
            {
                foreach (var destination in context.Destinations)
                {
                    Console.WriteLine(destination.Name);
                }

                var count = context.Destinations.Local.Count;
                Console.WriteLine("Destinations in memory: {0}", count);

             /* This code iterates over the Destinations set, causing the data to be loaded from
                the database. Because the data is loaded when we get the count from the Local property,
                we now see a nonzero result when we run the application */
            }
        }

/*      Using the Load Method to Bring Data into Memory
        -----------------------------------------------
        Iterating over the contents of a DbSet with a foreach loop is one way to get all the data
        into memory, but it’s a little inefficient to do that just for the sake of loading data. It’s
        also a little unclear what the intent of the code is, especially if the iteration code doesn’t
        directly precede the local query.
        Fortunately the DbContext API includes a Load method, which can be used on a
        DbSet to pull all the data from the database into memory.*/

        private static void GetLocalDestinationCountWithLoad()
        {
            using (var context = new BreakAwayContext())
            {
                context.Destinations.Load();
                var count = context.Destinations.Local.Count;
                Console.WriteLine("Destinations in memory: {0}", count);
            }
        }


/*      Because Load is an extension method on IQueryable<T>, we can also use it to load the
        results of a LINQ query into memory, rather than the entire contents of a set. For
        example, let’s say we only wanted to load Australian Destinations into memory and
        then run a few local queries on that subset of data.
 */
        private static void LoadAustralianDestinations()
        {
            using (var context = new BreakAwayContext())
            {
                var query = from d in context.Destinations
                            where d.Country == "Australia"
                            select d;
                query.Load();
                var count = context.Destinations.Local.Count;
                Console.WriteLine("Aussie destinations in memory: {0}", count);
            }
        }

        //Running LINQ Queries Against Local
        //----------------------------------------
        private static void LocalLinqQueries()
        {
            using (var context = new BreakAwayContext())
            {
                context.Destinations.Load();
                var sortedDestinations = from d in context.Destinations.Local
                    orderby d.Name
                    select d;
                Console.WriteLine("All Destinations:");
                foreach (var destination in sortedDestinations)
                {
                    Console.WriteLine(destination.Name);
                }

                var aussieDestinations = from d in context.Destinations.Local
                                         where d.Country == "Australia"
                                         select d;
                Console.WriteLine();
                Console.WriteLine("Australian Destinations:");
                foreach (var destination in aussieDestinations)
                {
                    Console.WriteLine(destination.Name);
                }
            }
        }


     /*  Note 
        -----------------------
        While Load and Local are great if you want to reduce the number of queries that get
        run against the database just remember that pulling all your data into memory may be
        an expensive operation. If you are running multiple queries that only return a subset
        of your data you’ll probably get better performance by letting these queries hit the
        database and just pull back the data you actually need.*/


        //Loading Related Data
        //-----------------------

       //Lazy Loading
       //-----------------------
/*      Lazy loading related data is the most transparent to your application and involves letting
        Entity Framework automatically retrieve the related data for you when you try to
        access it. For example, you may have the Grand Canyon destination loaded. If you then
        use the Lodgings property of this Destination, Entity Framework will automatically
        send a query to the database to load all Lodgings at the Grand Canyon. It will appear
        to your application code as if the Lodgings property was always populated.

             2) Your POCO class must be public and not sealed.
             1) The navigation properties that you want to be lazy loaded must also be marked as virtual
        */

        //Example for a lazy loaded property
        // public virtual List<Lodging> Lodgings { get; set; }


        /*    Understanding the downsides of lazy loading
                -----------------------
                Lazy loading is very simple because your application doesn’t really need to be aware
                that data is being loaded from the database. But that is also one of its dangers! Improper
                use of lazy loading can result in a lot of queries being sent to the database. For example,
                you might load fifty Destinations and then access the Lodgings property on each. That
                would result in 51 queries against the database—one query to get the Destinations and
                then for each of the fifty Destinations, to load that Destination’s Lodgings. In cases like
                this it may be much more efficient to load all that data in a single query, using a SQL
                join in the database query. This is where eager loading comes into play.
          
          
              Eager Loading
              -----------------------
         *    Eager loading related data relies on you telling Entity Framework what related data to
              include when you query for an entity type. Entity Framework will then use a JOIN in
         *    the generated SQL to pull back all of the data in a single query. Let’s assume we want
              to run though all Destinations and print out the Lodgings for each. Add a TestEager
              Loading method that queries for all Destinations and uses Include to also query for the
              associated Lodgings
         * 
         
         */

       // Eager Loading
       // -----------------------
        private static void TestEagerLoading()
        {
            using (var context = new BreakAwayContext())
            {
                var allDestinations = context
                .Destinations
                .Include(d => d.Lodgings);
                foreach (var destination in allDestinations)
                {
                    Console.WriteLine(destination.Name);
                    foreach (var lodging in destination.Lodgings)
                    {
                        Console.WriteLine(" - " + lodging.Name);
                    }
                }
            }
        }


        /*      Understanding the downsides of eager loading
         * -----------------------
            One thing to bear in mind with eager loading is that fewer queries aren’t always better.
            The reduction in the number of queries comes at the expense of the simplicity of the
            queries being executed. As you include more and more data, the number of joins in the
            query that is sent to the database increases and results in a slower and more complex
            query. If you need a significant amount of related data, multiple simpler queries will
            often be significantly faster than one big query that returns all the data.
         */
    }

}