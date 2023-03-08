using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace B16v1.Models
{
    //MVC model Rasporeda iz XML fajla. NAPOMENA: Zbog prirode modela nakon pokretanja aplikacije pravi se baza.
    public class Raspored
    {
        //za Key dodajte using System.ComponentModel.DataAnnotations;
        /*Model mora imaty Key, ali ako za key stavimo neki od property-ja koje postoje u xml fajlu,
         imacemo ili gresku ili pogresan ispis na veb strani. Zato treba dodati id koji se ne ispisuje.
        Ako naknadno to uradimo tj. izmenimo model, onda se on ne poklapa sa bazom koja ce se napraviti
        pri pokretanju aplikacije i onda postoje 2 resenja problema:
        1. obrisati bazu, i opet pokrenuti aplikaciju(preporuka jer je lakse i brze)
        2. code first migrations pristupom azurirati bazu izmenjenim modelom
        
         https://stackoverflow.com/questions/20203492/entitytype-has-no-key-defined-error
         */


        [Key]  
        public int Id { get; set; }
        public int Rbr { get; set; }      
       // public string DanUNedelji { get; set;}
       /*TODO DanUNedelji da bude tipa string*/
        public int DanUNedelji { get; set; }
        public string Predmet { get; set; }
    }

    /*Za DbContext klasu se mora instalirati paket Entity Framework koristeci nuget package menager,
     * potrebna internet konekcija.
     * DbSet:
     * https://learn.microsoft.com/en-us/dotnet/api/system.data.entity.dbset?view=entity-framework-6.2.0
     * DbContext:
     * https://learn.microsoft.com/en-us/dotnet/api/system.data.entity.dbcontext?view=entity-framework-6.2.0
     * 
     * 
     */
    public class RasporedDbContext : DbContext
    { 
        public DbSet<Raspored> Rasporedi { get; set; }

        /*ovo nije praksa u MVC-u ali jeste jedan nacin ako ne znate bolji tj. preko DbSet a to su liste:*/
        public List<Raspored> Rasporedi_List { get; set; }

    }
}