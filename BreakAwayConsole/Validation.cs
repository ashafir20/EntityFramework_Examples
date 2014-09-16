using System;
using DataAccess;
using Model;

namespace BreakAwayConsole
{
    public class Validation
    {
        /* Chapter 6 Validation API 

        Validating a Single Object on Demand with GetValidationResult
         * ------------------------------------------------------------

         * In Destionation Class
         * -------------------------------------------
            [MaxLength(10)]
            public string LastName { get; set; }
         * ---------------------------------------------
 
         *  Now let’s see what happens when we set the length to a string with more than ten
            characters. GetValidationResult allows you to explicitly validate a single entity. It returns
            a ValidationResult type that contains three important members. We’ll focus on
            just one of those for now, the IsValid property, which is a Boolean that indicates if the
            instance passed its validation rules. Let’s use that to validate a Person instance.
         * 
         * 
 
       */

        private static void ValidateNewPerson()
        {
            var person = new Person
            {
                FirstName = "Julie",
                LastName = "Lerman",
                Photo = new PersonPhoto { Photo = new Byte[] { 0 } }
            };
            using (var context = new BreakAwayContext())
            {
                if (context.Entry(person).GetValidationResult().IsValid)
                {
                    Console.WriteLine("Person is Valid");
                }
                else
                {
                    Console.WriteLine("Person is Invalid");
                }
            }
        }

        /*      If you run this method from the Main method, you will see the message “Person is Valid”
                in the console windows.
 
                The GetValidationResult method calls the necessary logic to validate any Validatio
                nAttributes defined on the object’s properties. It then looks to see if the type has a
                CustomValidationAttribute or implements the IValidatableObject interface and if it
                does, calls its Validate method. You’ll see this in action later in this chapter.
 
 
 
         Specifying Property Rules with ValidationAttribute Data Annotations
         -----------------------------------------------------------------------
 
 
 
 
      */


    }
}