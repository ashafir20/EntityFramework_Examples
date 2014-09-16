using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess;
using Model;

namespace BreakAwayConsole
{
    class AddChangeDeleteEntities
    {
        //----------------------------------------------
        //Chapter 2 - Adding Changing And Deleting Entities
        //----------------------------------------------


        //Adding New Entities
        //--------------------------

        private static void AddMachuPicchu()
        {
            using (var context = new BreakAwayContext())
            {
                var machuPicchu = new Destination
                {
                    Name = "Machu Picchu",
                    Country = "Peru"
                };

                context.Destinations.Add(machuPicchu);
                context.SaveChanges();
            }
        }


        //Changing Existing Entities
        private static void ChangeGrandCanyon()
        {
            using (var context = new BreakAwayContext())
            {
                var canyon = (from d in context.Destinations
                              where d.Name == "Grand Canyon"
                              select d).Single();

                canyon.Description = "227 mile long canyon.";
                context.SaveChanges();
            }
        }


        //Deleting Existing Entities

      /* To delete an entity using Entity Framework, you use the Remove method on DbSet.
        Remove works for both existing and newly added entities. Calling Remove on an entity
        that has been added but not yet saved to the database will cancel the addition of the
        entity. The entity is removed from the change tracker and is no longer tracked by the
        DbContext. Calling Remove on an existing entity that is being change-tracked will register
        the entity for deletion the next time SaveChanges is called.
       */

        private static void DeleteWineGlassBay()
        {
            using (var context = new BreakAwayContext())
            {
                var bay = (from d in context.Destinations
                           where d.Name == "Wine Glass Bay"
                           select d).Single();

                context.Destinations.Remove(bay);
                context.SaveChanges();
            }
        }

        //Multiple Changes at Once
        private static void MakeMultipleChanges()
        {
            using (var context = new BreakAwayContext())
            {
                var niagaraFalls = new Destination
                {
                    Name = "Niagara Falls",
                    Country = "USA"
                };

                context.Destinations.Add(niagaraFalls);

                var wineGlassBay = (from d in context.Destinations
                    where d.Name == "Wine Glass Bay"
                    select d).Single();

                wineGlassBay.Description = "Picturesque bay with beaches.";

                context.SaveChanges();

            }
        }

        //The “Find or Add” Pattern
        //Adding a new Person if existing record doesn’t exist
        private static void FindOrAddPerson()
        {
            using (var context = new BreakAwayContext())
            {
                var ssn = 123456789;
                var person = context.People.Find(ssn)
                ?? context.People.Add(new Person
                {
                    SocialSecurityNumber = ssn,
                    FirstName = "<enter first name>",
                    LastName = "<enter last name>"
                });

                Console.WriteLine(person.FirstName);
            }
        }
    }
}
