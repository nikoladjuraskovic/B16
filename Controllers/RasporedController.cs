using Antlr.Runtime.Tree;
using B16v1.Models;
using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace B16v1.Controllers
{
    public class RasporedController : Controller
    {
        //Kontroler je dodat rucno kao MVC 5 Controller - Empty

        //fja racuna redni broj dana u nedelji
        public int RedniBrojDana(string dan)
        {
            switch (dan)
            {
                case "Ponedeljak":
                    return 1;
                case "Utorak":
                    return 2;
                case "Sreda":
                    return 4;
                case "Četvrtak":
                    return 5;
                case "Petak":
                    return 5;
                default:
                    break;
            }

            return 0;
        }

        private RasporedDbContext db = new RasporedDbContext();
     
       // GET: Raspored
        public ActionResult Index(string fileName)
        {
           
            db.Rasporedi_List = new List<Raspored>();


            /*Ako budete imali problem sa pravima pristupa xml fajlu, onda:
            https://www.c-sharpcorner.com/blogs/access-to-path-is-denied-permissions-error#:~:text=Resolving%20%22Access%20Path%20is%20Denied%22%20error&text=To%20grant%20ASP.NET%20access,boxes%20for%20the%20desired%20access.
            https://stackoverflow.com/questions/19724297/asp-net-getting-the-error-access-to-the-path-is-denied-while-trying-to-upload 
            
            Pre ovoga proverite da niste pogresili url fajla
             
             */

            string filePath = Server.MapPath("~/App_Data" + "/raspored.xml");

            /*brisemo podatke iz baze da bi svaki put bio ispisan samo sadrzaj XML file-a
             * tj. da se podaci u bazi i DbSet-u nebi duplirali
             https://stackoverflow.com/questions/10448684/how-should-i-remove-all-elements-in-a-dbset
            https://learn.microsoft.com/en-us/dotnet/api/system.data.entity.dbset.remove?view=entity-framework-6.2.0
             
             */

            foreach (var entity in db.Rasporedi)
            {
                db.Rasporedi.Remove(entity); // koristimo metod Remove
            }

           db.SaveChanges(); // sacuvaj izmenu brisanja podataka iz baze

            int Rbr = 0;
            string DanUNedelji = "";
            string Predmet = "";

            using (XmlReader reader = XmlReader.Create(filePath))
            {
                while (reader.Read())
                {
                    switch(reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if(reader.Name == "Rbr")
                            {
                                Rbr = int.Parse(reader.ReadElementContentAsString().Trim());
                            }
                            else if(reader.Name == "DanUNedelji")
                            {
                                DanUNedelji = reader.ReadElementContentAsString().Trim();
                            } else if(reader.Name == "Predmet")
                            {
                                Predmet = reader.ReadElementContentAsString().Trim();
                            }

                            break;

                        case XmlNodeType.EndElement:
                            /*novi raspored ubacujemo samo onda kada smo procitali sve podatke o jednom rasporedu, tj. kada
                             smo dosli do zatvarajuceg tag-a rasporeda tj. </Raspored>*/
                            if (reader.Name == "Raspored")
                            {
                                Raspored raspored = new Raspored();
                                raspored.Rbr = Rbr;
                                raspored.DanUNedelji = RedniBrojDana(DanUNedelji);
                                raspored.Predmet = Predmet;
                                //System.Diagnostics.Debug.WriteLine("Redni Broj: " + raspored.Rbr);
                                db.Rasporedi.Add(raspored);
                                
                                db.Rasporedi_List.Add(raspored);
                            }

                            break;
                    }
                }
            }

            /*
             IEnumerable OrderBy metod:
        https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.orderby?redirectedfrom=MSDN&view=netframework-4.8#System_Linq_Enumerable_OrderBy__2_System_Collections_Generic_IEnumerable___0__System_Func___0___1
             
             */

            /*
             Sortiranje LINQ upitom:
            https://stackoverflow.com/questions/14593426/entity-framework-order-by-issue
            https://stackoverflow.com/questions/1700587/order-by-col1-col2-using-entity-framework
            https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.orderby?redirectedfrom=MSDN&view=netframework-4.8.1#System_Linq_Enumerable_OrderBy__2_System_Collections_Generic_IEnumerable___0__System_Func___0___1__System_Collections_Generic_IComparer___1__
            https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.icomparer-1?view=netframework-4.8.1

             */
            /*
            Comparer comparator = new Comparer();
            //TODO Custom comparator pozvati
            IOrderedQueryable<Raspored> rasporedSortiran = db.Rasporedi.OrderBy(x => x.Rbr).ThenBy(x => x.DanUNedelji, comparator);
            IOrderedQueryable<Raspored> rasporedSortiran = db.Rasporedi.OrderBy();

            */

            IOrderedQueryable<Raspored> rasporedSortiran = db.Rasporedi.OrderBy(x => x.Rbr).ThenBy(x => x.DanUNedelji);

            db.SaveChanges(); /*BITNO!!! Da bismo sacuvali promene u dbSet-u.*/

            /*Pogledu mozemo slati DbSet ili List, mada je bolje db set s obzirom da radimo MVC arhitekturu*/
            //return View(db.Rasporedi.ToList());
            //return View(db.Rasporedi_List());
            return View(rasporedSortiran.ToList());
        }
    }

    /*
     https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.icomparer-1?view=netframework-4.8.1
     
     */
    /*
    public class Comparer : IComparer<Raspored>
    {
        public int Compare(Raspored r1, Raspored r2)
        {
            int r1Dan = RedniBrojDana(r1.DanUNedelji);
            int r2Dan = RedniBrojDana(r2.DanUNedelji);

            if (r1Dan < r2Dan)
                return -1;
            else if (r1Dan > r2Dan)
                return 1;
            else

            return 0;
        }


        /*fja vraca redni broj dana u nedelji*/
    /*
        public int RedniBrojDana(string dan)
        {
            switch(dan)
            {
                case "Ponedeljak":
                    return 1;
                case "Utorak":
                    return 2;
                case "Sreda":
                    return 4;
                case "Četvrtak":
                    return 5;
                case "Petak":
                    return 5;
                default:
                    break;
            }

            return 0;
        }
        
    }
    */

}